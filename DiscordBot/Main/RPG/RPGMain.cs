using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    class RPGMain
    {
        private CommandService commands;
        private DiscordClient client;
        private List<RPGPlayer> players;
        private string statsFile = Path.Combine(@"F:\DiscordBot\stats", "rpgstats.bin");
        Thread thread;
        Boolean running;

        public RPGMain(CommandService c, DiscordClient dc)
        {
            commands = c;
            client = dc;

            LoadPlayers();

            // RPG game commands
            commands.CreateCommand("rpgtop")
                .Description("\n\tGet the top scoring players in the game")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Top(e));

            // Start game thread
            thread = new Thread(new ThreadStart(ThreadLoop));
            thread.Start();
        }

        public void ThreadLoop()
        {
            running = true;
            while (running)
            {
                var time = DateTime.Now;

                if (time.Minute % 15 == 0)
                {
                    SavePlayers(players);
                }

                var sleepTime = (60 * 1000) - time.Millisecond;
                //Console.WriteLine(sleepTime);
                System.Threading.Thread.Sleep(sleepTime);
            }
        }

        public void Abort()
        {
            running = false;
            SavePlayers(players);
            thread.Abort();
        }

        public RPGPlayer GetPlayerData(Discord.User u)
        {
            foreach(RPGPlayer p in players)
            {
                if(p.id == u.Id)
                {
                    p.UpdateName(u.Nickname);
                    return p;
                }
            }
            return new RPGPlayer(u);
        }

        public void Handle(MessageEventArgs e)
        {
            var data = GetPlayerData(e.User);
            data.AddExp(10);
        }

        public void LoadPlayers()
        {
            try
            {
                using (Stream stream = File.Open(statsFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    players = (List<RPGPlayer>)bformatter.Deserialize(stream);
                }
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") LOADED RPG GAME STATS");
            }
            catch (Exception e)
            {
                players = new List<RPGPlayer>();
                statsFile = Path.Combine(@"F:\DiscordBot\stats", "rpgstats_backup.bin");
            }
        }

        public void SavePlayers(List<RPGPlayer> p)
        {
            try
            {
                //serialize
                using (Stream stream = File.Open(statsFile, FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bformatter.Serialize(stream, players);
                }
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") SAVED RPG GAME STATS");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in saving");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async Task Top(Discord.Commands.CommandEventArgs e)
        {
            players.Sort();
            var mess = "**RPG Top 5 players**\n```";
            for(int i = 1; i <= 5 && i <= players.Count(); i++)
            {
                mess += "\n" + i + ".\t" + players.ElementAt(0).name;
            }
            mess += "```";
            await e.Channel.SendMessage(mess);
        }
    }
}
