using Discord;
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
        public string name                      { get; private set; }
        public ulong id                         { get; private set; }
        public string race                      { get; private set; }
        public int points                       { get; private set; }
        public int level                        { get; private set; }
        public int exp                          { get; private set; }
        public int maxExp                       { get; private set; }
        public int cooldown;
        public int patrolling;
        // Battle stats
        public int health                       { get; private set; }
        public int maxHealth                    { get; private set; }
        public int weaponSkill                  { get; private set; }
        public int strength                     { get; private set; }
        public int toughness                    { get; private set; }
        public int attackSpeed                  { get; private set; }
        // Bonus
        public double pointBonus                { get; private set; }
        public double regenBonus                { get; private set; }

        public PointDB(ulong i, string n)
        {
            name = n;
            id = i;
            race = "Ogres";
            points = 0;
            level = 1;
            exp = 0;
            maxExp = level * 100;
            cooldown = 0;
            patrolling = 0;
            var x = MyBot.rng.Next(15);
            health = 80 + 2*x;
            maxHealth = 80 + 2*x;
            var x2 = MyBot.rng.Next(x, 20);
            weaponSkill = 5 + x2;
            x = MyBot.rng.Next(x2, 25);
            strength = 5 + x;
            x2 = 25 - x;
            toughness = 5 + x2;
            attackSpeed = 1;
            pointBonus = 1;
            regenBonus = 1;
        }

        public void AddPoints(int n)
        {
            if(n < 0 || n > 1000)
            {
                Console.WriteLine(name + " tried to increase his points with more than 1000 or less than 0 points");
                return;
            }
            points += (int)(n*pointBonus);
        }

        public void SubtractPoints(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to decrease his points with more than 100 or less than 0 points");
                return;
            }
            points -= n;
        }

        public void AddExp(int n)
        {
            if (n < 0 || n > 1000)
            {
                Console.WriteLine(name + " tried to increase his exp with more than 1000 or less than 0 points");
                return;
            }
            exp += n;
            if (exp >= maxExp)
                IncreaseLevel();
        }

        public void SubtractExp(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to decrease his exp with more than 100 or less than 0 points");
                return;
            }
            exp -= n;
            if(exp < (maxExp - (level * 100)))
            {
                maxExp = maxExp - (level * 100);
                level--;
            }
        }

        public void IncreaseLevel()
        {
            if (exp < maxExp)
                exp = maxExp;
            level++;
            maxExp += level * 100;
            IncreaseMaxHealth(20);
            IncreaseStrength(2);
            IncreaseToughness(2);
            IncreaseWeaponskill(2);
            IncreaseSpeed(1);
            if (points >= maxExp)
                IncreaseLevel();
        }

        public void IncreaseHealth(int n)
        {
            if (n < 0 || n > 1000)
            {
                Console.WriteLine(name + " tried to increase his health with more than 1000 or less than 0 points");
                return;
            }
            if (health + n <= maxHealth)
                health += n;
            else health = maxHealth;
        }

        public void DecreaseHealth(int n)
        {
            if (n < 0 || n > 1000)
            {
                Console.WriteLine(name + " tried to decrease his health with more than 1000 or less than 0 points");
                return;
            }
            if (health - n > 0)
            {
                health -= n;
            }
            else
            {
                health = 0;
                points = (int)(points * 0.5);
            }
        }

        public void IncreaseMaxHealth(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to increase his maxhealth with more than 100 or less than 0 points");
                return;
            }
            var m = maxHealth / health;
            maxHealth += n;
            health = maxHealth / m;
        }

        public void DecreaseMaxHealth(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to decrease his maxhealth with more than 100 or less than 0 points");
                return;
            }
            if(n >= maxHealth)
            maxHealth -= n;
        }

        public void IncreaseWeaponskill(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to increase his WS with more than 100 or less than 0");
                return;
            }
            weaponSkill += n;
        }

        public void DecreaseWeaponskill(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to decrease his WS with more than 100 or less than 0");
                return;
            }
            weaponSkill -= n;
        }

        public void IncreaseStrength(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to increase his strength with more than 100 or less than 0");
                return;
            }
            strength += n;
        }

        public void DecreaseStrength(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to decrease his strength with more than 100 or less than 0");
                return;
            }
            strength -= n;
        }

        public void IncreaseToughness(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to increase his toughness with more than 100 or less than 0");
                return;
            }
            toughness += n;
        }

        public void DecreaseToughness(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to decrease his toughness with more than 100 or less than 0");
                return;
            }
            toughness -= n;
        }

        public void IncreaseSpeed(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to increase his speed with more than 100 or less than 0");
                return;
            }
            attackSpeed += n;
        }

        public void DecreaseSpeed(int n)
        {
            if (n < 0 || n > 100)
            {
                Console.WriteLine(name + " tried to decrease his speed with more than 100 or less than 0");
                return;
            }
            attackSpeed -= n;
        }
    }
}
