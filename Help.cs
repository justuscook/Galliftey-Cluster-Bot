using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

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
            var roles = (Context.User as SocketGuildUser).Roles;
            var commandList = "|";
            foreach(var command in commands.Commands)
            {
                if (command.Module.Name != "Mod" || roles.Contains(Context.Guild.GetRole(514619966125768705)))
                commandList += $" **{command.Name}** |";
            }
            await ReplyAsync($"Here is a list of commands: {commandList}");
        }
    }

}
