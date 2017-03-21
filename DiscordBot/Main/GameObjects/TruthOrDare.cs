using Discord;
using Discord.Commands;
using DiscordBot.Main.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Main
{
    class TruthOrDare
    {
        public Channel channel { get; private set; }
        public User host { get; private set; }
        public bool running { get; private set; }
        public int gamesLeft { get; private set; }
        public List<int> playedT { get; private set; }
        public List<int> playedD { get; private set; }
        public List<User> players { get; private set; }
        public User lastPlayer { get; private set; }

        public TruthOrDare(CommandService commands)
        {
            running = false;
            gamesLeft = 0;

            commands.CreateCommand("tod")
                .Alias("truthordare")
                .Description("<create | stop | truth | dare | next>\n\tPlay a truth or dare game!")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await InitGame(e));
        }
        
        public async Task InitGame(Discord.Commands.CommandEventArgs e)
        {
            if(running && e.Channel != channel)
            {
                await e.Channel.SendMessage("A game is already being played in another channel :/");
                return;
            }
            var param = e.GetArg("param").Split(' ');
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("Specify <create | stop | truth | dare | next | add> おねがいします");
                return;
            }

            if (param[0].ToLower() == "stop")
            {
                if (!running)
                {
                    await e.Channel.SendMessage("How does one stop a game that is not running?");
                    return;
                }
                if (e.User != host && e.User.Id != Constants.NYAid)
                {
                    await e.Channel.SendMessage("Shhh, only the game creator can stop the game!");
                    return;
                }
                await e.Channel.SendMessage("The gamehost has stopped the ToD game!");
                running = false;
                return;
            }

            if (param[0].ToLower() == "create")
            {
                if (running)
                {
                    await e.Channel.SendMessage("There is already a game running somewhere, sowwy");
                    return;
                }
                int num;
                if (param.Length <= 1 || !Int32.TryParse(param[1], out num))
                {
                    await e.Channel.SendMessage("Specify <create> <amount of games (max 15)> <list of players (mentioned)> to create a new game please!");
                    return;
                }
                gamesLeft = num;
                if (gamesLeft > 15) gamesLeft = 15;
                players = e.Message.MentionedUsers.ToList<User>();
                await e.Channel.SendMessage("We will play " + gamesLeft + " times, with " + players.Count() + " players!");
                channel = e.Channel;
                host = e.User;
                playedD = new List<int>();
                playedT = new List<int>();
                running = true;
                await NextRound();
                return;
            }

            if (param[0].ToLower() == "truth" || param[0].ToLower() == "dare" || param[0].ToLower() == "next")
            {
                if (!running)
                {
                    await e.Channel.SendMessage("There is no game being played right now, sweetheart");
                    return;
                }
                if (e.User != lastPlayer && e.User != host)
                {
                    await e.Channel.SendMessage("You are not the chosen one, baka");
                    return;
                }
                if (param[0].ToLower() == "next")
                {
                    await NextRound();
                    return;
                }
                if (e.User == host && e.User != lastPlayer)
                    return;
                int i;
                if(param[0].ToLower() == "truth")
                {
                    do
                    {
                        i = MyBot.rng.Next(Constants.truth.Count());
                        if (playedT.Count() >= Constants.truth.Count()) playedT = new List<int>();
                    } while (playedT.Contains(i));
                    await channel.SendMessage(Constants.truth.ElementAt(i));
                    playedT.Add(i);
                    return;
                }
                do
                {
                    i = MyBot.rng.Next(Constants.dare.Count());
                    if (playedD.Count() >= Constants.dare.Count()) playedD = new List<int>();
                } while (playedD.Contains(i));
                playedD.Add(i);
                await channel.SendMessage(Constants.dare.ElementAt(i));
                return;
            }

            if (param[0].ToLower() == "add")
            {
                if(e.Message.MentionedUsers.Count() <= 0)
                {
                    await e.Channel.SendMessage("You will have to mention the players you want to add!!");
                    return;
                }
                players.AddRange(e.Message.MentionedUsers);
                await channel.SendMessage("The game now has " + players.Count() + " players!\n" + gamesLeft + " rounds to go!");
                return;
            }
            await e.Channel.SendMessage("Something went wrong with your arguments XD");
        }

        private async Task NextRound()
        {
            if (gamesLeft <= 0 || !running)
            {
                await channel.SendMessage("Well played everyone, the game is now over!");
                running = false;
                gamesLeft = 0;
                return;
            }
            User player;
            do
            {
                player = players.ElementAt(MyBot.rng.Next(players.Count()));
            } while (lastPlayer != null && players.Count() > 1 && lastPlayer == player);
            lastPlayer = player;
            await channel.SendMessage("Next person is... " + player.Mention + "!\nTruth or dare??\t\t\t('>tod truth' or '>tod dare')");
            gamesLeft--;
        }
    }
}
