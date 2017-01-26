using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    [Serializable]
    class RPGPlayer : RPGCharacter
    {
        public string name { get; private set; }
        public ulong id { get; private set; }
        public string playerclass { get; private set; }
        public int exp { get; private set; }
        public int money { get; private set; }

        public RPGPlayer(Discord.User u) : base (100, 0, 10) 
        {
            name = u.Name;
            id = u.Id;
            playerclass = "peasant";
        }

        public void AddExp(int i)
        {
            if (i > 0 && i < 100)
            {
                exp += i;
            }
            else
            {
                Console.WriteLine("AddExp: amount out of bounds");
            }
        }

        public void AddMaxHealth(int i)
        {
            if (i > 0 && i < 100)
            {
                maxHealth += i;
            }
            else
            {
                Console.WriteLine("AddMaxHealth: amount out of bounds");
            }
        }

        public void AddMoney(int i)
        {
            if (i > 0 && i < 100)
            {
                money += i;
            }
            else
            {
                Console.WriteLine("AddMoney: amount out of bounds");
            }
        }

        public int CompareTo(RPGPlayer other)
        {
            return exp.CompareTo(other.exp);
        }

        public int GetLevel()
        {
            return exp / 100;
        }

        public void UpdateName(string n)
        {
            name = n;
        }
    }
}
