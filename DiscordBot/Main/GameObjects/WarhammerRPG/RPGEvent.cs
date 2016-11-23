using Discord;
using System.Threading.Tasks;

namespace DiscordBot.Main.GameObjects.WarhammerRPG
{
    static class RPGEvent
    {
        public static async Task Battle(Channel c, PointDB host, PointDB opponent)
        {
            var result = "**Brawl between " + host.name + " and " + opponent.name + "**\n";
            host.cooldown = 5;
            var p1 = host;
            var p2 = opponent;
            PointDB p3;
            while(p1.health > 0 || p2.health > 0)
            {
                if(MyBot.rng.Next((int)(p1.weaponSkill+p2.weaponSkill)) < p1.weaponSkill)
                {
                    var damage = MyBot.rng.Next(10, 20) * p1.strength / p2.toughness;
                    p2.DecreaseHealth(damage);
                    result += "\n" + p1.name + " attacked for **" + damage + "**";
                }
                p3 = p1;
                p1 = p2;
                p2 = p3;
            }
            result += "\n\nThe battle is over, " + p2.name + " laughs while walking away from " + p1.name + "'s corpse";
            if (host.id == p1.id)
            {
                host.AddExp(5);
                opponent.AddExp(20);
            }
            else
            {
                host.AddExp(20);
                opponent.AddExp(5);
            }
                await c.SendMessage(result);
        }

        public static async Task Patrol(Channel c, PointDB player)
        {
            string[] enemies = { "rebellious gnoblars", "greenskins", "mountain giants" };
            var damage = MyBot.rng.Next(player.level * 10, player.level * 50);
            player.DecreaseHealth(damage);
            string enemy;
            if (damage < player.level * 20)
                enemy = enemies[0];
            else if (damage < player.level * 40)
                enemy = enemies[1];
            else enemy = enemies[2];
            await c.SendMessage(player.name + " took " + damage + " damage from being attacked by " + enemy);
            player.AddExp(10);
            player.cooldown = 10;
        }
    }
}
