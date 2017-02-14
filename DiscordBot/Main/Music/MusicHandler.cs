using Discord;
using Discord.Commands;
using Discord.Audio;
using System;
using System.Threading.Tasks;
using NAudio.Wave;
using VideoLibrary;
using System.IO;
using System.Collections.Generic;
using NReco.VideoConverter;

namespace DiscordBot.Main.Music
{
    class MusicHandler
    {
        
        private CommandService commands;
        private DiscordClient discordClient;
        private IAudioClient discordAudio;
        private Channel voiceChannel;
        private bool playing;
        private List<Song> queue;
        public static FFMpegConverter videoconverter;
        private int songcounter;
        private int playedcounter;

        public MusicHandler(CommandService c, DiscordClient dc)
        {
            discordClient = dc;
            commands = c;
            playing = false;
            queue = new List<Song>();
            videoconverter = new FFMpegConverter();
            songcounter = 0;
            playedcounter = 0;

            discordClient.UsingAudio(x => // Opens an AudioConfigBuilder so we can configure our AudioService
            {
                x.Mode = AudioMode.Outgoing; // Tells the AudioService that we will only be sending audio
            });

            commands.CreateCommand("add")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Add a song to the queue")
                .Do(async (e) => await Add(e));

            commands.CreateCommand("addfile")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Add a song to the queue via filepath (mod)")
                .Do(async (e) => await AddFile(e));

            commands.CreateCommand("play")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Play the current queue")
                .Do(async (e) => await Play(e));

            commands.CreateCommand("silence")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Silence the singing Biri")
                .Do(async (e) => await Silence(e));

            commands.CreateCommand("summon")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Summon Biri to your voice channel")
                .Do(async (e) => await Summon(e));

        }

        private async Task Add(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            try
            {
                var youTube = YouTube.Default; // starting point for YouTube actions
                var video = youTube.GetVideo(e.GetArg("param")); // gets a Video object with info about the video
                var mp4file = Path.Combine(Environment.CurrentDirectory, "Music", video.FullName);
                Console.WriteLine(mp4file);
                File.WriteAllBytes(mp4file, video.GetBytes());
                var mp3file = Path.Combine(Environment.CurrentDirectory, "Music", songcounter + ".mp3");
                songcounter++;
                videoconverter.ConvertMedia(mp4file, mp3file, "mp3");
                System.IO.File.Delete(mp4file);
                MyBot.Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + e.Channel.Name + ") Song added: " + video.FullName, e.Channel.Name + "_log");
            }
            catch (Exception ex)
            {
                await e.Channel.SendMessage("I could not download that...");
                Console.WriteLine(ex.StackTrace);
                return;
            }
            await Play(e);
        }

        private async Task AddFile(Discord.Commands.CommandEventArgs e)
        {
            var path = @"C:\Users\dejon\Music\" + e.GetArg("param") + ".mp3";
            if(File.Exists(path))
            {
                System.IO.File.Copy(path, @"C:\Users\dejon\Music\DiscordBot\" + songcounter + ".mp3");
                songcounter++;
                await Play(e);
            }
        }

        private void ClearQueueFromDisk()
        {
            var i = 0;

            for (var currentfile = Path.Combine(Environment.CurrentDirectory, "Music", i + ".mp3"); File.Exists(currentfile); i++)
            {
                System.IO.File.Delete(currentfile);
                i++;
                currentfile = Path.Combine(Environment.CurrentDirectory, "Music", i + ".mp3");
            }
            playedcounter = 0;
            songcounter = 0;
            Console.WriteLine("Songs deleted from disk");
        }

        private async Task Play(Discord.Commands.CommandEventArgs e)
        {
            if (playing) return;
            if(discordAudio == null || discordAudio.State != ConnectionState.Connected)
            {
                await Summon(e);
            }
            playing = true;
            var currentfile = Path.Combine(Environment.CurrentDirectory, "Music", playedcounter + ".mp3");
            while (File.Exists(currentfile))
            {
                await SendAudio(e.Channel, Path.Combine(Environment.CurrentDirectory, "Music", playedcounter + ".mp3"));
                playedcounter++;
                playing = false;
                ClearQueueFromDisk();
                await e.Channel.SendMessage("Queue empty!");
            }
        }
        
        private async Task Silence(Discord.Commands.CommandEventArgs e)
        {
            if(e.User.VoiceChannel != voiceChannel)
            {
                await e.Channel.SendMessage("You are not even listsning to me!");
                return;
            }
            if(discordAudio == null || discordAudio.State != ConnectionState.Connected)
            {
                await e.Channel.SendMessage("Im not singing right now though...");
                return;
            }
            await discordAudio.Disconnect();
        }

        private async Task Summon(Discord.Commands.CommandEventArgs e)
        {
            voiceChannel = e.User.VoiceChannel;

            discordAudio = await discordClient.GetService<AudioService>() // We use GetService to find the AudioService that we installed earlier. In previous versions, this was equivelent to _client.Audio()
                    .Join(voiceChannel); // Join the Voice Channel, and return the IAudioClient.
        }

        public async Task SendAudio(Channel channel, string filePath)
        {
            try
            {
                MyBot.Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + channel.Name + ") Song playing: " + filePath, channel.Name + "_log");
                var channelCount = discordClient.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
                var OutFormat = new WaveFormat(48000, 16, 2); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
                using (var MP3Reader = new Mp3FileReader(filePath)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
                using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
                {
                    resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                    int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                    byte[] buffer = new byte[blockSize];
                    int byteCount;

                    while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                    {
                        if (byteCount < blockSize)
                        {
                            // Incomplete Frame
                            for (int i = byteCount; i < blockSize; i++)
                                buffer[i] = 0;
                        }
                        discordAudio.Send(buffer, 0, blockSize); // Send the buffer to Discord
                    }
                }
            } catch (Exception e)
            {
                await channel.SendMessage("Something went teribly wrong.. ABORT ABORT \\o/");
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
