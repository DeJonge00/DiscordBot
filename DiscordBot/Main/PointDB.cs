using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main
{
    [Serializable]
    class PointDB
    {
        public string name;
        public ulong id;
        public int points;

        public PointDB(ulong id, string name)
        {
            this.name = name;
            this.id = id;
            this.points = 0;
        }
    }
}
