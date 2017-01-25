using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    [Serializable]
    abstract class RPGCharacter
    {
        public int health { get; protected set; }
        public int maxHealth { get; protected set; }
        public int armor { get; protected set; }
        public int damage { get; protected set; }

        protected RPGCharacter(int health, int armor, int damage)
        {
            this.health = health;
            maxHealth = health;
            this.armor = armor;
            this.damage = damage;
        }

        public void AddHealth(int i)
        {
            if (i > 0 && i < 100)
            {
                health = Math.Max(0, Math.Min(maxHealth, health + i));
            }
            else
            {
                Console.WriteLine("AddHealth: amount out of bounds");
            }
        }
    }
}
