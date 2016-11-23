using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace DiscordBot.Main.GameObjects.WarhammerRPG
{
    class RPGShop
    {
        private int[] costs = { 100, 10, 100, 100, 100, 100 };
        private Game game;

        public RPGShop(Game g)
        {
            game = g;
        }

        public async Task Buy(CommandEventArgs e)
        {
            var param = e.GetArg("param").Split(' ');
            var item = -1;
            Int32.TryParse(param[1], out item);
            if (param[1].ToLower() == "health")
                item = 0;
            if (param[1].ToLower() == "meds")
                item = 1;
            if (param[1].ToLower() == "ws")
                item = 2;
            if (param[1].ToLower() == "s")
                item = 3;
            if(item < 0 || item > 3)
            {
                await e.Channel.SendMessage("An Ogre cannot give what an Ogre does not have...");
                return;
            }
            var amount = 1;
            if (param.Length >= 3)
                Int32.TryParse(param[2], out amount);
            await BuyItem(e, item, amount);
        }

        private async Task BuyItem(Discord.Commands.CommandEventArgs e, int item, int amount)
        {
            var cost = costs[item] * amount;
            var data = game.GetUser(e.User.Id, e.User.Name);
            if (cost > data.points)
            {
                await e.User.SendMessage("Are you trying to steal this :angry:, you can only afford " + (data.points / costs[item]) + " of this");
                return;
            }
            switch (item)
            {
                case 0:
                    data.IncreaseMaxHealth(amount * 10);
                    break;
                case 1:
                    data.IncreaseHealth(amount * 10);
                    break;
                case 2:
                    data.IncreaseWeaponskill(amount);
                    break;
                case 3:
                    data.IncreaseStrength(amount);
                    break;
                case 4:
                    data.IncreaseToughness(amount);
                    break;
                default:
                    await e.User.SendMessage("Something went wrong, item unknown");
                    return;
            }
            data.SubtractPoints(cost);
            await e.User.SendMessage("Succesfully bought " + amount + " of item " + item + " for " + cost);
        }

        public async Task ShopInfo(CommandEventArgs e)
        {
            var info = "**Guts 'n' Garbage**\n```" +
                "Item                          | Info                         | Cost" +
                "\n1 More Guts                     Increases Health Permanently   " + costs[0] +
                "\n2 Snack on a gnoblar            Increase Health                " + costs[1] +
                "\n3 Slaughter some greenskins     Increases Weapon Skill         " + costs[2] +
                "\n4 Hit a rock very hard          Increases Strength             " + costs[3] +
                "\n4 Get tortured                  Increases Toughness            " + costs[4] +
                "```";
            await e.User.SendMessage(info);
        }
    }
}
