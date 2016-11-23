using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;
using DiscordBot.Main.Music;

namespace DiscordBot.Main
{
    class MyBot
    {
        // Fields
        CommandService commands;
        //private MusicHandler musicHandler;
        public Channel gameChannel;

        public static DiscordClient discordClient       { get; private set; }
        public static Random rng = new Random();
        public MessageHandler handler                   { get; private set; }
        public Game game                                { get; private set; }
        public MusicHandler music                       { get; private set; }

        // Constructor
        public MyBot()
        {
            rng = new Random();
            discordClient = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discordClient.UsingCommands(x =>
            {
                x.PrefixChar = '>';
                x.AllowMentionPrefix = false;
                x.HelpMode = HelpMode.Disabled;
            });

            commands = discordClient.GetService<CommandService>();

            // List of commands
            commands.CreateCommand("biribiri")
                .Description("\n\tPrint a pic of the one true Biribiri")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async (e) => await Biribiri(e));

            commands.CreateCommand("bye")
                .Description("<user> \n\tSay goodbye to someone")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await Bye(e));

            commands.CreateCommand("cat")
                .Description("\n\tRandom cat pic!!!")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async (e) => await Cat(e));

            commands.CreateCommand("censored")
                .Description("<message>\n\tCensor your own words")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) => await Cencored(e));

            commands.CreateCommand("choose")
                .Description("<option1> {<;option2>}\n\tLet Biribiri choose one from your options")
                .Parameter("arguments", ParameterType.Unparsed)
                .Do(async (e) => await Choose(e));

            commands.CreateCommand("clean")
                .Description("\n\tRemove messages of Biribiri")
                .Parameter("amount", ParameterType.Unparsed)
                .Do(async (e) => await Clean(e));

            commands.CreateCommand("coinflip")
                .Description("<heads | tails>\n\tFlip a coin with a slight chance of railgun")
                .Parameter("choice", ParameterType.Unparsed)
                .Do(async (e) => await Coinflip(e));

            commands.CreateCommand("compliment")
                .Description("<user>\n\tGive someone a compliment")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await Compliment(e));

            commands.CreateCommand("del")
                .Alias("delete")
                .Description("<message>\n\tDelete your message after a sec")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Delete(e));

            commands.CreateCommand("echo")
                .Description("<message>\n\tEcho the message")
                .Parameter("toEcho", ParameterType.Unparsed)
                .Do(async (e) => await Echo(e));

            commands.CreateCommand("face")
                .Description("<words to prefix>\n\tSay (๑･̑◡･̑๑)")
                .Parameter("words", ParameterType.Unparsed)
                .Do(async (e) => await Face(e));

            commands.CreateCommand("help")
                .Description("\n\tHelp via direct messages")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async (e) => await Help(e));

            commands.CreateCommand("hug")
                .Description("<user>\n\tHug a user")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await Hug(e));

            commands.CreateCommand("karma")
                .Description("<user>\n\tCheck your biri karma")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await Karma(e));

            commands.CreateCommand("kill")
                .Description("<user>\n\tThreathen someone")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await Kill(e));

            commands.CreateCommand("kiss")
                .Description("<user>\n\tKiss someone")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await Kiss(e));

            commands.CreateCommand("lenny")
                .Description("<words to prefix>\n\tSay ( ͡° ͜ʖ ͡°)")
                .Parameter("words", ParameterType.Unparsed)
                .Do(async (e) => await Lenny(e));

            commands.CreateCommand("list")
                .Description("<bestgirl | anime>\n\tPrint a top-list")
                .Parameter("words", ParameterType.Unparsed)
                .Do(async (e) => await List(e));

            commands.CreateCommand("money")
                .Description("\n\tPrint money")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async (e) => await Money(e));

            commands.CreateCommand("table")
                .Description("\n\tPrint tableflip / unflip")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async (e) => await Table(e));

            game = new Game(commands, discordClient);
            handler = new MessageHandler(commands, game);
            //music = new MusicHandler(commands, discordClient);

            // Mod commands
            commands.CreateCommand("restart")
                .Description("\n\tRestart Biribiri (mod)")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async e => await Restart(e));

            commands.CreateCommand("quit")
                .Description("\n\tPut her to sleep (mod)")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async (e) => await Quit(e));

            commands.CreateCommand("ban")
                .Description("\n\tBan a user (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Ban(e));

            commands.CreateCommand("kick")
                .Description("\n\tKick a user (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Kick(e));

            commands.CreateCommand("role")
                .Description("\n\tAdd a role to yourself (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Role(e));

            discordClient.MessageReceived += async (s, e) =>
            {
                if (game.guessingGame != null && game.guessingGame.running && e.Channel == game.guessingGame.channel)
                {
                    await game.guessingGame.Handle(e);
                }
                if (game.quizGame != null && game.quizGame.running && e.Channel == game.quizGame.channel)
                {
                    await game.quizGame.Handle(e);
                }
                if (!e.Channel.IsPrivate)
                    await handler.Handle(e);
            };

            discordClient.MessageDeleted += (s, e) =>
            {
                if (e.Message.Text.Length > 0 && !Constants.BOTids.Contains(e.User.Id)) Console.WriteLine(e.User.Name + ": " + e.Message.Text);
            };

            // Connecting to discord server
            discordClient.ExecuteAndWait(async () =>
            {
                await discordClient.Connect("MjQ0NDEwOTY0NjkzMjIxMzc3.Cv9KRg.HltvxZMWG5uHF9p9JTz95jWW_h8", TokenType.Bot);
            });
        }

        // Command functions
        private async Task Ban(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id != Constants.NYAid)
            {
                await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
                return;
            }
            if (e.Message.MentionedUsers.Count() <= 0)
            {
                await e.Channel.SendMessage("Banning user: " + e.GetArg("param"));
                return;
            }
            await e.Message.MentionedUsers.ElementAt(0).SendMessage("You got banned lolololol :joy:");
        }

        private async Task Biribiri(Discord.Commands.CommandEventArgs e)
        {
            try
            {
                await e.Message.Delete();
            } catch
            {
                Console.WriteLine(e.User.Name + " " + e.GetArg("param"));
            }
            var i = rng.Next(Responses.biribiri.Length);
            var str = Responses.biribiri[i];
            await e.Channel.SendFile(str);
        }

        private async Task Bye(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("user");
            var str = Responses.bye[rng.Next(Responses.bye.Length)];
            if (param.Length >= 1)
            {
                str = param + ", " + str;
            }
            if (e.User.Id == Constants.NYAid)
            {
                str += " <3";
            }
            str = FirstCharToUpper(str);
            await e.Message.Delete();
            await e.Channel.SendMessage(str);
        }

        private async Task Cat(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var i = rng.Next(Responses.cat.Length);
            var str = Responses.cat[i];
            await e.Channel.SendFile(str);
        }

        private async Task Cencored(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            await e.Channel.SendMessage("~~" + e.GetArg("text") + "~~");
        }

        private async Task Choose(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("arguments").Split(',');
            if(param.Length <= 0)
            {
                await e.Channel.SendMessage("You will have to list some options for me to choose from...");
                return;
            }
            if(param.Length == 1)
            {
                await e.Channel.SendMessage("If I cant choose '" + param[0] + "', obviously");
                return;
            }
            
            string answer = "If I have to choose between " + param[0];
            for(int i = 1; i < param.Length; i++)
            {
                if(param[i].First() == ' ') param[i] = String.Join("", param[i].Skip(1));
                if (i + 1 == param.Length) answer += " and " + param[i];
                else answer += ", " + param[i];
            }
            answer += ",\nmy answer would be: " + param[rng.Next(param.Length)];
            await e.Channel.SendMessage(answer);
        }

        private async Task Clean(Discord.Commands.CommandEventArgs e)
        {
            if (e.User.Id == Constants.NYAid)
            {
                await e.Message.Delete();
                var num = 30;
                User[] user = e.Message.MentionedUsers.ToArray();
                ulong userID;
                if (user.Count() > 0) userID = user[0].Id;
                else userID = Constants.BIRIBIRIid;
                var param = e.GetArg("amount").Split(' ')[0];
                if (param.Length > 0) Int32.TryParse(param, out num);
                Message[] m = await e.Channel.DownloadMessages(num);
                for (int i = 0; i < m.Count(); i++)
                {
                    if (userID == m[i].User.Id) await m[i].Delete();
                }
            } 
            else
            {
                await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
            }
        }

        private async Task Coinflip(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Coinflip command used by " + e.User);
            var param = e.GetArg("choice").Split(' ');
            var n = rng.Next(100);
            if (param.Length <= 0 || n <= 4 || !(param[0].ToLower() == "heads" || param[0].ToLower() == "tails"))
            {
                await e.Channel.SendMessage("Haha, witness the power of a level 5 electromaster instead!");
                await e.Channel.SendFile(Responses.railgun[rng.Next(Responses.railgun.Length)]);
                return;
            }
            string result;
            if (rng.Next(2) <= 0) result = "heads";
            else result = "tails";
            var message = "The coin landed on " + result;
            if (param[0].ToLower() == result)
            {
                message += ", you win this time";
            }
            else
            {
                message += ", better luck next time!";
            }
            await e.Channel.SendMessage(message);
        }

        private async Task Compliment(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("user");
            var str = Responses.compliments[rng.Next(Responses.compliments.Length)];
            if (param.Length >= 1)
            {
                str = param + ", " + str;
            }
            str = FirstCharToUpper(str);
            await e.Channel.SendMessage(str);
        }

        private async Task Echo(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("toEcho");
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("...");
            }
            else
            {
                await e.Channel.SendMessage(param.ToString());
            }
        }

        private async Task Delete(Discord.Commands.CommandEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            await e.Message.Delete();
        }

        private async Task Face(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var s = Responses.faces.ElementAt(MyBot.rng.Next(Responses.faces.Count()));
            var param = e.GetArg("words");
            if (param.Length > 0) s = param + " " + s;
            await e.Channel.SendMessage(s);
        }

        private async Task Hug(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var num = MyBot.rng.Next(Responses.hug.Count());
            if (e.Message.MentionedUsers.Count() <= 0 || e.Message.MentionedUsers.ElementAt(0).Id == e.User.Id)
            {
                await e.Channel.SendMessage("So lonely that you are trying to hug yourself? *hahaha*");
                return;
            }
            if(e.User.Id == Constants.WIZZid && e.Message.MentionedUsers.ElementAt(0).Id != Constants.NYAid)
            {
                await e.Channel.SendMessage(e.User.Mention + Responses.hug[num] + "NYA-CHAN, the best boyfriend in the world! :heart:");
                return;
            }
            await e.Channel.SendMessage(e.User.Mention + Responses.hug[num] + e.Message.MentionedUsers.ElementAt(0).Mention);
        }

        private async Task Karma(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            BiriInteraction u;
            if (e.Message.MentionedUsers.Count() > 0)
                u = handler.GetUser(e.Message.MentionedUsers.ElementAt(0).Id, e.Message.MentionedUsers.ElementAt(0).Name);
            else u = handler.GetUser(e.User.Id, e.User.Name);
            await e.Channel.SendMessage(u.name + "'s karma: " + u.karma + " (level: " + u.karmaLevel + ")");
        }

        private async Task Kill(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("user");
            if(param.Split(' ').Contains("nya") || param.Split(' ').Contains("biribiri") || (e.Message.MentionedUsers.Count() > 0 && (e.Message.MentionedUsers.ElementAt(0).Id == Constants.NYAid || e.Message.MentionedUsers.ElementAt(0).Id == Constants.BIRIBIRIid)))
            {
                await Compliment(e);
                return;
            }
            var str = Responses.burn[rng.Next(Responses.burn.Count())];
            if (param.Length >= 1)
            {
                str = param + ", " + str;
            }
            str = FirstCharToUpper(str);
            await e.Channel.SendMessage(str);
        }

        private async Task Kiss(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var message = "";
            var param = e.GetArg("user");
            if (param.Length <= 0)
            {
                message = "Are you trying to kiss yourself or something?\nWeirdo";
            }
            else
            {
                if(e.Message.MentionedUsers.ElementAt(0).Id == Constants.WIZZid && e.User.Id != Constants.NYAid)
                {
                    message = "Hahaha! No she is not yours";
                }
                else
                {
                    message = e.User.Mention + " gently kisses " + param.ToString() + " on the cheek <3";
                }
                
            }
            await e.Channel.SendMessage(message);
        }

        private async Task Lenny(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var s = "( ͡° ͜ʖ ͡°)";
            var param = e.GetArg("words");
            if (param.Length > 0) s = param + " " + s;
            await e.Channel.SendMessage(s);
        }

        private async Task List(Discord.Commands.CommandEventArgs e)
        {
            var param = e.GetArg("words").Split(' ');
            if(param.Length <= 0)
            {
                await e.Channel.SendMessage("What kind of List do you want?");
                return;
            }
            switch (param[0].ToLower())
            {
                case "bestgirl":
                    await e.Channel.SendMessage(Responses.topLists[0]);
                    break;
                case "anime":
                    await e.Channel.SendMessage(Responses.topLists[1]);
                    break;
                default:
                    await e.Channel.SendMessage("Stop swearing! :anger:");
                    break;
            }
        }

        private async Task Money(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var str = Responses.money[rng.Next(Responses.money.Length)];
            str = FirstCharToUpper(str);
            await e.Channel.SendMessage(str);
        }

        private async Task Restart(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                game.Abort();
                handler.Abort();
                System.Diagnostics.Process.Start(@"C:\Users\dejon\Documents\Visual Studio 2015\Projects\DiscordBot\DiscordBot\bin\Debug\DiscordBot.exe");
                Environment.Exit(0);
            }
            else await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
        }

        private async Task Table(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var s = "(╯°□°）╯︵ ┻━┻\n┬─┬﻿ ノ( ゜-゜ノ)";
            await e.Channel.SendMessage(s);
        }

        private async Task Quit(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                game.Quit();
                handler.Abort();
                System.Threading.Thread.Sleep(1000);
                await discordClient.Disconnect();
            }
            else
            {
                await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
            }
        }

        private async Task Kick(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id != Constants.NYAid)
            {
                await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
                return;
            }
            if (e.Message.MentionedUsers.Count() <= 0)
            {
                await e.Channel.SendMessage("KICKING USER " + e.GetArg("param"));
                return;
            }
            await e.Message.MentionedUsers.ElementAt(0).Kick();
        }

        private async Task Role(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                var param = e.GetArg("param");
                if(param.Length <= 0)
                {
                    await e.Channel.SendMessage("No arguments given");
                    return;
                }
                Discord.Role role = null;
                foreach(Discord.Role r in e.Server.Roles)
                {
                    if (r.Name.ToLower() == param)
                        role = r;
                }
                if (role != null)
                {
                    if (!e.User.HasRole(role))
                    {
                        await e.User.AddRoles(role);
                        Console.WriteLine(e.User.Name + " added role " + role.Name);
                        return;
                    }
                    await e.User.RemoveRoles(role);
                    Console.WriteLine(e.User.Name + " removed role " + role.Name);
                    return;
                }
                Console.WriteLine("Role " + param + " not found :/");
            }
            else await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
        }

        // Help function
        private async Task Help(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            string s = "**Help menu:**\n```";
            for(int i = 0; i < commands.AllCommands.Count(); i++)
            {
                s += commands.AllCommands.ElementAt(i).Text + " " + commands.AllCommands.ElementAt(i).Description + "\n";
            }
            s += "```";
            await e.User.SendMessage(s);
        }

        // Methods
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }
    }
}
