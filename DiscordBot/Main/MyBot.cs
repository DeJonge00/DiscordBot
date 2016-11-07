using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace DiscordBot.Main
{
    class MyBot
    {
        // Fields
        CommandService commands;
        DiscordClient discordClient;
        Random random;
        Game game;

        // Constructor
        public MyBot()
        {
            random = new Random();
            discordClient = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discordClient.UsingCommands(x =>
            {
                x.PrefixChar = '>';
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Private;
            });

            commands = discordClient.GetService<CommandService>();
            game = new Main.Game(commands);

            // List of commands
            commands.CreateCommand("biribiri")
                .Description("Print a pic of the one true Biribiri")
                .Do(async (e) => await biribiri(e));

            commands.CreateCommand("bye")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await bye(e));

            commands.CreateCommand("cat")
                .Description("Random cat pic!!!")
                .Do(async (e) => await cat(e));

            commands.CreateCommand("censored")
                .Description("Censor your own words")
                .Parameter("text", ParameterType.Unparsed)
                .Do(async (e) => await cencored(e));

            commands.CreateCommand("compliment")
                .Description("Give someone a compliment")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await compliment(e));

            commands.CreateCommand("echo")
                .Description("Echo echo echo")
                .Parameter("toEcho", ParameterType.Unparsed)
                .Do(async (e) => await echo(e));

            commands.CreateCommand("kiss")
                .Description("Kiss someone")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) => await kiss(e));

            commands.CreateCommand("lenny")
                .Description("Say ( ͡° ͜ʖ ͡°)")
                .Do(async (e) => await lenny(e));

            commands.CreateCommand("money")
                .Description("Print money")
                .Do(async (e) => await money(e));

            commands.CreateCommand("restart")
                .Description("Restart Biribiri")
                .Do(async e => await restart(e));

            commands.CreateCommand("quit")
                .Description("Put her to sleep (mod)")
                .Do(async (e) => await quit(e));

            // Game commands
            commands.CreateCommand("start")
                 .Description("Start game")
                 .Do(e => start(e));

            // Connecting to discord server
            discordClient.ExecuteAndWait(async () =>
            {
                await discordClient.Connect("MjQ0NDEwOTY0NjkzMjIxMzc3.Cv9KRg.HltvxZMWG5uHF9p9JTz95jWW_h8", TokenType.Bot);
            });
        }

        // Command functions
        private async Task biribiri(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Biribiri command used by " + e.User);
            await e.Message.Delete();
            var str = Responses.biribiri[random.Next(Responses.biribiri.Length)];
            str = "biribiri/1.jpg";
            await e.Channel.SendFile(str);
        }
        private async Task bye(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Bye command used by " + e.User);
            await e.Message.Delete();
            var param = e.GetArg("user");
            var str = Responses.bye[random.Next(Responses.bye.Length)];
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

        private async Task cat(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Cat command used by " + e.User);
            await e.Message.Delete();
            await e.Channel.SendMessage("http://random.cat/");
        }

        private async Task cencored(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Censor command used by " + e.User);
            await e.Message.Delete();
            await e.Channel.SendMessage("~~" + e.GetArg("text") + "~~");
        }

        private async Task compliment(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Compliment command used by " + e.User);
            await e.Message.Delete();
            var param = e.GetArg("user");
            var str = Responses.compliments[random.Next(Responses.compliments.Length)];
            if (param.Length >= 1)
            {
                str = param + ", " + str;
            }
            str = FirstCharToUpper(str);
            await e.Channel.SendMessage(str);
        }

        private async Task echo(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Echo command used by " + e.User);
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

        private async Task kiss(Discord.Commands.CommandEventArgs e)
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
                message = e.User.Mention + " gently kisses " + param.ToString() + " on the cheek <3";
            }
            await e.Channel.SendMessage(message);
        }

        private async Task lenny(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Lenny command used by " + e.User);
            await e.Message.Delete();
            await e.Channel.SendMessage("( ͡° ͜ʖ ͡°)");
        }

        private async Task money(Discord.Commands.CommandEventArgs e)
        {
            Console.WriteLine("Money command used by " + e.User);
            await e.Message.Delete();
            var str = Responses.money[random.Next(Responses.money.Length)];
            str = FirstCharToUpper(str);
            await e.Channel.SendMessage(str);
        }

        private async Task restart(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                game.abort();
                System.Diagnostics.Process.Start(@"C:\Users\dejon\Documents\Visual Studio 2015\Projects\DiscordBot\DiscordBot\bin\Debug\DiscordBot.exe");
                Environment.Exit(0);
            }
            else await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
        }

        private async Task start(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                game.startGame(discordClient.FindServers("test").FirstOrDefault().FindChannels("general", ChannelType.Text, false).FirstOrDefault(), 100);
                game.startGame(discordClient.FindServers("+18yo club").FirstOrDefault().FindChannels("general", ChannelType.Text, false).FirstOrDefault(), 20);
            }
            else await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
        }

        private async Task quit(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            Console.WriteLine("Quit command used by " + e.User);
            if (e.User.Id == Constants.NYAid)
            {
                game.abort();
                game.save();
                System.Threading.Thread.Sleep(1000);
                await discordClient.Disconnect();
            }
            else
            {
                await e.Channel.SendMessage("Only NYA-sama can tell me what to do!");
            }
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
