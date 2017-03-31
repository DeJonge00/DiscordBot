using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main
{
    class Proposal
    {
        public User a;
        public User b;
        public Channel c;

        public Proposal(User f, User s, Channel ch)
        {
            a = f;
            b = s;
            c = ch;
        }
    }
}
