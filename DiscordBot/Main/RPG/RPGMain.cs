using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    class RPGMain
    {
        private CommandService commands;
        private DiscordClient client;
        private RPGShop rpgshop;
        private List<RPGPlayer> players;
        private Thread thread;
        private Boolean running;
        private string serverName;
        private string channelName;
        private Channel rpgchannel;
        private List<RPGPlayer> bossFightPlayers;

        public RPGMain(CommandService c, DiscordClient dc)
        {
            commands = c;
            client = dc;

            // RPG game commands
            commands.CreateCommand("rpg")
                .Alias("rpghelp")
                .Description("\n\tGet help from the rpg game master (Me lol)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Help(e));

            commands.CreateCommand("rpgstats")
                .Description("\n\tSee your rpg stats")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Stats(e));

            commands.CreateCommand("rpgtop")
                .Description("\n\tGet the top scoring players in the game")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Top(e));

            commands.CreateCommand("rpgjoinbossfight")
                .Alias("rpgjbf")
                .Description("\n\tJoin the next boss fight (>rpgjbf for short ;) )")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await JoinBossFight(e));

            // Mod commands
            commands.CreateCommand("rpgreset")
                .Description("\n\tReset game progress")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Reset(e));

            rpgshop = new RPGShop(commands, client);
            Start();
        }

        public void Start()
        {
            serverName = "test";
            channelName = "rpg";
            LoadPlayers();
            bossFightPlayers = new List<RPGPlayer>();

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
                if (time.Minute % 60 == 0)
                {
                    Bossfight();
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

        public void Bossfight()
        {
            try
            {
                rpgchannel = client.FindServers(serverName).First().FindChannels(channelName).FirstOrDefault();
            } catch
            {
                Console.WriteLine("Finding rpg channel failed");
                return;
            }
            var blevel = 1;
            var boss = new RPGBoss(blevel, blevel*50, blevel*10, blevel*10);
            if (bossFightPlayers.Count() <= 0)
            {
                MyBot.Log(DateTime.Now.ToUniversalTime().ToShortDateString() + ") Bossfight cancelled, noone showed up", "rpggame");
                return;
            }

            // Resolve boss battle
            rpgchannel.SendMessage("BOSSFIGHT!!\n*Wait wut... Not even implemented? smh*");
            MyBot.Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") Bossfight!! " + bossFightPlayers.Count() + " warriors ready", "rpggame");
            bossFightPlayers = new List<RPGPlayer>();
        }

        public RPGPlayer GetPlayerData(Discord.User u)
        {
            foreach(RPGPlayer p in players)
            {
                if(p.id == u.Id)
                {
                    p.UpdateName(u.Name);
                    return p;
                }
            }
            RPGPlayer player = new RPGPlayer(u);
            players.Add(player);
            return player;
        }

        public void Handle(MessageEventArgs e)
        {
            var data = GetPlayerData(e.User);
            data.AddExp(10);
        }

        private async Task Help(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var mess = "**All you need to know about this game!**\n"
                + ">rpgstats                    \tShow your stats (health and level and more)\n"
                + ">rpgtop <amount>             \tShow the top <amount> players of this game\n"
                + ">rpgjbf                      \tJoin the next bossfight (Bossfights are every hour, on the hour!)\n"
                + ">rpgshop items               \tShows a list of items availeble in the shop\n"
                + ">rpgshop buy <item> <amount> \tBuy the specified item, amount times (if you can afford it)";
            await e.Channel.SendMessage(mess);
        }

        public async Task JoinBossFight(Discord.Commands.CommandEventArgs e)
        {
            var player = GetPlayerData(e.User);
            bossFightPlayers.Add(player);
            var a = bossFightPlayers.Count();
            if(a == 1)
            {
                await e.Channel.SendMessage("Player **" + e.User.Name + "** added to bossfight\n1 player joined so far!");
            } else
            {
                await e.Channel.SendMessage("Player **" + e.User.Name + "** added to bossfight\n" + a + " players joined so far!");
            }
        }

        public void LoadPlayers()
        {
            try
            {
                using (Stream stream = File.Open(Constants.rpgStatsFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    players = (List<RPGPlayer>)bformatter.Deserialize(stream);
                }
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") LOADED RPG GAME STATS");
            }
            catch (Exception e)
            {
                players = new List<RPGPlayer>();
            }
        }

        public async Task Reset(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                players = new List<RPGPlayer>();
            }
            else
            {
                await e.Channel.SendMessage("Hahahaha, no.");
            }
        }

        public void SavePlayers(List<RPGPlayer> p)
        {
            try
            {
                //serialize
                using (Stream stream = File.Open(Constants.rpgStatsFile, FileMode.Create))
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

        public async Task Stats(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var player = GetPlayerData(e.User);
            var mess = "Stats for **" + e.User.Name + "**\n```"
                +   "Level:\t " + player.GetLevel()
                + "\nExp:\t   " + player.exp
                + "\nClass:\t " + player.playerclass
                + "\nHealth:\t" + player.health + " / " + player.maxHealth
                + "\nArmor:\t " + player.armor
                + "\nDamage:\t" + player.damage
                + "```";
            await e.Channel.SendMessage(mess);
        }

        public async Task Top(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            List<RPGPlayer> sortedplayers = players.OrderBy(o => o.exp).Reverse().ToList();
            var mess = "**RPG Top 5 players**\n```";
            Console.WriteLine(sortedplayers.Count());
            for(int i = 0; i < 5 && i < sortedplayers.Count(); i++)
            {
                mess += "\n" + (i+1) + ".\t" + sortedplayers[i].name + " : " + sortedplayers[i].exp + "xp";
            }
            mess += "```";
            await e.Channel.SendMessage(mess);
        }
    }
}
