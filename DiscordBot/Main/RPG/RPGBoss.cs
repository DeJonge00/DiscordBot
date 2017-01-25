using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    class RPGBoss : RPGCharacter
    {
        public int level { get; protected set; }

        public RPGBoss(int level, int health, int armor, int damage) : base (health, armor, damage) 
        {
            this.level = level;
        }
    }
}
