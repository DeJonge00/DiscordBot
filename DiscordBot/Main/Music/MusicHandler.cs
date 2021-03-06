﻿using Discord;
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
        private bool skipped;

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

            commands.CreateCommand("queue")
                .Parameter("param", ParameterType.Unparsed)
                .Description("List the queued music")
                .Do(async (e) => await Queue(e));

            commands.CreateCommand("silence")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Silence the singing Biri")
                .Do(async (e) => await Silence(e));

            commands.CreateCommand("skip")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Skip the current song")
                .Do(async (e) => await Skip(e));

            commands.CreateCommand("summon")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Summon Biri to your voice channel")
                .Do(async (e) => await Summon(e));

        }

        private async Task Add(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if(playing && e.User.VoiceChannel != voiceChannel)
            {
                await e.Channel.SendMessage("Join vc NOW :angry:");
                return;
            }
            var param = e.GetArg("param");
            if(param.Length<=0)
            {
                await e.Channel.SendMessage("I cannot add nothing to the queue");
                return;
            }
            try
            {
                var youTube = Client.For(YouTube.Default); // starting point for YouTube actions
                var video = youTube.GetVideo(e.GetArg("param")); // gets a Video object with info about the video
                var vidfile = Path.Combine(Environment.CurrentDirectory, "Music", video.FullName);
                Console.WriteLine(vidfile);
                var mp3file = Path.Combine(Environment.CurrentDirectory, "Music", video.Title + ".mp3");
                if (!File.Exists(mp3file))
                {
                    byte[] bytes = await video.GetBytesAsync();
                    File.WriteAllBytes(vidfile, bytes);
                    songcounter++;
                    videoconverter.ConvertMedia(vidfile, mp3file, "mp3");
                    File.Delete(vidfile);
                }
                queue.Add(new Song(mp3file, video.Title, e.User.Name));
                await e.Channel.SendMessage("**" + video.Title + "** has been added to the queue by *" + e.User.Name + "*!");
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
            await e.Message.Delete();
            if (playing && e.User.VoiceChannel != voiceChannel)
            {
                await e.Channel.SendMessage("Join vc NOW :angry:");
                return;
            }
            var param = e.GetArg("param");
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("I cannot add nothing to the queue");
                return;
            }
            var path = @"C:\Users\dejon\Music\" + param  + ".mp3";
            var name = param.Split('\\')[param.Split('\\').Length-1];
            if (!File.Exists(path))
            {
                await e.Channel.SendMessage("Could not find file: " + param);
                return;
            }
            queue.Add(new Song(path, name, e.User.Name));
            await e.Channel.SendMessage("**" + name + "** has been added to the queue by *" + e.User.Name + "*!");
            await Play(e);
        }

        private async Task Play(Discord.Commands.CommandEventArgs e)
        {
            if (playing) return;
            if (discordAudio == null || discordAudio.Channel != e.User.VoiceChannel)
            {
                await Summon(e);
            }
            playing = true;
            Console.WriteLine(queue.Count);
            while(queue.Count>0)
            {
                await SendAudio(e.Channel, queue[0]);
                queue.RemoveAt(0);
            }
            await discordAudio.Disconnect();
            queue = new List<Song>();
            playing = false;
            await e.Channel.SendMessage("Queue empty!");
        }

        private async Task Queue(Discord.Commands.CommandEventArgs e)
        {
            var msg = "The awesome music queue: (" + queue.Count + " songs in the queue)";
            for(int i = 0; i < queue.Count && i < 10; i++)
            {
                msg += "\n" + (i+1) + "\t" + queue[i].title + ", requested by: *" + queue[i].user + "*";
            }
            await e.Channel.SendMessage(msg);
        }

        private async Task Silence(Discord.Commands.CommandEventArgs e)
        {
            if(e.User.VoiceChannel != voiceChannel)
            {
                await e.Channel.SendMessage("You are not even listening to me!");
                return;
            }
            if(discordAudio == null || discordAudio.State != ConnectionState.Connected)
            {
                await e.Channel.SendMessage("Im not singing right now though...");
                return;
            }
            await discordAudio.Disconnect();
        }

        private async Task Skip(Discord.Commands.CommandEventArgs e)
        {
            if(!playing)
            {
                await e.Channel.SendMessage("Im not singing baka!");
                return;
            }
            if(e.User.Name != queue[0].user && e.User.Id != Constants.NYAid)
            {
                await e.Channel.SendMessage("Who are you to skip this beautifull song??");
            }
            skipped = true;
            await e.Channel.SendMessage("Skipping this song!");
        }

        private async Task Summon(Discord.Commands.CommandEventArgs e)
        {
            try { await e.Message.Delete(); } catch{ /* NO ERROR HANDLING OMG!!! o_o */}
            if(e.User.VoiceChannel == null)
            {
                await e.Channel.SendMessage("Join a voicechannel first, you dummy");
                return;
            }
            try
            {
                voiceChannel = e.User.VoiceChannel;
                discordAudio = await discordClient.GetService<AudioService>() // We use GetService to find the AudioService that we installed earlier. In previous versions, this was equivelent to _client.Audio()
                    .Join(voiceChannel); // Join the Voice Channel, and return the IAudioClient.
            } catch
            {
                await e.Channel.SendMessage("Cannot join voicechannel cuz discord is being a bitch :D");
            }
        }

        public async Task SendAudio(Channel channel, Song song)
        {
            try
            {
                MyBot.Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + channel.Name + ") Song playing: " + song.path, channel.Name + "_log");
                var channelCount = discordClient.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
                var OutFormat = new WaveFormat(48000, 16, 2); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
                using (var MP3Reader = new Mp3FileReader(song.path)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
                using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
                {
                    resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                    int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                    byte[] buffer = new byte[blockSize];
                    int byteCount;
                    await channel.SendMessage("Playing *" + song.user + "'s* song **" + song.title + "** now!");
                    MyBot.Log("Playing *" + song.user + "'s* song **" + song.title + "** now!", "music_log");
                    while (playing && !skipped && (byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                    {
                        if (byteCount < blockSize)
                        {
                            // Incomplete Frame
                            for (int i = byteCount; i < blockSize; i++)
                                buffer[i] = 0;
                        }
                        discordAudio.Send(buffer, 0, blockSize); // Send the buffer to Discord
                    }
                    if (skipped) skipped = false;
                }
            } catch (Exception e)
            {
                await channel.SendMessage("Something went teribly wrong.. ABORT ABORT \\o/");
                Console.WriteLine(e.StackTrace);
            }
        }

        public void Quit()
        {
            playing = false;
        }
    }
}
