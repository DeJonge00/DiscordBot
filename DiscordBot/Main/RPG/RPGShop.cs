using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    class RPGShop
    {
        private DiscordClient client;
        private CommandService commands;

        public RPGShop(CommandService c, DiscordClient dc)
        {
            commands = c;
            client = dc;

            commands.CreateCommand("rpgshop")
                .Description("\n\tShop for RPG game")
                .Parameter("param", ParameterType.Unparsed)
                .Do(async e => await Shop(e));
        }

        private async Task Buy(Discord.Commands.CommandEventArgs e, string[] param)
        {
            if(param.Count() <= 1)
            {
                await e.Channel.SendMessage("Just going to watch the merchendise? Oh ok");
                return;
            }
            var amount = 1;         // Amount of (the same) items player wants to buy
            if (param.Count() >= 2)
            {
                Int32.TryParse(param.ElementAt(2), out amount);
            }
            switch(param.ElementAt(1))
            {
                case "0":
                case "hp":
                case "health":
                    Console.WriteLine("Trying to buy hp");
                    break;
                case "1":
                case "exp":
                case "xp":
                    Console.WriteLine("Trying to buy exp");
                    break;
                case "2":
                case "armor":
                case "armour":
                    Console.WriteLine("Trying to buy armor");
                    break;
                default:
                    await e.Channel.SendMessage("Ehhmm, we don't have that in stock right now...");
                    return;
            }
        }

        public async Task Shop(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("param").Split(' ');
            if(param.Count() < 1)
            {
                await e.Channel.SendMessage("What do you want to do in the shop?   (>rpgshop help for more info!)");
                return;
            }
            switch(param.ElementAt(0))
            {
                case "buy":
                    await Buy(e, param);
                    break;
                case "items":
                    await e.Channel.SendMessage("**Todays shopitems:**" 
                        + "\n0)\tHealth potions\t(hp/health)" 
                        + "\n1)\tExperience potions\t(exp/xp)"
                        + "\n2)\tArmor enhancements\t(armor/armour)");
                    break;
                default:
                    await e.Channel.SendMessage("Shopcommand " + param.ElementAt(0) + " not recognized");
                    return;
            }
        }
    }
}
