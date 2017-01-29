using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    class RPGMonster : RPGCharacter
    {
        public RPGMonster(int health, int armor, int damage, int weaponskill) : base(health, armor, damage, weaponskill)
        {
        }
    }
}
