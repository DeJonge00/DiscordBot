
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.IO;
using System.Threading;
using System.Net;

namespace DiscordBot.Main
{
    class MessageHandler
    {
        public Game game;
        public bool running;
        private string statsFile = @"F:\DiscordBot\stats\interaction.bin";
        public List<BiriInteraction> users;
        private Thread runningThread;
        private int counter = 0;

        private int sunLock = -1;
        private int kysLock = -1;
        private int dedLock = -1;
        private int helloLock = -1;
        private int burnLock = -1;

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
            if (!(e.Message.Text.Length <= 0 || Constants.BOTprefix.Contains(e.Message.Text.First()) || e.User.IsBot))
            {
                game.GetUser(e.User.Id, e.User.Name).AddPoints(10);
                var player = game.GetUser(e.Message.User.Id, e.Message.User.Name);
                //Message length limit
                string[] srvrwhite = { "test", "9CHAT" };
                /*if(srvrwhite.Contains(e.Server.Name) && e.Channel.Name != "creepypastas" && e.Message.Text.Length > 1000)
                {
                    await e.Message.Delete();
                    return;
                }*/

                // Caps limit
                double count = 0;
                ulong[] white = { Constants.NYAid, Constants.CATEid, Constants.WIZZid };
                if (srvrwhite.Contains(e.Server.Name) && !white.Contains(e.User.Id))
                {
                    foreach (char c in e.Message.Text)
                    {
                        if (Char.IsUpper(c))
                            count++;
                    }
                    
                    var nameUps = 0;
                    foreach(User u in e.Message.MentionedUsers)
                    {
                        foreach (char c in u.Name)
                        {
                            if (Char.IsUpper(c))
                                nameUps++;
                        }
                    }
                    count -= nameUps;
                    if (count >= 10 && count / (e.Message.Text.Length-nameUps) > 0.5)
                    {
                        await e.Message.Delete();
                        //var m = await e.Channel.SendMessage(e.User.Mention + " can you stop spamming caps pls?");
                        //System.Threading.Thread.Sleep(3000);
                        //await m.Delete();
                        return;
                    }
                }


                if (e.Message.Text.ToLower().Split(' ').Contains("ded") || e.Message.Text.ToLower().Split(' ').Contains("*ded*") && (e.Server.Name != "9CHAT" || e.User.Id == Constants.NYAid))
                {
                    var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + " said: " + e.Message.Text;
                    MyBot.Log(str, e.Server.Name);
                    int i;
                    do
                    {
                        i = MyBot.rng.Next(Responses.ded.Length);
                    } while (i == dedLock);
                    dedLock = i;
                    await e.Message.Channel.SendMessage(Responses.ded[i]);
                    return;
                }
                if ((e.Message.Text.ToLower().Split(' ').Contains("kys") || e.Message.Text.ToLower() == "kill yourself") && (e.Server.Name != "9CHAT" || MyBot.rng.Next(100) < 25) && !e.Message.IsMentioningMe())
                {
                    int i;
                    do
                    {
                        i = MyBot.rng.Next(Responses.kys.Length);
                    } while (i == kysLock);
                    kysLock = i;
                    await e.Message.Channel.SendMessage(Responses.kys[i]);
                    var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + " said: " + e.Message.Text;
                    MyBot.Log(str, e.Server.Name);
                    return;
                }
                if (e.Message.Text.ToLower().Split(' ').Contains("lenny"))
                {
                    await e.Message.Channel.SendMessage("( ͡° ͜ʖ ͡°)");
                    var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + " said: " + e.Message.Text;
                    MyBot.Log(str, e.Server.Name);
                    return;
                }
                string[] greetings = { "hello", "hi", "hai" };
                if (greetings.Contains(e.Message.Text.Split()[0].ToLower()) && (e.Server.Name != "9CHAT" || e.User.Id == Constants.NYAid))
                {
                    int i;
                    do
                    {
                        i = MyBot.rng.Next(Responses.hello.Length);
                    } while (i == helloLock);
                    helloLock = i;
                    var s = Responses.hello[i];
                    if (e.Message.User.Id == Constants.NYAid) s += " :heart:";
                    await e.Message.Channel.SendMessage(s);
                    var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + " said: " + e.Message.Text;
                    MyBot.Log(str, e.Server.Name);
                    return;
                }
                if(e.Message.Text.Split(' ')[0] == "\\o/")
                {
                    await PraiseTheSun(e);
                    return;
                }
                // Mentioned biribiri
                if (e.Message.MentionedUsers.Count() > 0 && e.Message.IsMentioningMe() || e.Message.Text.ToLower().Split(' ').Contains("biribiri") || e.Message.Text.ToLower().Split(' ').Contains("biri") || e.Message.Text.ToLower().Split(' ').Contains("biri,") || e.Message.Text.ToLower().Split(' ').Contains("biribiri,"))
                {
                    var text = e.Message.Text.ToLower().Split(' ');
                    string returnstr = "NO!";
                    var u = GetUser(e.User.Id, e.User.Name);
                    string[] nice = { "love", "sweet", "nice", "good", ":heart:", "like" };
                    string[] bad = { "death", "kill", "gross", "bad" };
                    for(int i = 0; i < text.Count(); i++)
                    {
                        if (nice.Contains(text.ElementAt(i)))
                            u.AddKarma(1);
                        if (bad.Contains(text.ElementAt(i)))
                            u.AddKarma(-1);
                    }
                    if (text.Contains("thanks"))
                    {
                        u.AddKarma(2);
                        returnstr = "My pleasure :heart:";
                        await e.Channel.SendMessage(returnstr);
                        return;
                    }
                    if (text.Contains("kys")) {
                        u.AddKarma(-5);
                        int i;
                        do
                        {
                            i = MyBot.rng.Next(Responses.kys.Length);
                        } while (i == kysLock);
                        kysLock = i;
                        await e.Channel.SendMessage(Responses.kys[i]);
                        do
                        {
                            i = MyBot.rng.Next(Responses.burn.Length);
                        } while (i == burnLock);
                        burnLock = i;
                        await e.User.SendMessage(Responses.burn[i]);
                        return;
                    }
                    if(e.Message.Text[e.Message.Text.Length-1] == '?')
                    {
                        await e.Channel.SendMessage(Responses.qa[MyBot.rng.Next(Responses.qa.Count())]);
                        return;
                    }
                    returnstr = Responses.response[MyBot.rng.Next(Responses.response.Count())];
                    await e.Channel.SendMessage(returnstr);
                }
            }
            if(e.Message.Attachments.Count() > 0 && !Constants.BOTids.Contains(e.User.Id) && e.User.Id != Constants.NYAid)
            {
                var ext = ".jpg";
                if (e.Message.Attachments.ElementAt(0).Url.EndsWith(".gif"))
                    ext = ".gif";
                while(File.Exists(Path.Combine(@"F:\DiscordBot\stats\", counter.ToString() + ext)))
                {
                    counter++;
                }
                var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") Saving " + e.User.Name + "'s picture as " + counter + ext;
                MyBot.Log(str, e.Server.Name);
                SaveFile(counter.ToString() + ext, e.Message.Attachments.ElementAt(0).Url);
            }
            if(e.User.Id == Constants.BONFIREid && e.Message.Text.Length > 25)
            {
                int i;
                var s = e.Message.Text.Split(' ');
                if (!Int32.TryParse(s.ElementAt(s.Count()-3), out i))
                {
                    return;
                }
                if(i%100 == 0)
                {
                    await e.Channel.SendMessage("Congratulations on your nolife behaviour!!!");
                    return;
                }
                if(i%25 == 0)
                {
                    await e.Channel.SendMessage("Whyyyy person? Why dont you like **me** more?");
                    return;
                }
            }
        }

        public void Abort()
        {
            Save(0);
            running = false;
            runningThread.Abort();
        }

        public string AddKarmaString(BiriInteraction u)
        {
            var karma = Math.Max(-2, Math.Min(3, u.karmaLevel));
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
            int i;
            do
            {
                i = MyBot.rng.Next(Responses.sun.Length);
            } while (i == sunLock);
            sunLock = i;
            for (int a = 0; a < e.Message.MentionedUsers.Count(); a++)
                if (e.Message.MentionedUsers.ElementAt(a).Id == Constants.TRISTANid)
                    i = 3;
            var str = Responses.sun[i];
            try
            {
                await e.Channel.SendFile(str);
            } catch
            {

            }
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
                System.Threading.Thread.Sleep(15 * 60 * 1000);
                Save(0);
            }
        }

        public void Save(ulong id)
        {
            if (id == Constants.NYAid || id == 0)
            {
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") SAVING INTERACTION STATS");
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

        private void SaveFile(string name, string url)
        {
            var path = Path.Combine(@"F:\DiscordBot\stats\", name);

            byte[] imageBytes;
            HttpWebRequest imageRequest = (HttpWebRequest)WebRequest.Create(url);
            WebResponse imageResponse = imageRequest.GetResponse();

            Stream responseStream = imageResponse.GetResponseStream();

            using (BinaryReader br = new BinaryReader(responseStream))
            {
                imageBytes = br.ReadBytes(2000000);
                br.Close();
            }
            responseStream.Close();
            imageResponse.Close();

            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            try
            {
                bw.Write(imageBytes);
            }
            finally
            {
                fs.Close();
                bw.Close();
            }
        }
    }
}
