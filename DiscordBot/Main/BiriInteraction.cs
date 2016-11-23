using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main
{
    [Serializable]
    class BiriInteraction
    {
        public string name { get; private set; }
        public ulong id { get; private set; }
        public int karma { get; private set; }
        public int karmaLevel { get; private set; }

        public BiriInteraction(ulong i, string n)
        {
            name = n;
            id = i;
            karma = 0;
            karmaLevel = 0;
        }

        public void AddKarma(int n)
        {
            karma += n;
        }

        public void IncrementLevel()
        {
            karmaLevel++;
            if (karma >= karmaLevel * 10)
                IncrementLevel();
        }

        public void DecrementLevel()
        {
            karmaLevel--;
            if (karma <= karmaLevel * 10)
                DecrementLevel();
        }
    }
}
