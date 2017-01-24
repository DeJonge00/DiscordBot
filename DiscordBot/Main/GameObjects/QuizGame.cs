using Discord;
using Discord.Commands;
using DiscordBot.Main.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Main
{
    class QuizGame
    {
        private GameControl game;
        public Channel channel { get; private set; }
        public User host { get; private set; }
        public bool running { get; private set; }
        public QuizQuestion currentQuestion { get; private set; }
        public int questionsLeft { get; private set; }
        private List<QuizQuestion> questions;

        public QuizGame(CommandService commands, GameObjects.GameControl g)
        {
            game = g;
            running = false;

            commands.CreateCommand("quiz")
                .Alias("qg")
                .Description("<stop | create> <amount of questions> <genre>\n\tA quiz")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await InitGame(e));
        }

        public async Task InitGame(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("param").Split(' ');
            if (param.Length <= 0)
            {
                await e.Channel.SendMessage("Specify <create | stop> おねがいします");
                return;
            }
            
            if (param[0].ToLower() == "stop")
            {
                if (!running)
                {
                    await e.Channel.SendMessage("How does one stop a game that is not running?");
                    return;
                }
                if (e.User != host && e.User.Id != Constants.NYAid)
                {
                    await e.Channel.SendMessage("Shhh, only the game creator can stop the game!");
                    return;
                }
                running = false;
                await NextQuestion();
                return;
            }

            if (param[0].ToLower() == "create")
            {
                if (running)
                {
                    await e.Channel.SendMessage("There is already a game running somewhere, sowwy");
                    return;
                }
                int amount;
                if(param.Length <= 1 || !Int32.TryParse(param[1], out amount))
                {
                    await e.Channel.SendMessage("Specify how many questions I should ask, my darlin'");
                    return;
                }
                if (param.Length <= 2)
                {
                    questions = new List<QuizQuestion>();
                    questions.AddRange(Constants.animeQuestions);
                    questions.AddRange(Constants.musicQuestions);
                }
                else
                {
                    switch (param[2])
                    {
                        case "anime":
                            questions = Constants.animeQuestions;
                            break;
                        case "songs":
                            questions = Constants.musicQuestions;
                            break;
                        default:
                            await e.Channel.SendMessage("Genre not know to this electromaster :/");
                            return;
                    }
                }
                questionsLeft = Math.Min(amount, 10);
                running = true;
                channel = e.Channel;
                await channel.SendMessage("Quiz start! Get ready for " + questionsLeft + " questions!");
                await NextQuestion();
                return;
            }
            await e.Channel.SendMessage("Something went wrong with your arguments XD");
        }

        public async Task Handle(MessageEventArgs e)
        {
            if (e.User.IsBot) return;
            if (e.Message.Text.Length <= 0) return;
            if (e.Message.Text.Length <= 0) return;
            if (!currentQuestion.IsAnswer(e.Message.Text)) return;
            await channel.SendMessage(e.User.Name + " has a correct answer, namely: '" + e.Message.Text + "'");
            await NextQuestion();
            game.GetUser(e.User.Id, e.User.Name).AddPoints(20);
        }

        private async Task NextQuestion()
        {
            if (questionsLeft <= 0 || !running)
            {
                await channel.SendMessage("Well played everyone, the game is now over!");
                running = false;
                return;
            }
            questionsLeft--;
            currentQuestion = questions.ElementAt(MyBot.rng.Next(questions.Count()));
            System.Threading.Thread.Sleep(1000);
            await channel.SendMessage("Next question:\n" + currentQuestion.question);
        }
    }
}
