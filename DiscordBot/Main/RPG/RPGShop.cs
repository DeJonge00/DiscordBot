using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        private Task Buy(Discord.Commands.CommandEventArgs e, string[] param)
        {
            throw new NotImplementedException();
        }

        private Task Help(Discord.Commands.CommandEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async Task Shop(Discord.Commands.CommandEventArgs e)
        {
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
                case "help":
                    await Help(e);
                    break;
                case "info":
                    break;
                default:
                    await e.Channel.SendMessage("Shopcommand " + param.ElementAt(0) + " not recognized");
                    return;
            }
        }
    }
}
