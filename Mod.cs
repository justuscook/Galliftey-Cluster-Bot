using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Net.Http;
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static Raid_SL_Bot.Program;

namespace GCB
{
    public class Mod : InteractiveBase<SocketCommandContext>
    {
        [Command("status", RunMode = RunMode.Async)]
        public async Task ChangeBotStats(string status)
        {
            if (status == "off" && (Context.User.Id == 269643701888745474 || (Context.User as SocketGuildUser).GuildPermissions.ManageGuild))
            {
                await Context.Client.SetStatusAsync(UserStatus.Invisible);
            }
            else await Context.Client.SetStatusAsync(UserStatus.Online);
        }

        [Command("text", RunMode = RunMode.Async)]
        public async Task ChangeStatusText(string activity, [Remainder]string text = null)
        {
            var roles = (Context.Message.Author as SocketGuildUser).Roles;
            if (!roles.Contains(Context.Guild.GetRole(540712854177841173)) && !roles.Contains(Context.Guild.GetRole(619927345838686209)) && Context.Message.Author.Id != 269643701888745474 && !roles.Contains(Context.Guild.GetRole(514619966125768705)))
            {
                await ReplyAndDeleteAsync("Hahahaha, you think you can do this command?!", timeout: TimeSpan.FromSeconds(10));
                return;
            }
            if (text == null)
            {
                await ReplyAndDeleteAsync("You must type something to change my status, duh!");
                var newtext = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                if (newtext != null)
                {
                    text = newtext.Content;
                }
                else return;
            }
            if (text == "reset")
            {
                text = "the Gallifrey Cluster";
            }
            var type = ActivityType.Watching;
            switch (activity.ToLower())
            {
                case "watching":
                    type = ActivityType.Watching;
                    break;
                case "listening":
                    type = ActivityType.Listening;
                    break;
                case "playing":
                    type = ActivityType.Playing;
                    break;
                case "streaming":
                    type = ActivityType.Streaming;
                    break;
            }
            await Context.Client.SetStatusAsync(UserStatus.Online);
            await Context.Client.SetActivityAsync(new Game(text, type));
        }
        [Command("say", RunMode = RunMode.Async)]
        public async Task TalkAsBot(ISocketMessageChannel channel, [Remainder]string text)
        {
            var roles = (Context.Message.Author as SocketGuildUser).Roles;
            if (roles.Contains(Context.Guild.GetRole(514619966125768705)))
            {
                await channel.SendMessageAsync($"{text}");
            }
            else return;
        }/*
        [Command("lobby", RunMode = RunMode.Async)]
        public async Task TalkAsBotInLobby([Remainder]string text = null)
        {
            var roles = (Context.Message.Author as SocketGuildUser).Roles;
            if (roles.Contains(Context.Guild.GetRole(514619966125768705)))
            {
                await Context.Guild.GetTextChannel(515092679164428289).SendMessageAsync($"{text}");
            }
            else return;
        }*/
        [Command("addChannel", RunMode = RunMode.Async)]
        public async Task AddAlloweedChannel(ISocketMessageChannel channel)
        {
            var roles = (Context.Message.Author as SocketGuildUser).Roles;
            if (roles.Contains(Context.Guild.GetRole(514619966125768705)))
            {
                var filePath = "allowedChannels.json";
                var jsonData = File.ReadAllText(filePath);
                var allowed = JsonConvert.DeserializeObject<AllowedChannels>(jsonData);
                if (allowed.Allowed.Contains(channel.Id))
                {
                    await ReplyAndDeleteAsync("That channel is already allowed.");
                    return;
                }
                else
                {
                    allowed.Allowed.Add(channel.Id);
                    jsonData = JsonConvert.SerializeObject(allowed);
                    File.WriteAllText(filePath, jsonData);
                    await ReplyAndDeleteAsync($"{channel.Name} added to the list of allowed channels.");
                }
            }
            else return;

        }
        [Command("clean",RunMode = RunMode.Async)]
        public async Task Delete25Messages(ISocketMessageChannel chan)
        {
            try
            {
                if (chan == null)
                {
                    var messages = await Context.Channel.GetMessagesAsync(25).FlattenAsync();
                    List<IMessage> toDelete = new List<IMessage>();
                    foreach (var message in messages)
                    {
                        if (message.Author.IsBot) toDelete.Add(message);
                    }
                    await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(toDelete);
                }
                else
                {
                    var messages = await chan.GetMessagesAsync(25).FlattenAsync();
                    List<IMessage> toDelete = new List<IMessage>();
                    foreach (var message in messages)
                    {
                        if (message.Author.IsBot) toDelete.Add(message);
                    }
                    await (chan as SocketTextChannel).DeleteMessagesAsync(toDelete);
                }
            }
            catch(Exception e)
            {
                await ReplyAsync($"{Context.Guild.GetUser(269643701888745474).Mention} I broke the broom..\n{e.Message}");
                return;
            }
            await ReplyAndDeleteAsync("Sweep, sweep, I cleanded up some of my messages :recycle:");
        }
    }

}