using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;
using DiscordBot.Main.Music;
using System.IO;
using System.Collections.Generic;

namespace DiscordBot.Main
{
    class MyBot
    {
        // Fields
        CommandService commands;

        public static DiscordClient discordClient       { get; private set; }
        public static Random rng = new Random();
        public MessageHandler handler                   { get; private set; }
        public Game game                                { get; private set; }
        public MusicHandler music                       { get; private set; }

        // Duplicate saving
        private int biribiriLock = -1;
        private int byeLock = -1;
        private int catLock = -1;
        private int complimentLock = -1;
        private int faceLock = -1;
        private int hugLock = -1;
        private int moneyLock = -1;
        private int kysLock = -1;
        private int cureLock = -1;
        private int dedLock = -1;

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
            commands.CreateCommand("60fps")
                .Alias("60")
                .Description("<number\n\tPrint a pic of the one true Biribiri")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await CureCancer(e));

            commands.CreateCommand("biribiri")
                .Description("<number>\n\tPrint a pic of the one true Biribiri")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Biribiri(e));

            commands.CreateCommand("bye")
                .Description("<user> \n\tSay goodbye to someone")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await Bye(e));

            commands.CreateCommand("cat")
                .Description("<number>\n\tRandom cat pic!!!")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Cat(e));

            commands.CreateCommand("censored")
                .Description("<message>\n\tCensor your own words")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) => await Cencored(e));

            commands.CreateCommand("choose")
                .Description("<option1> {<;option2>}\n\tLet Biribiri choose one from your options")
                .Parameter("arguments", ParameterType.Unparsed)
                .Do(async (e) => await Choose(e));

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

            commands.CreateCommand("ded")
                .Alias("chat")
                .Description("\n\tLiven chat up a little")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await DedChat(e));

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

            commands.CreateCommand("ping")
                .Description("\n\tGet your ping (and its a game!)")
                .Parameter("null", ParameterType.Unparsed)
                .Do(async (e) => await Ping(e));

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

            commands.CreateCommand("clean")
                .Description("\n\tRemove messages of Biribiri (mod)")
                .Parameter("amount", ParameterType.Unparsed)
                .Do(async (e) => await Clean(e));

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
                if (e.Message.Text.Length > 0 && !e.User.IsBot)
                {
                    var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + ": " + e.Message.Text;
                    File.AppendAllText(@"F:\DiscordBot\log\log.txt", str + Environment.NewLine);
                    Console.WriteLine(str);
                }
            };

            discordClient.UserUpdated += (s, e) =>
            {
                var message = "User changed: " + e.Before.Name + " |";
                if (e.Before.Nickname != e.After.Nickname)
                {
                    message += " nn from: " + e.Before.Nickname + " to: " + e.After.Nickname;
                }
                else if (e.Before.Name != e.After.Name)
                {
                    message += " name from: " + e.Before.Name + " to: " + e.After.Name;
                }
                else if (!e.Before.Roles.ToList().Equals(e.After.Roles.ToList()))
                {
                    var check = message;
                    List<Role> roles1 = e.Before.Roles.ToList();
                    List<Role> roles2 = e.After.Roles.ToList();
                    var listMin = roles1.Except(roles2).ToList();
                    var listPlus = roles2.Except(roles1).ToList();
                    foreach (Role r in listMin)
                    {
                        message += " -role: " + r.Name;
                    }
                    foreach (Role r in listPlus)
                    {
                        message += " +role: " + r.Name;
                    }
                    if (check == message)
                        return;
                }
                else
                {
                    return;
                }
                Console.WriteLine(message);
            };

            discordClient.RoleUpdated += (s, e) =>
            {
                var message = "Role changed:";
                if (e.Before.Permissions.Administrator != e.After.Permissions.Administrator)
                {
                    if (e.Before.Permissions.Administrator)
                        message += " -Administrator";
                    else message += " +Administrator";
                }
                if (e.Before.Permissions.AttachFiles != e.After.Permissions.AttachFiles)
                {
                    if (e.Before.Permissions.AttachFiles)
                        message += " -AttachFiles";
                    else message += " +AttachFiles";
                }
                if (e.Before.Permissions.BanMembers != e.After.Permissions.BanMembers)
                {
                    if (e.Before.Permissions.BanMembers)
                        message += " -BanMembers";
                    else message += " +BanMembers";
                }
                if (e.Before.Permissions.ChangeNickname != e.After.Permissions.ChangeNickname)
                {
                    if (e.Before.Permissions.ChangeNickname)
                        message += " -ChangeNickname";
                    else message += " +ChangeNickname";
                }
                if (e.Before.Permissions.Connect != e.After.Permissions.Connect)
                {
                    if (e.Before.Permissions.Connect)
                        message += " -Connect";
                    else message += " +Connect";
                }
                if (e.Before.Permissions.CreateInstantInvite != e.After.Permissions.CreateInstantInvite)
                {
                    if (e.Before.Permissions.CreateInstantInvite)
                        message += " -CreateInstantInvite";
                    else message += " +CreateInstantInvite";
                }
                if (e.Before.Permissions.DeafenMembers != e.After.Permissions.DeafenMembers)
                {
                    if (e.Before.Permissions.DeafenMembers)
                        message += " -DeafenMembers";
                    else message += " +DeafenMembers";
                }
                if (e.Before.Permissions.EmbedLinks != e.After.Permissions.EmbedLinks)
                {
                    if (e.Before.Permissions.EmbedLinks)
                        message += " -EmbedLinks";
                    else message += " +EmbedLinks";
                }
                if (e.Before.Permissions.KickMembers != e.After.Permissions.KickMembers)
                {
                    if (e.Before.Permissions.KickMembers)
                        message += " -KickMembers";
                    else message += " +KickMembers";
                }
                if (e.Before.Permissions.ManageChannels != e.After.Permissions.ManageChannels)
                {
                    if (e.Before.Permissions.ManageChannels)
                        message += " -ManageChannels";
                    else message += " +ManageChannels";
                }
                if (e.Before.Permissions.ManageMessages != e.After.Permissions.ManageMessages)
                {
                    if (e.Before.Permissions.ManageMessages)
                        message += " -ManageMessages";
                    else message += " +ManageMessages";
                }
                if (e.Before.Permissions.ManageNicknames != e.After.Permissions.ManageNicknames)
                {
                    if (e.Before.Permissions.ManageNicknames)
                        message += " -ManageNicknames";
                    else message += " +ManageNicknames";
                }
                if (e.Before.Permissions.ManageRoles != e.After.Permissions.ManageRoles)
                {
                    if (e.Before.Permissions.ManageRoles)
                        message += " -ManageRoles";
                    else message += " +ManageRoles";
                }
                if (e.Before.Permissions.ManageServer != e.After.Permissions.ManageServer)
                {
                    if (e.Before.Permissions.ManageServer)
                        message += " -ManageServer";
                    else message += " +ManageServer";
                }
                if (e.Before.Permissions.MentionEveryone != e.After.Permissions.MentionEveryone)
                {
                    if (e.Before.Permissions.MentionEveryone)
                        message += " -MentionEveryone";
                    else message += " +MentionEveryone";
                }
                if (e.Before.Permissions.MoveMembers != e.After.Permissions.MoveMembers)
                {
                    if (e.Before.Permissions.MoveMembers)
                        message += " -MoveMembers";
                    else message += " +MoveMembers";
                }
                if (e.Before.Permissions.MuteMembers != e.After.Permissions.MuteMembers)
                {
                    if (e.Before.Permissions.MuteMembers)
                        message += " -MuteMembers";
                    else message += " +MuteMembers";
                }
                if (e.Before.Permissions.ReadMessageHistory != e.After.Permissions.ReadMessageHistory)
                {
                    if (e.Before.Permissions.ReadMessageHistory)
                        message += " -ReadMessageHistory";
                    else message += " +ReadMessageHistory";
                }
                if (e.Before.Permissions.ReadMessages != e.After.Permissions.ReadMessages)
                {
                    if (e.Before.Permissions.ReadMessages)
                        message += " -ReadMessages";
                    else message += " +ReadMessages";
                }
                if (e.Before.Permissions.SendMessages != e.After.Permissions.SendMessages)
                {
                    if (e.Before.Permissions.SendMessages)
                        message += " -SendMessages";
                    else message += " +SendMessages";
                }
                if (e.Before.Permissions.SendTTSMessages != e.After.Permissions.SendTTSMessages)
                {
                    if (e.Before.Permissions.SendTTSMessages)
                        message += " -SendTTSMessages";
                    else message += " +SendTTSMessages";
                }
                if (e.Before.Permissions.Speak != e.After.Permissions.Speak)
                {
                    if (e.Before.Permissions.Speak)
                        message += " -Speak";
                    else message += " +Speak";
                }
                if (e.Before.Permissions.UseVoiceActivation != e.After.Permissions.UseVoiceActivation)
                {
                    if (e.Before.Permissions.UseVoiceActivation)
                        message += " -UseVoiceActivation";
                    else message += " +UseVoiceActivation";
                }
                Console.WriteLine(message);
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
            await e.Message.Delete();
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.biribiri.Length);
                } while (i == biribiriLock);
            }
            if (i > Responses.biribiri.Count())
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.biribiri.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            biribiriLock = i;
            var str = Responses.biribiri[i];
            await e.Channel.SendFile(str);
        }

        private async Task Bye(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("user");
            int i;
            do
            {
                i = rng.Next(Responses.bye.Length);
            } while (i == byeLock);
            byeLock = i;
            var str = Responses.bye[rng.Next(i)];
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
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.cat.Length);
                } while (i == catLock);
            }
            if (i > Responses.cat.Count())
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.cat.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            catLock = i;
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
            var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + ": " + e.Message.Text;
            File.AppendAllText(@"F:\DiscordBot\log\log.txt", str + Environment.NewLine);
            Console.WriteLine(str);
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
            int i;
            do
            {
                i = rng.Next(Responses.compliments.Length);
            } while (i == complimentLock);
            complimentLock = i;
            var str = Responses.compliments[rng.Next(i)];
            if (param.Length >= 1)
            {
                str = param + ", " + str;
            }
            str = FirstCharToUpper(str);
            await e.Channel.SendMessage(str);
        }

        private async Task CureCancer(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.curecancer.Length);
                } while (i == cureLock);
            }
            if (i > Responses.curecancer.Count())
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.curecancer.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            cureLock = i;
            var gif = Responses.curecancer[i];
            var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + ": " + e.Message.Text;
            File.AppendAllText(@"F:\DiscordBot\log\log.txt", str + Environment.NewLine);
            Console.WriteLine(str);
            await e.Channel.SendFile(gif);
        }

        private async Task DedChat(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.dedchat.Length);
                } while (i == dedLock);
            }
            if (i > Responses.dedchat.Count())
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.dedchat.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            biribiriLock = i;
            var str = Responses.dedchat[i];
            await e.Channel.SendFile(str);
        }

        private async Task Echo(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("toEcho");
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("...");
                return;
            }
            await e.Channel.SendMessage(param.ToString());
        }

        private async Task Delete(Discord.Commands.CommandEventArgs e)
        {
            int i = 15;
            Int32.TryParse(e.GetArg("param").Split().ElementAt(0), out i);
            System.Threading.Thread.Sleep(100*i);
            await e.Message.Delete();
        }

        private async Task Face(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            do
            {
                i = rng.Next(Responses.faces.Length);
            } while (i == faceLock);
            faceLock = i;
            var s = Responses.faces.ElementAt(i);
            var param = e.GetArg("words");
            if (param.Length > 0) s = param + " " + s;
            await e.Channel.SendMessage(s);
        }

        private async Task Hug(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int num;
            do
            {
                num = rng.Next(Responses.hug.Length);
            } while (num == hugLock);
            hugLock = num;
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
            int i;
            do
            {
                i = rng.Next(Responses.burn.Length);
            } while (i == kysLock);
            kysLock = i;
            var str = Responses.burn[i];
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
            int i;
            do
            {
                i = rng.Next(Responses.money.Length);
            } while (i == moneyLock);
            moneyLock = i;
            var str = Responses.money[i];
            str = FirstCharToUpper(str);
            await e.Channel.SendMessage(str);
        }

        private async Task Ping(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var timediff = DateTime.Now.ToUniversalTime();
            timediff.Subtract(e.Message.Timestamp);
            Discord.Message mess;
            if (rng.Next(3) == 0)
            {
                if(rng.Next(2) == 0)
                {
                    mess =await e.Channel.SendMessage("You missed, I win! `" + e.User.Name + "'s ping: " + timediff.Millisecond.ToString() + "`");
                } else
                {
                    mess = await e.Channel.SendMessage("Ooops, you win this time... ` " + e.User.Name + "'s ping: " + timediff.Millisecond.ToString() + "`");
                }
            } else
            {
                mess = await e.Channel.SendMessage("Pong! ` " + e.User.Name + "'s ping: " + timediff.Millisecond.ToString() + "`");
            }
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
            var str = DateTime.Now.ToShortTimeString() + " - " + e.Severity + " - " + e.Source + ") " + e.Message;
            File.AppendAllText(@"F:\DiscordBot\log\log.txt", str + Environment.NewLine);
            Console.WriteLine(str);
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }
    }
}
