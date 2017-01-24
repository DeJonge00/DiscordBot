using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace DiscordBot.Main.GameObjects
{
    class RPSGame
    {
        private CommandService commands;
        private GameControl game;
        public bool running                         { get; private set; }
        private Channel channel;
        private User host;
        private string hostChoice;
        private User opponent;
        private string opponentChoice;
        private string[] options                = { "rock", "paper", "scissors" };

        public RPSGame(CommandService c, GameControl g)
        {
            commands = c;
            game = g;

            commands.CreateCommand("rps")
                .Description("<stop | create> <opponent>\n\tRock-paper-scissors game")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await InitGame(e));
        }

        private async Task InitGame(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("param").Split(' ');
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("Specify <create | stop> おねがいします");
                return;
            }
            if (options.Contains(param[0].ToLower()))
            {
                if(!running)
                {
                    await e.Channel.SendMessage("How does one play a game that is not running?");
                    return;
                }
                if (e.User.Id == host.Id)
                {
                    hostChoice = param[0];
                    await FinishGame();
                    return;
                }
                if (e.User.Id == opponent.Id)
                {
                    opponentChoice = param[0];
                    await FinishGame();
                    return;
                }
                await e.User.SendMessage("Shhhh, people are playing");
                return;
            }
                if (param[0].ToLower() == "stop" || param[0].ToLower() == "quit")
            {
                if (!running)
                {
                    await e.Channel.SendMessage("How does one stop a game that is not running?");
                    return;
                }
                if (e.User != host && e.User != opponent && e.User.Id != Constants.NYAid)
                {
                    await e.Channel.SendMessage("Shhh, only the game creator can stop the game!");
                    return;
                }
                await e.Channel.SendMessage("One of the players left the rps game!");
                running = false;
                return;
            }

            if (param[0].ToLower() == "create")
            {
                if (running)
                {
                    await e.Channel.SendMessage("There is already a game running somewhere, sowwy");
                    return;
                }
                if (e.Message.MentionedUsers.Count() <= 0)
                {
                    await e.Channel.SendMessage("Mention a user to play against!");
                    return;
                }
                host = e.User;
                opponent = e.Message.MentionedUsers.ElementAt(0);
                await e.Channel.SendMessage("Rock-paper-scissors game succesfully created between "+ host.Name + " and " + opponent.Name + "-desu");
                channel = e.Channel;
                await StartGame();
                return;
            }
            await e.Channel.SendMessage("Specify <create | stop> おねがいします");
        }

        private async Task StartGame()
        {
            hostChoice = null;
            opponentChoice = null;
            running = true;
            await host.SendMessage("Choose between '>rps Rock', '>rps Paper' or '>rps Scissors' (or '>rps stop' to quit the game)");
            await opponent.SendMessage("Choose between '>rps Rock', '>rps Paper' or '>rps Scissors' (or '>rps stop' to quit the game)");
        }

        private async Task FinishGame()
        {
            if (opponentChoice == null || hostChoice == null)
                return;
            var answer = host.Name + " chose " + hostChoice + ", " + opponent.Name + " chose " + opponentChoice;
            if (opponentChoice.ToLower() == hostChoice.ToLower())
            { // Draw
                answer += "\nThe result is a draw!";
            }
            if ((opponentChoice.ToLower() == "rock" && hostChoice.ToLower() == "scissors") || (opponentChoice.ToLower() == "paper" && hostChoice.ToLower() == "rock") || (opponentChoice.ToLower() == "scissors" && hostChoice.ToLower() == "paper"))
            { // Opponent wins
                answer += "\nAnd the winner is... " + opponent.Name;
                game.GetUser(opponent.Id, opponent.Name).AddPoints(20);
                game.GetUser(host.Id, host.Name).AddPoints(5);
            }
            if ((hostChoice.ToLower() == "rock" && opponentChoice.ToLower() == "scissors") || (hostChoice.ToLower() == "paper" && opponentChoice.ToLower() == "rock") || (hostChoice.ToLower() == "scissors" && opponentChoice.ToLower() == "paper"))
            { // Host wins
                answer += "\nThe challenger " + host.Name + " wins!";
                game.GetUser(host.Id, host.Name).AddPoints(20);
                game.GetUser(opponent.Id, opponent.Name).AddPoints(5);
            }
            await channel.SendMessage(answer);
            running = false;
        }
    }
}
