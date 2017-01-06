using Discord;
using Discord.Commands;
using Discord.Audio;
using System;
using System.Threading.Tasks;
using NAudio.Wave;

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

            commands.CreateCommand("music")
                .Alias("m")
                .Parameter("param", ParameterType.Unparsed)
                .Description("Play music in voicechannel")
                .Do(async (e) => await Handle(e));

        }

        private async Task Handle(CommandEventArgs e)
        {
            var param = e.GetArg("param").Split(' ');
            Console.WriteLine("Music command used");
            if (param.Length <= 0)
            {
                await help(e);
                return;
            }
            switch(param[0])
            {
                case "summon":
                    await summon(e);
                    return;
                case "stop":
                    await stop(e);
                    return;
                case "play":
                    SendAudio(@"C:\Users\dejon\Music\DiscordBot\99.mp3");
                    return;
                default:
                    await e.Channel.SendMessage("Stop using magic like " + param[0]);
                    return;
            }

        }

        private async Task stop(CommandEventArgs e)
        {
            if (!playing)
            {
                await e.Channel.SendMessage("Im not playing baka!");
                return;
            }
            await discordAudio.Disconnect();
            await e.Channel.SendMessage("I quit singing! :angry:");
        }

        private async Task summon(CommandEventArgs e)
        {
            await e.Message.Delete();
            Console.WriteLine("Summon command used");
            if (playing && voiceChannel != null && voiceChannel != e.User.VoiceChannel)
            {
                await e.Channel.SendMessage("Im already playing in another voicechannel baka!");
                return;
            }
            if(!playing)
            {
                voiceChannel = e.User.VoiceChannel;
                if(voiceChannel == null)
                {
                    await e.Channel.SendMessage("Join a vc plz");
                    return;
                }
                discordAudio = await discordClient.GetService<AudioService>().Join(voiceChannel);
                Console.WriteLine("Joined channel " + voiceChannel.Name);
                return;
            }
            await e.Channel.SendMessage("Im already playing somewhere!");
        }

        private async Task help(CommandEventArgs e)
        {
            await e.Channel.SendMessage("Help not available :/");
        }

        public void SendAudio(string filePath)
        {
            var channelCount = discordClient.GetService<AudioService>().Config.Channels; // Get the number of AudioChannels our AudioService has been configured to use.
            var OutFormat = new WaveFormat(48000, 16, channelCount); // Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
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

        }
    }
}
