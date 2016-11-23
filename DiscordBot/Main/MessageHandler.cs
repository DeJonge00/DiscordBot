
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.IO;
using System.Threading;

namespace DiscordBot.Main
{
    class MessageHandler
    {
        public Game game;
        public bool running;
        private string statsFile = Path.Combine(@"F:\DiscordBot\stats", "interaction.bin");
        public List<BiriInteraction> users;
        private Thread runningThread;

        public MessageHandler(CommandService commands, Game g)
        {
            game = g;
            running = true;

            // Load interaction DB
            try
            {
                using (Stream stream = File.Open(statsFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    users = (List<BiriInteraction>)bformatter.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            if (users == null) users = new List<BiriInteraction>();

            // Start saving
            runningThread = new Thread(new ThreadStart(SaveInterval));
            runningThread.Start();
        }

        internal async Task Handle(MessageEventArgs e)
        {
            if (!(e.Message.Text.Length <= 0 || Constants.BOTprefix.Contains(e.Message.Text.First()) || Constants.BOTids.Contains(e.Message.User.Id)))
            {
                game.GetUser(e.User.Id, e.User.Name).AddPoints(10);
                var player = game.GetUser(e.Message.User.Id, e.Message.User.Name);

                if (e.Message.Text.ToLower().Split(' ').Contains("ded") || e.Message.Text.ToLower().Split(' ').Contains("*ded*") && (e.Server.Name != "9CHAT" || e.User.Id == Constants.NYAid))
                {
                    Console.WriteLine(e.User.Name + " said 'ded'");
                    await e.Message.Channel.SendMessage(Responses.ded[MyBot.rng.Next(Responses.ded.Length)]);
                    return;
                }
                if ((e.Message.Text.ToLower().Split(' ').Contains("kys") || e.Message.Text.ToLower() == "kill yourself") && (e.Server.Name != "9CHAT" || MyBot.rng.Next(100) < 10) && !e.Message.IsMentioningMe())
                {
                    await e.Message.Channel.SendMessage(Responses.kys[MyBot.rng.Next(Responses.kys.Length)]);
                    Console.WriteLine(e.User.Name + " said 'kys'");
                    return;
                }
                if (e.Message.Text.ToLower().Split(' ').Contains("lenny"))
                {
                    await e.Message.Channel.SendMessage("( ͡° ͜ʖ ͡°)");
                    Console.WriteLine(e.User.Name + " said 'lenny'");
                    return;
                }
                string[] greetings = { "hello", "hi", "hai" };
                if (greetings.Contains(e.Message.Text.Split()[0].ToLower()) && (e.Server.Name != "9CHAT" || e.User.Id == Constants.NYAid))
                {
                    var s = Responses.hello[MyBot.rng.Next(Responses.hello.Length)];
                    if (e.Message.User.Id == Constants.NYAid) s += " :heart:";
                    await e.Message.Channel.SendMessage(s);
                    Console.WriteLine(e.User.Name + " said 'hi'");
                    return;
                }
                if(e.Message.Text.Split(' ')[0] == "\\o/")
                {
                    await PraiseTheSun(e);
                    return;
                }
                // Mentioned biribiri
                if (e.Message.MentionedUsers.Count() > 0 && e.Message.IsMentioningMe() || e.Message.Text.ToLower().Split(' ').Contains("biribiri") || e.Message.Text.ToLower().Split(' ').Contains("biri"))
                {
                    var text = e.Message.Text.ToLower().Split(' ');
                    var u = GetUser(e.User.Id, e.User.Name);
                    if (text.Contains("love") || text.Contains("sweet") || text.Contains("nice") || text.Contains("good") || text.Contains(":heart:") || text.Contains("like"))
                    {
                        u.AddKarma(1);
                        return;
                    }
                    if (text.Contains("death") || text.Contains("kill") || text.Contains("gross") || text.Contains("bad"))
                    {
                        u.AddKarma(-1);
                        return;
                    }
                    if (text.Contains("thanks"))
                    {
                        u.AddKarma(2);
                        await e.Channel.SendMessage("My pleasure" + " " + AddKarmaString(u));
                        return;
                    }
                    if (text.Contains("kys")) {
                        u.AddKarma(-5);
                        await e.Channel.SendMessage(Responses.kys[MyBot.rng.Next(Responses.kys.Count())]);
                        await e.User.SendMessage(Responses.burn[MyBot.rng.Next(Responses.burn.Count())] + " " + AddKarmaString(u));
                        return;
                    }
                    if(e.Message.Text[e.Message.Text.Length-1] == '?')
                    {
                        await e.Channel.SendMessage(Responses.qa[MyBot.rng.Next(Responses.qa.Count())] + " " + AddKarmaString(u));
                        return;
                    }
                    await e.Channel.SendMessage(Responses.response[MyBot.rng.Next(Responses.response.Count())] + " " + AddKarmaString(u));
                }
            }
            if (e.Message.User.Id == Constants.WIZZid)
            {
                if (MyBot.rng.Next(100) < 1) await e.Message.Channel.SendMessage(":heart:");
            }
        }

        public void Abort()
        {
            running = false;
            runningThread.Abort();
        }

        public string AddKarmaString(BiriInteraction u)
        {
            var karma = Math.Max(-2, Math.Min(2, u.karmaLevel));
            if (u.id == Constants.NYAid)
                return Constants.karmaResponse[0];
            return Constants.karmaResponse[karma + 3];
        }

        public BiriInteraction GetUser(ulong id, string name)
        {
            // Check if user is in the DB
            BiriInteraction user = null;
            for (int i = 0; i < users.Count; i++)
            {
                if (id == users[i].id)
                {
                    user = users[i];
                }
            }
            // Create new entry in database
            if (user == null)
            {
                user = new BiriInteraction(id, name);
                users.Add(user);
            }
            return user;
        }

        private async Task PraiseTheSun(MessageEventArgs e)
        {
            await e.Message.Delete();
            Console.WriteLine("\\o/ command used by " + e.User);
            var i = MyBot.rng.Next(Responses.sun.Length);
            for (int a = 0; a < e.Message.MentionedUsers.Count(); a++)
                if (e.Message.MentionedUsers.ElementAt(a).Id == Constants.TRISTANid)
                    i = 3;
            var str = Responses.sun[i];
            await e.Channel.SendFile(str);
        }

        public async Task Reset(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            if (e.User.Id == Constants.NYAid)
            {
                users = new List<BiriInteraction>();
            }
            else
            {
                await e.Channel.SendMessage("Hahahaha, no.");
            }
        }

        private void SaveInterval()
        {
            while (running)
            {
                Save(0);
                System.Threading.Thread.Sleep(15 * 60 * 1000);
            }
        }

        public void Save(ulong id)
        {
            if (id == Constants.NYAid || id == 0)
            {
                Console.WriteLine("SAVING STATS");
                try
                {
                    //serialize
                    using (Stream stream = File.Open(statsFile, FileMode.Create))
                    {
                        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        bformatter.Serialize(stream, users);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                    Console.WriteLine("Error in saving");
                }
            }
        }
    }
}
