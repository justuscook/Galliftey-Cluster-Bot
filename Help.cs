using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GCB
{
    public class Help : ModuleBase<SocketCommandContext>
    {
        private CommandService commands;
        public Help(CommandService service)           // Create a constructor for the commandservice dependency
        {
            commands = service;
        }
        [Command("help",RunMode =RunMode.Async)]
        public async Task HelpCommand()
        {
            var commandList = "|";
            foreach(var command in commands.Commands)
            {
                commandList += $" **{command.Name}** |";
            }
            await ReplyAsync($"Here is a list of commmands: {commandList}");
        }
    }

}
