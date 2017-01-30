using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordBot.Main.RPG
{
    class RPGShop
    {
        private CommandService commands;
        private RPGMain rpgmain;

        public RPGShop(CommandService c, RPGMain rpgm)
        {
            commands = c;
            rpgmain = rpgm;

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
            Console.WriteLine("XD");
            var amount = 1;         // Amount of (the same) items player wants to buy
            if (param.Count() > 2)
            {
                Int32.TryParse(param.ElementAt(2), out amount);
            }
            var player = rpgmain.GetPlayerData(e.User);
            switch(param.ElementAt(1))
            {
                case "0":
                case "hp":
                case "health":
                    var cost = amount * 25;
                    if (cost > player.money)
                    {
                        var mess = "You cant buy something without money";
                        if(player.money > 25)
                        {
                            cost = player.money / 25;
                            var cost2 = Math.Ceiling((player.maxHealth - player.health)/10);
                            if (cost < cost2)
                            {
                                mess += "\nYou can only buy a maximum of " + cost;
                            } else
                            {
                                mess += "\nYou only need to buy a maximum of " + cost2;
                            }
                        }
                        await e.Channel.SendMessage(mess);
                        return;
                    }
                    player.AddMoney(-cost);
                    player.AddHealth(amount*10);
                    await e.Channel.SendMessage("You succesfully bought " + amount + " healthpotions, healing you to + " + player.health);
                    MyBot.Log(DateTime.Now.ToUniversalTime().ToShortTimeString() + ") " + player.name + " bought " + amount + " healthpotions, healing to + " + player.health, RPGMain.filename);
                    break;
                case "1":
                case "maxhealth":
                case "maxhp":
                case "2":
                case "armor":
                case "armour":
                case "3":
                case "weaponskill":
                case "ws":
                default:
                    await e.Channel.SendMessage("Ehhmm, we don't have that in stock right now...");
                    return;
            }
        }

        public async Task Shop(Discord.Commands.CommandEventArgs e)
        {
            await e.Message.Delete();
            var param = e.GetArg("param").Split(' ');
            if(param.Count() <= 0)
            {
                await e.Channel.SendMessage("What do you want to do in the shop?   (>rpghelp for more info!)");
                return;
            }
            switch(param.ElementAt(0))
            {
                case "buy":
                    await Buy(e, param);
                    break;
                case "items":
                    await e.Channel.SendMessage("**Todays shopitems:**" 
                        + "\n0)\tHealth potions      \t(hp/health)" 
                        + "\n1)\tMuscle training     \t(maxhp/maxhealth)"
                        + "\n2)\tArmor enhancements  \t(armor/armour)"
                        + "\n3)\tWeapon training     \t(weaponskill/ws)");
                    break;
                default:
                    await e.Channel.SendMessage("Shopcommand " + param.ElementAt(0) + " not recognized");
                    return;
            }
        }
    }
}
