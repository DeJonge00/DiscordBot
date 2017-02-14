using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.Music
{
    class Song
    {
        public string path;
        public string title;

        public Song(string path, string name)
        {
            this.path = path;
            title = name;
        }
    }
}
