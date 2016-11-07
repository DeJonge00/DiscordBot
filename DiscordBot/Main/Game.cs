using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DiscordBot.Main
{
    class Game
    {
        // Fields
        private Boolean running;
        List<PointDB> players;
        private string statsFile = Path.Combine(@"F:\DiscordBot\stats", "stats.bin");
        private CommandService commands;
        private Random rng;

        // Constructor

        public Game(CommandService commands)
        {
            this.commands = commands;
            this.running = true;
            this.rng = new Random();
            try
            {
                using (Stream stream = File.Open(statsFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    players = (List<PointDB>)bformatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            if (players == null) players = new List<PointDB>();

            // Command list
            commands.CreateCommand("resetGameData")
               .Description("Reset player points database (dev stuff)")
               .Do(async (e) => await reset(e));

            commands.CreateCommand("save")
                .Description("Save points")
                .Do((e) => save());

            commands.CreateCommand("top")
                .Description("Top players (sorted on points)")
                .Parameter("amount", ParameterType.Unparsed)
                .Do(async (e) => await top(e));
        }

        // List of commands functions
        public async Task reset(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            Console.WriteLine("Reset command used by " + e.User);
            if (e.User.Id == Constants.NYAid)
            {
                players = new List<PointDB>();
            }
            else
            {
                await e.Channel.SendMessage("Hahahaha, no.");
            }
        }

        public void save()
        {
            Console.WriteLine("SAVING STATS");
            try
            {
                //serialize
                using (Stream stream = File.Open(statsFile, FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bformatter.Serialize(stream, players);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Error in saving");
            }
        }

        public async Task top(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Top command used by " + e.User);
            await e.Message.Delete();
            if (!running)
            {
                await e.Channel.SendMessage("The game is not even running :/");
                return;
            }
            int amount;
            string param = e.GetArg("amount").Split(' ')[0];
            List<PointDB> SortedList = players.OrderBy(o => o.points).Reverse().ToList();
            if (!(Int32.TryParse(param, out amount))) amount = players.Count();
            if (amount > players.Count() || amount > 10)
            {
                amount = Math.Min(players.Count(), 10);
            }
            string s = "Top " + amount + "\n";
            for (int i = 0; i < amount; i++)
            {
                s += (i + 1) + ") " + SortedList[i].name + ": " + SortedList[i].points + "\n";
            }
            await e.Channel.SendMessage(s);
        }

        // Start game in specific channel
        public async void startGame(Discord.Channel channel, int interval)
        {
            int timer = interval;
            Console.WriteLine("startGame()");
            Message[] messages = await channel.DownloadMessages(1);
            var oldMessage = messages[0];

            while (running)
            {
                if (timer <= 0)
                {
                    save();
                    timer = 5000;
                } else { timer--; }
                messages = await channel.DownloadMessages(1);
                var message = messages[0];
                if(message == oldMessage)
                {
                    System.Threading.Thread.Sleep(interval);
                }
                else
                {
                    oldMessage = message;
                    if (!(message.Text.Length <= 0 || message.Text.First() == '>' || message.User.Id == Constants.BIRIBIRIid))
                    {
                        var points = 1;
                        var player = getUser(message.User.Id, message.User.Name);

                        if(message.Text == "ded" || message.Text == "*ded*")
                        {
                            points = 0;
                            await message.Channel.SendMessage(Responses.ded[rng.Next(Responses.ded.Length)]);
                        }
                        if(message.Text == "kys" || message.Text.ToLower() == "kill yourself")
                        {
                            points = 0;
                            await message.Channel.SendMessage(Responses.kys[rng.Next(Responses.kys.Length)]);
                        }
                        if(message.Text.Split()[0].ToLower() == "hello" || message.Text.Split()[0].ToLower() == "ola")
                        {
                            var s = "Hello to you too!";
                            if (message.User.Id == Constants.NYAid) s += " <3";
                            await message.Channel.SendMessage(s);
                        }
                        player.points += points;
                    }

                    if(message.User.Id == Constants.WIZZid)
                    {
                        if (rng.Next(300) <= 1) await message.Channel.SendMessage("<3");
                    }
                }
            }
        }

        private PointDB getUser(ulong id, string name)
        {
            // Check if user is in the DB
            PointDB player = null;
            for (int i = 0; i < players.Count; i++)
            {
                if (id == players[i].id)
                {
                    player = players[i];
                }
            }
            // Create new entry in database
            if (player == null)
            {
                player = new PointDB(id, name);
                players.Add(player);
            }
            return player;
        }

        public void abort()
        {
            this.running = false;
        }
    }
}
