using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main
{
    class Family
    {
        public User a;
        public User b;
        public int points;
        public DateTime wedding;
        public List<User> kids;

        public Family(User f, User s)
        {
            a = f;
            b = s;
            points = 0;
            wedding = DateTime.Now;
            kids = new List<User>();
        }

        public User so(User u)
        {
            if (a == u) return b;
            return b;
        }
    }
}
