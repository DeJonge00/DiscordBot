﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    [Serializable]
    abstract class RPGCharacter
    {
        public double health { get; protected set; }
        public double maxHealth { get; protected set; }
        public double armor { get; protected set; }
        public double damage { get; protected set; }
        public double weaponskill { get; protected set; }

        protected RPGCharacter(int health, int armor, int damage, int weaponskill)
        {
            this.health = health;
            maxHealth = health;
            this.armor = armor;
            this.damage = damage;
            this.weaponskill = weaponskill;
        }

        public void AddHealth(int i)
        {
            if (i < -10000 || i > 10000)
            {
                Console.WriteLine("AddHealth: amount out of bounds");
                return;
            }
            health = Math.Max(0, Math.Min(maxHealth, health + i));
        }

        public void MultiplyHealth(double i)
        {
            if (i < 0.5 || i > 2)
            {
                Console.WriteLine("MultiplyHealth: amount out of bounds");
                return;
            }
            health = Math.Max(0, Math.Min(maxHealth, health*i));
        }
    }
}
