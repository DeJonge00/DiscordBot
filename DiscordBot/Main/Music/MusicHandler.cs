using Discord;
using Discord.Commands;
using Discord.Audio;
using System;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Threading;
using System.Diagnostics;

namespace DiscordBot.Main.Music
{
    class MusicHandler
    {
        
        private CommandService commands;
        private DiscordClient discordClient;
        private IAudioClient discordAudio;
        private Channel voiceChannel;
        private bool playing;

        public MusicHandler(CommandService c, DiscordClient dc)
        {
            discordClient = dc;
            commands = c;
            playing = false;

            discordClient.UsingAudio(x => // Opens an AudioConfigBuilder so we can configure our AudioService
            {
                x.Mode = AudioMode.Outgoing; // Tells the AudioService that we will only be sending audio
            });

            /*commands.CreateCommand("music")
                .Alias("m")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Play music in voicechannel")
                .Do(async (e) => await Handle(e));*/

            commands.CreateCommand("play")
                .Alias("m")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Add a song to the queue")
                .Do(async (e) => await Play(e));

            commands.CreateCommand("summon")
                .Alias("m")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Summon Biri to your voice channel")
                .Do(async (e) => await Summon(e));

        }

        private async Task Play(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Play comm used");
            SendAudio(@"Main\Music\99.mp3");
        }

        private async Task Summon(Discord.Commands.CommandEventArgs e)
        {
            voiceChannel = e.User.VoiceChannel;

            discordAudio = await discordClient.GetService<AudioService>() // We use GetService to find the AudioService that we installed earlier. In previous versions, this was equivelent to _client.Audio()
                    .Join(voiceChannel); // Join the Voice Channel, and return the IAudioClient.
        }

        public void SendAudio(string filePath)
        {
            Console.WriteLine("Sendaudio: " + filePath);
            var channelCount = discordClient.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
            var OutFormat = new WaveFormat(48000, 16, 2); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
            using (var MP3Reader = new Mp3FileReader(filePath)) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
            using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
            {
                resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                byte[] buffer = new byte[blockSize];
                int byteCount;

                Console.WriteLine("while loop start");
                while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                {
                    if (byteCount < blockSize)
                    {
                        // Incomplete Frame
                        for (int i = byteCount; i < blockSize; i++)
                            buffer[i] = 0;
                    }
                    Console.WriteLine("byteCount: " + byteCount);
                    discordAudio.Send(buffer, 0, blockSize); // Send the buffer to Discord
                }
                discordAudio.Disconnect();
            }
        }
    }
}
