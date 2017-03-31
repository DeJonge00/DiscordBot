using System.Linq;
using System.Threading.Tasks;
using Discord;
using System.IO;
using System.Net;
using System;

namespace DiscordBot.Main
{
    class MessageHandler
    {
        private int counter = 0;

        private int sunLock;
        private DateTime suntime;
        private int kysLock;
        private int dedLock;
        private int helloLock;
        private int burnLock;

        public MessageHandler()
        {
            sunLock = -1;
            suntime = new DateTime(0);
            kysLock = -1;
            dedLock = -1;
            helloLock = -1;
            burnLock = -1;
        }

        internal async Task Handle(MessageEventArgs e)
        {
            if (Constants.user != "NYA") return;
            if (e.Message.Text.Split(' ')[0] == "\\o/")
            {
                await PraiseTheSun(e);
                return;
            }
            if (e.Message.IsMentioningMe() || !(e.Message.Text.Length <= 0 || !char.IsLetter(e.Message.Text.First())))
            {
                string[] srvrwhite = { "test", "9CHAT" };
                //Message length limit
                /*if(srvrwhite.Contains(e.Server.Name) && e.Channel.Name != "creepypastas" && e.Message.Text.Length > 1000)
                {
                    await e.Message.Delete();
                    return;
                }*/

                // Caps limit
                /*double count = 0;
                ulong[] white = { Constants.NYAid, Constants.CATEid };
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
                }*/

                if (e.Message.User.Id == Constants.WIZZid)
                    return;
                if (e.Message.Text.ToLower() == "ayy" && e.User.Id == Constants.NYAid)
                {
                    await e.Channel.SendMessage("lmao");
                }
                /*
                if (e.Message.Text.ToLower() == "ded" || e.Message.Text.ToLower() == "*ded*" && (e.Server.Name != "9CHAT" || e.User.Id == Constants.NYAid))
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
                if ((e.Message.Text.ToLower() == "kys" || e.Message.Text.ToLower() == "kill yourself") && (e.Server.Name != "9CHAT" || MyBot.rng.Next(100) < 25) && !e.Message.IsMentioningMe())
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
                string[] greetings = { "hello", "hi", "hai" };
                if (greetings.Contains(e.Message.Text.Split()[0].ToLower()) && /*(e.Server.Name != "9CHAT" || e.User.Id == Constants.NYAid))
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
                }*/
                if (e.Message.Text.ToLower().Split(' ').Contains("lenny"))
                {
                    await e.Message.Channel.SendMessage("( ͡° ͜ʖ ͡°)");
                    var str = e.Message.Timestamp.ToShortTimeString() + " - " + e.Channel.Name + ") " + e.User.Name + " said: " + e.Message.Text;
                    MyBot.Log(str, e.Server.Name);
                    return;
                }
                
                // Mentioned biribiri
                if (!char.IsSymbol(e.Message.Text[0]) && (e.Message.MentionedUsers.Count() > 0 && e.Message.IsMentioningMe() || e.Message.Text.ToLower().Split(' ').Contains("biribiri") || e.Message.Text.ToLower().Split(' ').Contains("biri") || e.Message.Text.ToLower().Split(' ').Contains("biri,") || e.Message.Text.ToLower().Split(' ').Contains("biribiri,")))
                {
                    var text = e.Message.Text.ToLower().Split(' ');
                    string returnstr = "NO!";
                    if (text.Contains("thanks"))
                    {
                        returnstr = "My pleasure :heart:";
                        await e.Channel.SendMessage(returnstr);
                        return;
                    }
                    if (text.Contains("kys")) {
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

            if(e.Message.Attachments.Count() > 0 && e.User.Id != Constants.NYAid)
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
                SaveFile(@"F:\DiscordBot\stats\", counter.ToString() + ext, e.Message.Attachments.ElementAt(0).Url);
            }
        }

        private async Task PraiseTheSun(MessageEventArgs e)
        {
            await e.Message.Delete();
            var time = e.Message.Timestamp;
            if (time.Subtract(suntime).Minutes < 5) return;
            suntime = time;
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

        public static void SaveFile(string p, string name, string url)
        {
            var path = Path.Combine(p, name);

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
