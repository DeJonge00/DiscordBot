using Discord;
using Discord.Commands;
using DiscordBot.Main.GameObjects;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Main
{
    class GuessingGame
    {
        public Channel channel                          { get; private set; }
        private string botChannelName;
        private Channel botChannel = null;
        public User host                                { get; private set; }
        public bool running                             { get; private set; }
        public GuessingSong currentSong                 { get; private set; }
        private bool titleGuessed;
        private bool artistGuessed;
        private bool knownFromGuessed;
        public int songsLeft                            { get; private set; }

        public GuessingGame(CommandService commands)
        {
            botChannelName = "bot_only";
            running = false;

            commands.CreateCommand("guess")
                .Alias("gs")
                .Description("<stop | create> <amount of songs>\n\tSong guessing game")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await InitGame(e));
        }

        public async Task InitGame(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("param").Split(' ');
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("Specify <create | stop> おねがいします");
                return;
            }
            if(param[0].ToLower() == "setbotchannel")
            {
                if(param.Length <= 1)
                {
                    await channel.SendMessage("Channel not specified");
                    return;
                }
                botChannelName = param[1];
                await channel.SendMessage("Botchannel is now '" + botChannelName + "'");
                return;
                
            }
            if(param[0].ToLower() == "stop")
            {
                if (!running)
                {
                    await e.Channel.SendMessage("How does one stop a game that is not running?");
                    return;
                }
                if(e.User != host && e.User.Id != Constants.NYAid)
                {
                    await e.Channel.SendMessage("Shhh, only the game creator can stop the game!");
                    return;
                }
                await e.Channel.SendMessage("Stop command used");
                running = false;
                return;
            }

            if(param[0].ToLower() == "create")
            {
                if (running)
                {
                    await e.Channel.SendMessage("There is already a game running somewhere, sowwy");
                    return;
                }
                int num;
                if(param.Length <= 1 || !Int32.TryParse(param[1], out num))
                {
                    await e.Channel.SendMessage("Specify <create> <amount of songs (max 10)> to create a new game please!");
                    return;
                }
                songsLeft = num;
                if (songsLeft > 10) songsLeft = 10;
                await e.Channel.SendMessage("Game with " + songsLeft + " songs created!");
                channel = e.Channel;
                host = e.User;
                for (int i = 0; i < channel.Server.AllChannels.Count(); i++)
                {
                    if (channel.Server.AllChannels.ElementAt(i).Name.ToLower() == botChannelName)
                        botChannel = channel.Server.AllChannels.ElementAt(i);
                }
                if (botChannel == null)
                {
                    await channel.SendMessage("Textchannel " + botChannelName + " not found :/");
                    return;
                }
                running = true;
                await PlaySong();
                return;
            }
            await e.Channel.SendMessage("Something went wrong with your arguments XD");
        }

        public async Task Handle(MessageEventArgs e)
        {
            if (e.User.IsBot) return;
            if (e.Message.Text.Length <= 0) return;
            if (!artistGuessed && e.Message.Text == currentSong.artist.ToLower())
            {
                artistGuessed = true;
                await channel.SendMessage("Artist (" + currentSong.artist + ") guessed by " + e.User.Mention + "!");
            }
            if (!titleGuessed && e.Message.Text == currentSong.title.ToLower())
            {
                titleGuessed = true;
                await channel.SendMessage("Title (" + currentSong.title + ") guessed by " + e.User.Mention + "!");
            }
            if (currentSong.knownFrom != null && !knownFromGuessed && e.Message.Text == currentSong.knownFrom.ToLower())
            {
                knownFromGuessed = true;
                await channel.SendMessage("Knownfrom (" + currentSong.knownFrom + ") guessed by " + e.User.Mention + "!");
            }
            if(artistGuessed || titleGuessed || knownFromGuessed)
            {
                await PlaySong();
            }
        }

        private async Task PlaySong()
        {
            if(songsLeft <= 0 || !running)
            {
                await channel.SendMessage("Well played everyone, the game is now over!");
                running = false;
                songsLeft = 0;
                return;
            }
            currentSong = Constants.songs.ElementAt(MyBot.rng.Next(Constants.songs.Count()));
            await botChannel.SendMessage("Current song: " + currentSong.title);
            await botChannel.SendMessage("!play " + currentSong.link);
            titleGuessed = false;
            artistGuessed = false;
            knownFromGuessed = false;
            songsLeft--;
        }
    }
}
