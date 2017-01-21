using Discord;
using Discord.Commands;
using DiscordBot.Main.GameObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace DiscordBot.Main
{
    class Game
    {
        // Fields
        private Boolean running;
        public List<PointDB> players;
        private string statsFile = Path.Combine(@"F:\DiscordBot\stats", "stats.bin");
        private CommandService commands;
        private Random rng;
        private Thread runningThread;
        public DiscordClient client;
        // Games
        public GuessingGame guessingGame        { get; private set; }
        public RPSGame rpsGame                  { get; private set; }
        public QuizGame quizGame                { get; private set; }
        public TruthOrDare todGame              { get; private set; }

        // Constructor
        public Game(CommandService c, DiscordClient dc)
        {
            client = dc;
            commands = c;
            running = true;
            rng = new Random();
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
            commands.CreateCommand("stats")
               .Description(" <user>\n\tShow user's stats (yours if none specified)")
               .Parameter("null", ParameterType.Unparsed)
               .Do(async (e) => await Stats(e));

            // Mod command list
            commands.CreateCommand("resetGameData")
               .Description("\n\tReset player points database (mod)")
               .Parameter("null", ParameterType.Unparsed)
               .Do(async (e) => await Reset(e));

            commands.CreateCommand("save")
                .Description("\n\tSave points (mod)")
                .Parameter("null", ParameterType.Unparsed)
                .Do((e) => Save(e.User.Id));

            commands.CreateCommand("top")
                .Description("\n\tTop players (sorted on points)")
                .Parameter("amount", ParameterType.Unparsed)
                .Do(async (e) => await Top(e));
            
            // Init games
            guessingGame = new GuessingGame(commands, this);
            rpsGame = new RPSGame(commands, this);
            quizGame = new QuizGame(commands, this);
            todGame = new TruthOrDare(commands, this);

            // Start game thread
            runningThread = new Thread(new ThreadStart(StartGame));
            runningThread.Start();
        }

        // List of commands functions
        public async Task Reset(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                players = new List<PointDB>();
            }
            else
            {
                await e.Channel.SendMessage("Hahahaha, no.");
            }
        }

        public void Save(ulong id)
        {
            if (id == Constants.NYAid || id == 0)
            {
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") SAVING GAME STATS");
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
        }

        private async Task Stats(Discord.Commands.CommandEventArgs e)
        {
            var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + ": " + e.Message.Text;
            File.AppendAllText(@"F:\DiscordBot\log\log.txt", str + Environment.NewLine);
            Console.WriteLine(str);
            if (e.Message.MentionedUsers.Count() > 0)
                for (int i = 0; i < e.Message.MentionedUsers.Count(); i++)
                    await ShowStats(e.Channel, e.Message.MentionedUsers.ElementAt(i));
            else await ShowStats(e.Channel, e.User);
        }

        private async Task ShowStats(Channel c, User user)
        {
            PointDB data = null;
            for (int i = 0; i < players.Count(); i++)
                if (players.ElementAt(i).id == user.Id)
                    data = players.ElementAt(i);
            if (data == null)
                players.Add(data = new PointDB(user.Id, user.Name));
            var str = "Stats for " + user.Name +
                "```\nLevel:\t\t\t\t " + data.level +
                "\nBattle experience:\t " + data.exp + " / " + data.maxExp +
                "\nValuable metals:\t   " + data.points +
                "\nHealth:\t\t\t\t" + data.health + " / " + data.maxHealth +
                "\nWeapon skill:\t\t  " + data.weaponSkill + 
                "\nStrength:\t\t\t  " + data.strength + 
                "\nToughness:\t\t\t " + data.toughness +
                //"\nAttack speed:\n\t" + data.attackSpeed +
                "```";
            await c.SendMessage(str);
        }

        public async Task Top(Discord.Commands.CommandEventArgs e)
        {
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
            if (amount > players.Count() || amount > 9)
            {
                amount = Math.Min(players.Count(), 9);
            }
            string s = "```Top " + amount + "\n";
            for (int i = 0; i < amount; i++)
            {
                s += (i + 1) + ") lvl: " + SortedList[i].level + " " + SortedList[i].name + ": " + SortedList[i].points + "\n";
            }
            s += "```";
            await e.Channel.SendMessage(s);
        }

        private void StartGame()
        {
            while (running)
            {
                Save(0);
                System.Threading.Thread.Sleep(15*60*1000);
            }
        }

        public void Abort()
        {
            this.running = false;
            runningThread.Abort();
        }

        public PointDB GetUser(ulong id, string name)
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

        public void Quit()
        {
            Abort();
            Save(0);
        }
    }
}
