using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using DiscordBot.Main.Music;
using System.IO;
using System.Collections.Generic;
using DiscordBot.Main.GameObjects;
using DiscordBot.Main.RPG;

namespace DiscordBot.Main
{
    class MyBot
    {
        // Fields
        CommandService commands;

        public static DiscordClient discordClient       { get; private set; }
        public static Random rng = new Random();
        private MessageHandler handler;
        private MusicHandler music;
        private RPGMain rpg;
        private GuessingGame guessingGame;
        private QuizGame quizGame;
        private RPSGame rpsGame;
        private TruthOrDare todGame;

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
        private int loopLock = -1;
        private int languageLock = -1;
        private int ythoLock = -1;
        private int cuddleLock = -1;
        private int singLock = -1;

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
                .Description("\n\tCure cancer by posting a 60fps gif")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await CureCancer(e));

            commands.CreateCommand("biribiri")
                .Description("<number>\n\tPrint a pic of the one true Biribiri")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Biribiri(e));

            commands.CreateCommand("bye")
                .Description("<@user> \n\tSay goodbye to someone")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Bye(e));

            commands.CreateCommand("cast")
                .Description("<user>\n\tCast a random spell!")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Cast(e));

            commands.CreateCommand("cat")
                .Description("<number>\n\tRandom cat pic!!!")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Cat(e));

            commands.CreateCommand("censored")
                .Description("<message>\n\tCensor your own words")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Cencored(e));

            commands.CreateCommand("choose")
                .Description("<option1> {<, option2>}\n\tLet Biribiri choose one from your options")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Choose(e));

            commands.CreateCommand("coinflip")
                .Description("<heads | tails>\n\tFlip a coin with a slight chance of railgun")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Coinflip(e));

            commands.CreateCommand("compliment")
                .Description("<@user>\n\tGive someone a compliment")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Compliment(e));

            commands.CreateCommand("cuddle")
                .Description("<user>\n\tRandom cuddle gif!!!")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Cuddle(e));

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
                .Description("<message>\n\tEcho <message>")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Echo(e));

            commands.CreateCommand("echoelse")
                .Alias("echo2")
                .Description("<channel> <chat> <message>\n\tEcho <message> in <channel> - <chat>")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await EchoElsewhere(e));

            commands.CreateCommand("face")
                .Description("<words to prefix>\n\tSay (๑･̑◡･̑๑)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Face(e));

            commands.CreateCommand("help")
                .Description("\n\tHelp via direct messages")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Help(e));

            commands.CreateCommand("hug")
                .Description("<@user>\n\tHug a user")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Hug(e));

            commands.CreateCommand("kill")
                .Description("<user>\n\tThreathen someone")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Kill(e));

            commands.CreateCommand("kiss")
                .Description("<@user>\n\tKiss someone")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Kiss(e));

            commands.CreateCommand("language")
                .Alias("watchit")
                .Description("<number>\n\tLet people be mindfull of their language")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Language(e));

            commands.CreateCommand("lenny")
                .Description("<words to prefix>\n\tSay ( ͡° ͜ʖ ͡°)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Lenny(e));

            commands.CreateCommand("loop")
                .Description("\n\tLoop-da-loopy-loop!")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Loop(e));

            commands.CreateCommand("money")
                .Description("\n\tPrint money")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Money(e));

            commands.CreateCommand("ping")
                .Description("\n\tGet your ping (and its a game!)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Ping(e));

            commands.CreateCommand("picture")
                .Alias("pp")
                .Description("<@user | servername>\n\tGet the picture of the mentioned person")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Picture(e));

            commands.CreateCommand("sing")
                .Description("<songname>\n\tLet me sing a song for you")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Sing(e));

            commands.CreateCommand("table")
                .Description("\n\tPrint tableflip / unflip")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Table(e));

            commands.CreateCommand("y tho")
                .Alias("ytho")
                .Description("<number>\n\tBut... why though??")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await YTho(e));

            // Mod commands
            commands.CreateCommand("birigame")
                .Description("\n\tSet bb's game (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await SetGame(e));

            commands.CreateCommand("biristate")
                .Description("\n\tSet bb's state (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await SetState(e));

            commands.CreateCommand("restart")
                .Description("\n\tRestart Biribiri (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Restart(e));

            commands.CreateCommand("spam")
                .Description("<amount> <server>\n\tSpam a server XD (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Spam(e));

            commands.CreateCommand("type")
                .Description("<amount> <server> <channel>\n\tSend ***istyping*** to a channel (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Type(e));

            commands.CreateCommand("quit")
                .Description("\n\tPut her to sleep (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Quit(e));

            commands.CreateCommand("ban")
                .Description("\n\tBan a user (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Ban(e));

            commands.CreateCommand("clean")
                .Description("\n\tRemove messages of Biribiri (mod)")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Clean(e));

            discordClient.MessageReceived += async (s, e) =>
            {
                try
                {
                    if (e.User.IsBot)
                    {
                        return;
                    }
                    // In case I get kicked one day and they forget BB
                    /*if(e.Server.Name == "9CHAT")
                    {
                        Log(e.User.Name + " - " + e.Channel.Name + " : " + e.Message.Text, "fuckThatLoli");
                    }*/
                    if (e.Channel.IsPrivate && e.Message.Text.Count() > 0)
                    {
                        Log(e.User.Name + " send dm containing: " + e.Message.Text, "privateMessages");
                    }
                    if (guessingGame != null && guessingGame.running && e.Channel == guessingGame.channel)
                    {
                        await guessingGame.Handle(e);
                    }
                    if (quizGame != null && quizGame.running && e.Channel == quizGame.channel)
                    {
                        await quizGame.Handle(e);
                    }
                    if (!e.Channel.IsPrivate)
                    {
                        await handler.Handle(e);
                        await rpg.Handle(e);
                    }
                } catch
                {
                    Console.WriteLine("discordClient.MessageReceived failed");
                }
            };

            discordClient.MessageDeleted += (s, e) =>
            {
                if (e.Message.Text != null && e.Message.Text.Length > 0 && e.Message.Text.Length < 300 && !e.User.IsBot)
                {
                    var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + " deleted: " + e.Message.Text;
                    MyBot.Log(str, e.Server.Name);
                }
            };

            discordClient.MessageUpdated += (s, e) =>
            {
                if (e.Before.Text != null && e.Before.Text.Length > 0 && e.Before.Text.Length < 300 && !e.User.IsBot)
                {
                    var str = e.Before.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + " edited: " + e.Before.Text;
                    MyBot.Log(str, e.Server.Name);
                }
            };

            discordClient.UserUpdated += (s, e) =>
            {
                var message = DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + e.Server.Name + ") User changed: " + e.Before.Name + " |";
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
                MyBot.Log(message, e.Server.Name);
            };

            discordClient.RoleUpdated += (s, e) =>
            {
                var message = DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + e.Server.Name + ") Role changed: " + e.Before.Name + " |";
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
                MyBot.Log(message, e.Server.Name);
            };

            discordClient.UserJoined += async (s, e) =>
            {
                var str = "Welcome " + e.User.Mention + "!!! :cate:";
                // e.Server.FindChannels("general").FirstOrDefault().SendMessage(str);
                Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + e.Server.Name + ") " + "Welcome " + e.User.Name + "!!! :cate:", e.Server.Name);
            };

            discordClient.UserLeft += async (s, e) =>
            {
                var str = "Im sorry to say that \"" + e.User.Name + "\" just left :sob: :sob:";
                await e.Server.FindChannels("general").FirstOrDefault().SendMessage(str);
                Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + e.Server.Name + ") " + str, e.Server.Name);
            };

            handler = new MessageHandler();
            //guessingGame = new GuessingGame(commands);
            rpsGame = new RPSGame(commands);
            quizGame = new QuizGame(commands);
            todGame = new TruthOrDare(commands);
            music = new MusicHandler(commands, discordClient);
            rpg = new RPGMain(commands, discordClient);

            // Connecting to discord server
            discordClient.ExecuteAndWait(async () =>
            {
                try
                {
                    await discordClient.Connect(Constants.botToken, TokenType.Bot);
                } catch
                {
                    Console.WriteLine("Connecting failed");
                }
                discordClient.SetGame("with loli's <3");
                discordClient.SetStatus(UserStatus.DoNotDisturb.Value);
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
            if (i > Responses.biribiri.Count() || i < 0)
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
            var param = e.GetArg("param");
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

        private async Task Cast(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var mess = "**" + e.User.Name + "** casts "
                + Responses.spell.ElementAt(MyBot.rng.Next(Responses.spell.Count())) + " on **"
                + e.GetArg("param") + "**! " + Responses.spellresult.ElementAt(MyBot.rng.Next(Responses.spellresult.Count()));
            await e.Channel.SendMessage(mess);
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
            if (i > Responses.cat.Count() || i < 0)
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
            var param = e.GetArg("param").Split(',');
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
                var param = e.GetArg("param").Split(' ')[0];
                if (param.Length > 0) Int32.TryParse(param, out num);
                if (num < 1) return;
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
            var param = e.GetArg("param").Split(' ');
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
            var param = e.GetArg("param");
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

        private async Task Cuddle(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.cuddle.Length);
                } while (i == cuddleLock);
            }
            if (i > Responses.cuddle.Count() || i < 0)
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.cuddle.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            cuddleLock = i;
            if(e.Message.MentionedUsers.Count() > 0)
            {
                await e.Channel.SendMessage("A special cuddle for you " + e.Message.MentionedUsers.ElementAt(0).Mention);
            }
            var str = Responses.cuddle[i];
            await e.Channel.SendFile(str);
        }

        private async Task CureCancer(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.curecancer.Count());
                } while (i == cureLock);
            }
            if (i > Responses.curecancer.Count() || i < 0)
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.curecancer.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            cureLock = i;
            var gif = Responses.curecancer[i];
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
            if (i > Responses.dedchat.Count() || i < 0)
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
            var param = e.GetArg("param");
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("...");
                return;
            }
            await e.Channel.SendMessage(param.ToString());
        }

        private async Task EchoElsewhere(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("param").Split(' ');
            if (param.Length < 3)
            {
                await e.Channel.SendMessage("...");
                return;
            }
            var message = String.Join(" ", param.Skip(2));
            try
            {
                await discordClient.FindServers(param.ElementAt(0)).First().FindChannels(param.ElementAt(1)).First().SendMessage(message);
            } catch
            {
                var m = await e.Channel.SendMessage("Channel could not be found :/");
                System.Threading.Thread.Sleep(3000);
                await m.Delete();
            }
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
            var param = e.GetArg("param");
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
            await e.Channel.SendMessage(e.User.Mention + Responses.hug[num] + e.Message.MentionedUsers.ElementAt(0).Mention);
        }

        private async Task Kill(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("param");
            if(param.Split(' ').Contains("nya") || param.Split(' ').Contains("biribiri") || (e.Message.MentionedUsers.Count() > 0 && (e.Message.MentionedUsers.ElementAt(0).Id == Constants.NYAid || e.Message.MentionedUsers.ElementAt(0).Id == Constants.BIRIBIRIid)))
            {
                await Compliment(e);
                return;
            }
            if (e.Message.MentionedUsers.Count() > 0 && (e.GetArg("param").Split(' ').Contains(e.User.Name) || e.Message.MentionedUsers.ElementAt(0) == e.User)) {
                await e.Channel.SendMessage("You should not be trying to kill yourself, suicide is never the answer! :heart:");
                return;
            }
            int i;
            do
            {
                i = rng.Next(Responses.burn.Length);
            } while (i == kysLock);
            kysLock = i;
            var str = Responses.burn[i] + " :smiling_imp:";
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
            var param = e.GetArg("param");
            if (param.Length <= 0)
            {
                message = "Are you trying to kiss yourself or something?\nWeirdo";
            }
            else
            {
                message = e.User.Mention + " gently kisses " + param.ToString() + " on the cheek <3";
            }
            await e.Channel.SendMessage(message);
        }

        private async Task Language(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.language.Length);
                } while (i == languageLock);
            }
            if (i > Responses.language.Count() || i < 0)
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.language.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            languageLock = i;
            var str = Responses.language[i];
            await e.Channel.SendFile(str);
        }

        private async Task Lenny(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var s = "( ͡° ͜ʖ ͡°)";
            var param = e.GetArg("param");
            if (param.Length > 0) s = param + " " + s;
            await e.Channel.SendMessage(s);
        }

        private async Task List(Discord.Commands.CommandEventArgs e)
        {
            var param = e.GetArg("param").Split(' ');
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

        private async Task Loop(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            do
            {
                i = rng.Next(Responses.loop.Length);
            } while (i == loopLock);
            loopLock = i;
            string message = Responses.loop.ElementAt(i) + " ";
            var m = await e.Channel.SendMessage(message);
            for(int j = 1; j < 10; j++)
            {
                message += Responses.loop.ElementAt(i) + " ";
                System.Threading.Thread.Sleep(2000);
                await m.Edit(message);
            }
            System.Threading.Thread.Sleep(3000);
            await m.Delete();
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

        private async Task Picture(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if(e.Message.MentionedUsers.Count() > 0)
            {
                var name = e.Message.MentionedUsers.ElementAt(0).Name + ".jpg";
                MessageHandler.SaveFile(@"F:\DiscordBot\pp\", name, e.Message.MentionedUsers.ElementAt(0).AvatarUrl);
                await e.Channel.SendFile(Path.Combine(@"F:\DiscordBot\pp\",  name));
                return;
            }
            if(e.GetArg("param").Length > 0 && e.GetArg("param") == e.Server.Name)
            {
                var name = e.Server.Name + ".jpg";
                Console.WriteLine(e.Server.Name);
                try { MessageHandler.SaveFile(@"F:\DiscordBot\pp\", name, e.Server.IconUrl); }
                catch (Exception ex) { Console.WriteLine(ex.StackTrace); }
                await e.Channel.SendFile(Path.Combine(@"F:\DiscordBot\pp\", name));
                return;
            }
            await e.Channel.SendMessage("Give me a valid user or the servername as parameter pl0x");
        }

        private async Task Restart(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                rpg.Abort();
                System.Diagnostics.Process.Start(@"C:\Users\dejon\Documents\Visual Studio 2015\Projects\DiscordBot\DiscordBot\bin\Debug\DiscordBot.exe");
                Environment.Exit(0);
            }
            else await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
        }

        private async Task SetGame(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                var param = e.GetArg("param");
                if (param.Length > 0)
                {
                    try
                    {
                        discordClient.SetGame(param);
                        return;
                    }
                    catch
                    {
                        Console.WriteLine("Setting game failed");
                    }
                }
                return;
            }
            await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
        }

        private async Task SetState(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                var param = e.GetArg("param");
                if (param.Length > 0)
                {
                    try
                    {
                        discordClient.SetStatus(param);
                        return;
                    } catch
                    {
                        Console.WriteLine("Setting state failed");
                    }
                }
                await e.Channel.SendMessage("Choose from: " + UserStatus.DoNotDisturb.Value + ", " + UserStatus.Idle.Value + ", " + UserStatus.Invisible.Value + ", " + UserStatus.Online.Value + ", please!");
                return;
            }
            await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
        }

        private async Task Sing(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i = 0;
            if (e.GetArg("param").Length > 0)
            {
                if (!Int32.TryParse(e.GetArg("param"), out i))
                {
                    switch (e.GetArg("param"))
                    {
                        case "sos":
                        case "sound of silence":
                            i = 0;
                            break;
                        case "pretender":
                            i = 1;
                            break;
                        case "pomf":
                        case "pomf pomf":
                            i = 2;
                            break;
                        default:
                            await e.Channel.SendMessage("I don't know that song...");
                            return;
                    }
                }
            }
            else
            {
                do
                {
                    i = rng.Next(Responses.songs.Length);
                } while (i == singLock);
            }
            singLock = i;
            var song = Responses.songs[i].Split('|');
            var m = await e.Channel.SendMessage(song[0]);
            for (int j = 1; j < song.Count(); j++)
            {
                System.Threading.Thread.Sleep(3000);
                await m.Edit(song[j]);
            }
            System.Threading.Thread.Sleep(3000);
            await m.Edit("*bows*");
            System.Threading.Thread.Sleep(5000);
            await m.Delete();
            return;
        }

        private async Task Spam(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                var param = e.GetArg("param");
                if (param.Count() <= 0)
                    return;
                try
                {
                    var amount = 1;
                    var messages = new List<Message>();
                    Int32.TryParse(param, out amount);
                    for(int i=0;i<amount;i++)
                        foreach (Channel ch in e.Server.TextChannels)
                            messages.Add(await ch.SendMessage("SPAM LOL"));
                    System.Threading.Thread.Sleep(amount*1500);
                    foreach (Message m in messages)
                        await m.Delete();
                } catch
                {
                    await e.Channel.SendMessage("Spamming failed");
                }
            }
        }

        private async Task Type(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                var param = e.GetArg("param").Split(' ');
                if (param.Count() <= 2)
                    return;
                try
                {
                    var amount = 1;
                    var messages = new List<Message>();
                    Int32.TryParse(param[0], out amount);
                    for (int i = 0; i < amount; i++)
                    {
                        try
                        {
                            await discordClient.FindServers(param[1]).First().FindChannels(param[2]).First().SendIsTyping();
                            System.Threading.Thread.Sleep(9000);
                        } catch
                        {
                            Console.WriteLine("Channel not found");
                            return;
                        }
                    }
                    
                    foreach (Message m in messages)
                        await m.Delete();
                }
                catch
                {
                    await e.Channel.SendMessage("Spamming failed");
                }
            }
        }

        private async Task Table(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var s = "(╯°□°）╯︵ ┻━┻\n┬─┬﻿ ノ( ゜-゜ノ)";
            await e.Channel.SendMessage(s);
        }

        private async Task YTho(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            int i;
            if (!Int32.TryParse(e.GetArg("param"), out i))
            {
                do
                {
                    i = rng.Next(Responses.ytho.Length);
                } while (i == ythoLock);
            }
            if (i > Responses.ytho.Count() || i < 0)
            {
                await e.Channel.SendMessage("Best I can do is " + Responses.ytho.Count() + " ¯\\_(ツ)_/¯");
                return;
            }
            ythoLock = i;
            var str = Responses.ytho[i];
            await e.Channel.SendFile(str);
        }

        private async Task Quit(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                rpg.Abort();
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
            Console.WriteLine("x: " + commands.AllCommands.Count());
            for(int i = 0; i < commands.AllCommands.Count(); i++)
            {
                if(i%40==0 && i!=0)
                {
                    await e.User.SendMessage(s+"```");
                    s = "```";
                }
                s += commands.AllCommands.ElementAt(i).Text + " " + commands.AllCommands.ElementAt(i).Description + "\n";
            }
            s += "```\n*If anything does not seem to work as it is supposed to: msg NYA-CHAN*";
            await e.User.SendMessage(s);
        }

        // Methods
        private void Log(object sender, LogMessageEventArgs e)
        {
            var str = DateTime.Now.ToUniversalTime().ToShortTimeString() + " - " + e.Severity + " - " + e.Source + ") " + e.Message;
            File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "Logs", "log.txt"), str + Environment.NewLine);
            Console.WriteLine(str);
        }

        public static void Log(string s, string filename)
        {
            File.AppendAllText(Path.Combine(Environment.CurrentDirectory, "Logs", filename + "_log.txt"), s + Environment.NewLine);
            Console.WriteLine(s);
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException("ARGH!");
            return input.First().ToString().ToUpper() + String.Join("", input.Skip(1));
        }
    }
}
