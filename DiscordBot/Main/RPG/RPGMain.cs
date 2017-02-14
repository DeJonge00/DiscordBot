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
        private Thread thread;
        private Boolean running;
        private string serverName;
        private string channelName;
        private Channel rpgchannel;
        public static string filename = "rpggame";

        private List<RPGPlayer> players;
        private List<RPGPlayer> farmers;
        private List<RPGPlayer> bossFightPlayers;

        public RPGMain(CommandService c, DiscordClient dc)
        {
            commands = c;
            client = dc;

            // RPG game commands
            commands.CreateCommand("rpg")
                .Description("\n\tBasic commands of the rpg")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await BasicRPG(e));

            commands.CreateCommand("rpgbattle")
                .Description("<@person>\n\tBattle a fellow chatwarrior to a lethal battle!")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await PlayerBattle(e));

            commands.CreateCommand("rpgfarm")
                .Description("<amount>\n\tFarm for the next <amount> minutes (no talking income)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Farm(e));

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
                .Description("\n\tReset game progress (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Reset(e));

            rpgshop = new RPGShop(commands, this);
            Start();
        }

        private void Start()
        {
            serverName = "test";
            channelName = "rpg";
            LoadPlayers();
            bossFightPlayers = new List<RPGPlayer>();
            farmers = new List<RPGPlayer>();

            // Start game thread
            thread = new Thread(new ThreadStart(ThreadLoop));
            thread.Start();
        }

        // Triggers events
        private void ThreadLoop()
        {
            running = true;
            while (running)
            {
                var time = DateTime.Now;
                foreach(RPGPlayer p in farmers)
                {
                    p.DecrementFarming();
                    if(!p.IsFarming())
                    {
                        farmers.Remove(p);
                    }
                }
                if(time.Minute % 10 == 0)
                {
                    foreach(RPGPlayer p in players)
                    {
                        if(p.health < p.maxHealth)
                        {
                            p.AddHealth(10);
                        }
                    }
                }
                if(time.Minute % 18 == 0)
                {
                    FarmingEncounter();
                } 
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

        private async Task BasicRPG(Discord.Commands.CommandEventArgs e)
        {
            var param = e.GetArg("param").Split(' ');
            if(param.Count()<=0)
            {
                await Help(e);
                return;
            }
            switch(param[0])
            {
                case "help":
                    await Help(e);
                    break;
                case "class":
                    bool success;
                    if(param.Count() <= 1)
                    {
                        success = GetPlayerData(e.User).SetClass("Peasant");
                    } else
                    {
                        success = GetPlayerData(e.User).SetClass(param[1]);
                    }
                    if(success)
                    {
                        await e.Channel.SendMessage("You now have a new class! Good luck fighting!");
                    } else
                    {
                        await e.Channel.SendMessage("Hmm, do not go making up classes pls...");
                    }
                    break;
                default:
                    await Help(e);
                    break;
            }
        }

        private void Battle(Channel channel, RPGPlayer p, RPGMonster m)
        {
            rpgchannel.SendMessage("Battle between **" + p.name + "** and a monster!\nNothing happened :/");
        }

        private void Battle(Channel channel, RPGPlayer p1, RPGPlayer p2)
        {
            var battlereport = "Battle between **" + p1.name + "** (" + p1.health + ") and **" + p2.name + "** (" + p2.health + ")!\n";
            RPGPlayer p3;
            for (int i = 0; (p1.health > 0 && p2.health > 0) && i < 30; i++)
            {
                var ws = MyBot.rng.Next((int)(p1.weaponskill + p2.weaponskill));
                if (ws < p1.weaponskill)
                {
                    int damage = (int)((MyBot.rng.Next(100, 200) * p1.damage)/100);
                    p2.AddHealth(-damage);
                    battlereport += "\n**" + p1.name + "** attacked for **" + damage + "**";
                }
                p3 = p1;
                p1 = p2;
                p2 = p3;
                //Console.WriteLine(p1.name + ": " + p1.health + " | " + p2.name + " : " + p2.health);
            }
            if(p1.health <= 0)
            {
                battlereport += "\n\nThe battle is over, **" + p2.name + "** (" + p2.health + ") laughs while walking away from **" + p1.name + "**'s corpse";
            } else
            {
                battlereport += "\n\nThe battle lasted long, both players are exhausted. They agree on a draw *this time*\n"
                    + "Healthreport: **" + p1.name + "** (" + p1.health + "), **" + p2.name + "** (" + p2.health + ")";
            }
            channel.SendMessage(battlereport);
        }

        private void Bossfight()
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
            var boss = new RPGMonster(blevel*100, blevel*10, blevel*15, blevel*5);
            if (bossFightPlayers.Count() <= 0)
            {
                MyBot.Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") Bossfight cancelled, noone showed up", "rpggame");
                return;
            }

            // Resolve boss battle
            rpgchannel.SendMessage("BOSSFIGHT!!\n*Wait wut... Not even implemented? smh*");
            MyBot.Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") Bossfight!! " + bossFightPlayers.Count() + " warriors ready", filename);
            bossFightPlayers = new List<RPGPlayer>();
        }
        
        private async Task Farm(Discord.Commands.CommandEventArgs e)
        {
            var player = GetPlayerData(e.User);
            if(farmers.Contains(player))
            {
                await e.Channel.SendMessage("You are already farming! " + player.farmingLeft + " minutes left!");
                return;
            }
            var param = e.GetArg("param");
            int minutes = 5;
            if(param.Length > 0)
            {
                Int32.TryParse(param, out minutes);
            }
            if(player.SetFarming(minutes))
            {
                farmers.Add(player);
                await e.Channel.SendMessage("You are now farming for " + minutes + " minutes!");
                return;
            }
            await e.Channel.SendMessage("Maybe check your parameters again?");
        }

        private void FarmingEncounter()
        {
            foreach(RPGPlayer p in farmers)
            {
                var mlevel = Math.Max(1, p.GetLevel()+(MyBot.rng.Next(10)-5));
                var monster = new RPGMonster(mlevel*20, mlevel*8, mlevel*10, mlevel*5);
                Battle(rpgchannel, p, monster);
            }
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

        public async Task Handle(MessageEventArgs e)
        {
            var data = GetPlayerData(e.User);
            var i = (int)Math.Round(Math.Pow(data.GetLevel(), 0.25)*Math.Max(5, Math.Min(50, (e.Message.Text.Length/2)-7)));
            data.AddExp(i);
            data.AddMoney(i);
        }

        private async Task Help(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var mess = "**All you need to know about this game!**\n"
                + ">rpgstats\n\tShow your stats (health and level and more)\n"
                + ">rpgtop <amount>\n\tShow the top <amount> players of this game (max 9)\n"
                + ">rpgjbf\n\tJoin the next bossfight (Bossfights are every hour, on the hour!)\n"
                + ">rpgshop items\n\tShows a list of items availeble in the shop\n"
                + ">rpgshop buy <item> <amount>\n\tBuy the specified item, amount times (if you can afford it)";
            await e.Channel.SendMessage(mess);
        }

        private async Task JoinBossFight(Discord.Commands.CommandEventArgs e)
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

        private void LoadPlayers()
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

        private async Task PlayerBattle(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var player = GetPlayerData(e.User);
            if (player.IsFarming())
            {
                await e.Channel.SendMessage("You are really busy with farming at the moment... (" + player.farmingLeft + " minutes left)");
                return;
            }
            if (player.health <= 0)
            {
                await e.Channel.SendMessage("Lol, you are dead! How would you even fight in that state??");
                return;
            }
            if (e.Message.MentionedUsers.Count() <= 0 || e.Message.MentionedUsers.ElementAt(0).Name == e.User.Name)
            {
                await e.Channel.SendMessage("Sooo, who are you gonna battle??");
                return;
            }
            var enemy = GetPlayerData(e.Message.MentionedUsers.ElementAt(0));
            if (enemy.health <= 0)
            {
                await e.Channel.SendMessage("Your enemy is currently busy with being dead, try again later");
                return;
            }
            if (enemy.IsFarming())
            {
                await e.Channel.SendMessage("Your enemy is really busy with farming at the moment... (" + player.farmingLeft + " minutes left)");
                return;
            }
            Battle(e.Channel, player, enemy);
        }

        private async Task Reset(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id != Constants.NYAid)
            {
                await e.Channel.SendMessage("Hahahaha, no.");
                return;
            }
            var param = e.GetArg("param");
            if (param.Length <= 0)
            {
                return;
            }
            switch(param)
            {
                case "all":
                    players = new List<RPGPlayer>();
                    break;
            }
        }

        private void SavePlayers(List<RPGPlayer> p)
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

        private async Task Stats(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            RPGPlayer player;
            var param = e.GetArg("param");
            if(e.Message.MentionedUsers.Count() > 0)
            {
                player = GetPlayerData(e.Message.MentionedUsers.ElementAt(0));
            } else if(param.Length > 0)
            {
                player = GetPlayerData(e.User);
                foreach (RPGPlayer p in players)
                {
                    if(p.name.ToLower() == param.ToLower())
                    {
                        player = p;
                        break;
                    }
                }
            } else 
            {
                player = GetPlayerData(e.User);
            }
            var mess = "Stats for **" + player.name + "**\n```"
                +   "Level:      \t" + player.GetLevel()
                + "\nExp:        \t" + player.exp
                + "\nClass:      \t" + player.playerclass
                + "\nMoney:      \t" + player.money
                + "\nHealth:     \t" + player.health + " / " + player.maxHealth
                + "\nArmor:      \t" + player.armor
                + "\nDamage:     \t" + player.damage
                + "\nWeaponskill:\t" + player.weaponskill
                + "```";
            await e.Channel.SendMessage(mess);
        }

        private async Task Top(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            List<RPGPlayer> sortedplayers = players.OrderBy(o => o.exp).Reverse().ToList();
            var mess = "**RPG Top 5 players**\n```";
            Console.WriteLine(sortedplayers.Count());
            var num = 5;
            if(e.GetArg("param").Length > 0)
            {
                Int32.TryParse(e.GetArg("param"), out num);
                if (num > 9) num = 9;
            }
            for(int i = 0; i < num && i < sortedplayers.Count(); i++)
            {
                mess += "\n" + (i+1) + ".\t" + sortedplayers[i].name + " : " + sortedplayers[i].exp + "xp (lvl: " + sortedplayers[i].GetLevel() + ")";
            }
            mess += "```";
            await e.Channel.SendMessage(mess);
        }
    }
}
