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
        public int farmingLeft { get; private set; }

        public RPGPlayer(Discord.User u) : base (100, 0, 10, 10) 
        {
            name = u.Name;
            id = u.Id;
            playerclass = "Peasant";
        }

        public void AddDamage(int i)
        {
            if (i > 0 && i < 100)
            {
                damage += i;
            }
            else
            {
                Console.WriteLine("AddDamage: amount out of bounds");
            }
        }

        public void AddExp(int i)
        {
            if (i < 0 || i > 100)
            {
                Console.WriteLine("AddExp: amount out of bounds");
                return;
            }
            var l = GetLevel();
            exp += i;
            if(GetLevel() > l)  // Level up
            {
                damage = Math.Floor(damage*1.2);
                weaponskill += 2;
                maxHealth += 25;
            }
        }

        public void AddMaxHealth(double i)
        {
            if (i < 0 || i > 100)
            {
                Console.WriteLine("AddMaxHealth: amount out of bounds");
                return;
            }
            var p = maxHealth / health;
            maxHealth += i;
            MultiplyHealth(p);
        }

        public void AddMoney(int i)
        {
            if (i > -500 && i < 500)
            {
                money += i;
            }
            else
            {
                Console.WriteLine("AddMoney: amount out of bounds");
            }
        }

        public void AddWeaponSkill(int i)
        {
            if (i > 0 && i < 100)
            {
                weaponskill += i;
            }
            else
            {
                Console.WriteLine("AddWeaponSkill: amount out of bounds");
            }
        }

        public int CompareTo(RPGPlayer other)
        {
            return exp.CompareTo(other.exp);
        }

        public void DecrementFarming()
        {
            farmingLeft--;
            if(farmingLeft <= 0)
            {
                farmingLeft = 0;
            }
        }

        public int GetLevel()
        {
            return exp / 100;
        }

        public bool IsFarming()
        {
            return farmingLeft < 0;
        }

        public void SetClass(string c)
        {
            string[] classes =
            {
                "Peasant"
            };
            if (!classes.Contains(c))
            {
                Console.WriteLine("SetClass: Invalid class");
                return;
            }
            playerclass = c;
        }

        public void SetFarming(int i)
        {
            if(i < 1 || i > 120)
            {
                Console.WriteLine("SetFarming: amount out of bounds");
                return;
            }
            farmingLeft = i;
        }

        public void UpdateName(string n)
        {
            name = n;
        }
    }
}
