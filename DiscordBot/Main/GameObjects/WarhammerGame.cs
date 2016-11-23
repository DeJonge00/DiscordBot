using Discord;
using Discord.Commands;
using Discord.Legacy;
using DiscordBot.Main.GameObjects.WarhammerRPG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBot.Main.GameObjects
{
    class WarhammerGame
    {
        private CommandService commands;
        private Game game;
        private RPGShop shop;
        private bool running;
        private int counter;
        private Thread runningThread;

        public WarhammerGame(CommandService c, Game g)
        {
            commands = c;
            game = g;
            shop = new RPGShop(game);
            running = false;

            commands.CreateCommand("rpg")
                .Description("Work in progress")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await HandleCommand(e));
        }

        private async Task HandleCommand(Discord.Commands.CommandEventArgs e)
        {
            var param = e.GetArg("param").Split(' ');
            await e.Message.Delete();
            if (param.Length <= 0)
            {
                await GameHelp(e);
                return;
            }
            if (param[0].ToLower() == "start" && e.User.Id == Constants.NYAid)
            {
                runningThread = new Thread(new ThreadStart(StartGame));
                runningThread.Start();
                return;
            }
            if (param[0].ToLower() == "shop")
            {
                await shop.ShopInfo(e);
                return;
            }
            if (param[0].ToLower() == "buy")
            {
                await shop.Buy(e);
                return;
            }
            string[] farm = { "patrol", "farm" };
            if (farm.Contains(param[0].ToLower()))
            {
                int amount;
                var user = game.GetUser(e.User.Id, e.User.Name);
                if(param.Length <= 1)
                {
                    amount = 5;
                } else
                {
                    if (!Int32.TryParse(param[1], out amount))
                        amount = 5;
                }
                user.patrolling = amount;
                await e.User.SendMessage("You will be patrolling for the next " + amount + " minutes");
                return;
            }
            if (param[0].ToLower() == "battle")
            {
                if (e.Message.MentionedUsers.Count() <= 0)
                {
                    await e.User.SendMessage("Mention a user to annihalate");
                    return;
                }
                await RPGEvent.Battle(e.Channel, game.GetUser(e.User.Id, e.User.Name), game.GetUser(e.Message.MentionedUsers.ElementAt(0).Id, e.Message.MentionedUsers.ElementAt(0).Name));
                return;
            }
            if(param.Length >= 8 && e.Message.MentionedUsers.Count() >= 1 && param[0] == "addStats" && e.User.Id == Constants.NYAid)
            {
                Console.WriteLine("RPG Addstats command used");
                var user = game.GetUser(e.Message.MentionedUsers.ElementAt(0).Id, e.Message.MentionedUsers.ElementAt(0).Name);
                int exp, points, health, ws, s, t;
                if (Int32.TryParse(param[2], out exp) && Int32.TryParse(param[3], out points) && Int32.TryParse(param[4], out health) && Int32.TryParse(param[5], out ws) && Int32.TryParse(param[6], out s) && Int32.TryParse(param[7], out t))
                {
                    user.AddExp(exp);
                    user.AddPoints(points);
                    user.IncreaseHealth(health);
                    user.IncreaseWeaponskill(ws);
                    user.IncreaseStrength(s);
                    user.IncreaseToughness(t);
                }
            }
            await e.Channel.SendMessage("Words like '" + param[0] + "' are not known in the tongue of real warriors");
        }

        private async Task GameHelp(Discord.Commands.CommandEventArgs e)
        {
            var helpMessage = "**A Message From Above**\n```" 
                + ">stats\n\tSee your warrior's combat ability"
                + "\n>game\n\tGame help"
                + "\n>game shop\n\tSee shop inventory"
                + "```";
            await e.User.SendMessage(helpMessage);
        }

        private async void StartGame()
        {
            counter = 0;
            running = true;
            DateTime startTime = new DateTime(2010, 05, 12, 13, 15, 00);
            DateTime endTime = new DateTime(2010, 05, 12, 13, 45, 00);
            var gameChannel = game.client.FindServers("Sexual station").FirstOrDefault().FindChannels("bot_spam", ChannelType.Text, false).FirstOrDefault();
            while (running)
            {
                if (counter % (10*60) == 0)
                    foreach (PointDB player in game.players)
                    {
                        if (player.health < player.maxHealth)
                            player.IncreaseHealth(player.maxHealth / 10);
                    }
                if (counter % (5*60) == 0)
                    foreach (PointDB player in game.players)
                    {
                        if (player.patrolling > 0 && player.cooldown <= 0)
                            await RPGEvent.Patrol(gameChannel, player);
                    }

                foreach (PointDB player in game.players)
                {
                    if(player.cooldown > 0) player.cooldown--;
                    if(player.patrolling > 0) player.patrolling--;
                }

                startTime = System.DateTime.Now;
                endTime = System.DateTime.Now;
                int sleeptimer = (int)(1000 + endTime.Subtract(startTime).Milliseconds);
                if (sleeptimer > 0)
                    System.Threading.Thread.Sleep(sleeptimer);
                counter++;
            }
        }

        internal void abort()
        {
            running = false;
        }
    }
}
