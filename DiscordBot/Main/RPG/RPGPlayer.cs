using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    [Serializable]
    class RPGPlayer : IComparable<RPGPlayer>
    {
        public string name { get; private set; }
        public ulong id { get; private set; }
        public string playerclass { get; private set; }
        public int exp { get; private set; }
        public int money { get; private set; }
        public int health { get; private set; }
        public int maxHealth { get; private set; }
        public int armor { get; private set; }
        public int damage { get; private set; }

        public RPGPlayer(Discord.User u)
        {
            name = u.Nickname;
            id = u.Id;
            playerclass = "peasant";
            health = 100;
            armor = 0;
            damage = 10;
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

        public void UpdateName(string n)
        {
            name = n;
        }
    }
}
