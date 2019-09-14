using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.IO;
using Newtonsoft.Json.Linq;
using Discord.Addons.Interactive;
using System.Timers;

namespace Raid_SL_Bot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();//bot code i somewhat understand
        //Discord bot stuff
        public DiscordSocketClient client =
            new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true,
            });
        private CommandService commands;
        private IServiceProvider services;
        public Timer timer = new Timer();


        public void AlarmClock()
        {
            timer.Interval = 60000;
            timer.Elapsed += tick;
        }
        public async void tick(object sender, EventArgs e)
        {
            try
            {
                if (DateTime.UtcNow.TimeOfDay >= new TimeSpan(7, 0, 0) && DateTime.UtcNow.TimeOfDay <= new TimeSpan(7, 1, 0))
                {
                    var chan = client.GetChannel(521166881529397258) as ISocketMessageChannel;//g2 CB chan
                    var NMTeam = client.GetGuild(514616202249895936).GetRole(614097750866526208);//g2
                    var BruTeam = client.GetGuild(514616202249895936).GetRole(614098077141303319);//g2
                    await chan.SendMessageAsync($"3 Hours to Clan Boss reset!! {BruTeam.Mention} {NMTeam.Mention}");
                }
            }
            catch (Exception err)
            {
                Console.WriteLine($"THe timer went off, but\n{err}");
            }
        }


        public string Prefix;
        public async Task MainAsync()
        {
            var jsonStr = File.ReadAllText("config.json");//change this to config.json to build bot, has prefix and bot token to connect to discord
            var jobj = JObject.Parse(jsonStr);
            var prefix = jobj.SelectToken("prefix").ToString();
            Prefix = prefix;
            var token = jobj.SelectToken("token").ToString();
            client = new DiscordSocketClient();
            //console log
            client.Log += log =>
            {

                //Console.WriteLine(log.ToString());
                if (!log.Message.ToLower().Contains("unknown")) Console.WriteLine($"{DateTime.Now} [{log.Severity,8}] {log.Source}: {log.Message}");
                return Task.CompletedTask;
            };
            //connect to discord
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            //sets bot status when it joins, leaves, or starts up to the number of servers its in, just for fun, diaplys help command like most bots
            client.Ready += async () =>
            {
                await client.SetGameAsync("the Gallifrey Cluster", null, ActivityType.Watching);
                await client.SetStatusAsync(UserStatus.Invisible);
            };
            //bot code that kinda make sense to me lol
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(new InteractiveService(client))
                .BuildServiceProvider();

            commands = new CommandService();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            //wehn bot sees a message do stuff
            client.MessageReceived += HandleCommandAsync;
            timer.Start();
            AlarmClock();
            await Task.Delay(-1);
        }
        public bool IsThisChannelAllowed(SocketMessage m)
        {
            if (m.Channel.Id == 577869684813201438 || m.Channel.Id == 581431959457366016 || m.Channel.Id == 619630134210854925 || m.Channel.Id == 620344940852936714 || m.Channel.Id == 622406661708972042) return true;
            else return false;
        }
        public async Task HandleCommandAsync(SocketMessage m)
        {

            if (!(m is SocketUserMessage msg)) return;//i think this ignores discord system messages
            if (msg.Author.IsBot) return;//ignores all bots, i think this is an unwritten rule for bots
                                         //if (msg.Author.Id != 269643701888745474) return;//For Testing only Me
            if (!IsThisChannelAllowed(m))
            {
                return;
            }
            int argPos = 0;

            //if (!(msg.HasStringPrefix(Prefix, ref argPos))) return;//check for prefix
            { if (!(msg.HasStringPrefix((Prefix), ref argPos))) return; }

            var context = new SocketCommandContext(client, msg);//command conext
            var result = await commands.ExecuteAsync(context, argPos, services);//try command
            var command = commands.Search(context, argPos);

            if (!result.IsSuccess)//if it fails tell the user, some runtime errors dont seem to trigger this
            {
                if (result.Error == CommandError.UnknownCommand) return;
                Console.WriteLine(result.ToString());
                await msg.Channel.SendMessageAsync("Sorry that didn't work, @Orcinus get in here!");
            }
        }
    }
}