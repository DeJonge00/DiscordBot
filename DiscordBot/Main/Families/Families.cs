using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.Families
{
    class Families
    {
        private List<Family> families;
        private List<Proposal> proposals;

        public Families(CommandService commands)
        {
            proposals = new List<Proposal>();
            LoadFamilies();

            commands.CreateCommand("adopt")
                .Description("\n\tAdopt a kid")
                .Parameter("param", ParameterType.Unparsed);

            commands.CreateCommand("divorce")
                .Description("\n\tDivorce your lover")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Divorce(e));

            commands.CreateCommand("marry")
                .Description("\n\tMarry someone")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async (e) => await Marry(e));
        }

        private async Task Divorce(Discord.Commands.CommandEventArgs e)
        {
            Family f;
            if ((f = GetFamily(e.User)) == null)
            {
                await e.Channel.SendMessage("It appears you are not loved enough to get married in the first place");
                return;
            }
            families.Remove(f);
            await e.Channel.SendMessage("You are now officially single");
            await f.so(e.User).SendMessage("Your partner has divorced you, get REKT");
        }

        private Family GetFamily(User u)
        {
            foreach (Family m in families)
                if (m.a == u || m.b == u) return m;
            return null;
        }

        public void HandleProposal(MessageEventArgs e)
        {
            foreach(Proposal p in proposals)
            {
                if (p.a.Id == e.User.Id)
                {
                    string[] accept = { "i do", "ido", "i accept", "accept", "yes", "yus", "ok" };
                    if (accept.Contains(e.Message.Text.ToLower()))
                    {
                        families.Add(new Family(p.a, p.b));
                        p.c.SendMessage("Sounded good enough!\n" + p.a.Name + " and " + p.b.Name + " are now officially married!!!");
                    }
                    else
                    {
                        p.c.SendMessage("Sounded like you got rejected really bad " + p.b.Name + "!\nHahaha :smile:");
                    }
                    proposals.Remove(p);
                    break;
                }
            }
        }

        private void LoadFamilies()
        {
            try
            {
                using (Stream stream = File.Open(Constants.marryFile, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    families = (List<Family>)bformatter.Deserialize(stream);
                }
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") LOADED MARIAGES");
            }
            catch (Exception e)
            {
                families = new List<Family>();
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") LOADED MARIAGES (reset)");
            }
        }

        private async Task Marry(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            Family m;
            if ((m = GetFamily(e.User)) != null)
                await e.Channel.SendMessage("You are currently married to **" + m.so(e.User).Name + "**!");

            if (e.Message.MentionedUsers.Count() <= 0)
            {
                await e.Channel.SendMessage("You are currently a lonely single!");
                return;
            }
            User other = e.Message.MentionedUsers.ElementAt(0);
            if (e.User == other)
            {
                await e.Channel.SendMessage("So it seems like nobody wants you?");
                return;
            }
            if ((m = GetFamily(other)) != null)
            {
                await e.Channel.SendMessage("The person you are trying to marry is currently married to **" + m.so(other) + "**!");
                return;
            }
            // Get married
            proposals.Add(new Proposal(other, e.User, e.Channel));
            await e.Channel.SendMessage(other.Mention + ", do you want to marry " + e.User.Name + "?\nSay \"I do\" to accept!");
        }

        private void SaveFamilies()
        {
            try
            {
                //serialize
                using (Stream stream = File.Open(Constants.marryFile, FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bformatter.Serialize(stream, families);
                }
                Console.WriteLine(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") SAVED MARRIAGES");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in saving");
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void Quit()
        {
            SaveFamilies();
        }
    }
}
