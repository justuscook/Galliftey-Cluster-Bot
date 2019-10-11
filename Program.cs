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
using System.Collections.Generic;
using Newtonsoft.Json;

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
                ExclusiveBulkDelete = false
            });
        private CommandService commands;
        private IServiceProvider services;
        public Timer timer = new Timer();
        public List<string> welcome = new List<string>() {"Ruff, Ruff, hello human, welcome!","Yay...another human clicked a link to ome here...welcome.", ":robot: :crossed_swords: :smiley: ", "The universe is big. It’s vast and complicated and ridiculous. And sometimes, very rarely, impossible things just happen and we call them miracles. - The Doctor", "Rule 1: The Doctor lies", "You know, the very powerful and the very stupid have one thing in common: they don’t alter their views to fit the facts; they alter the facts to fit their views. - The Doctor",
                                                            "There’s a lot of things you need to get across this universe. Warp drive… wormhole refractors… You know the thing you need most of all? You need a hand to hold - The Doctor","You want weapons? We’re in a library! Books! The best weapons in the world! - The Doctor", "Who let you in here?","Welcome human user.","Welcome to the TARDIS, Here lives Raid's Num... doesn't the Doctor usully handle this...", "if(newUser.Joins) uselessHumanUsers += 1;","http://giphygifs.s3.amazonaws.com/media/e4vLe4S8w9hDi/giphy.gif \n WARNING: WE ARE UNDER ATTACK! Oh, its just another human."};
        public class AllowedChannels
        {
            public List<ulong> Allowed { get; set; }
        }
        public async Task DeleteOldBotMessages(ISocketMessageChannel chan, int getOld)
        {
            /*List<IMessage> messages = (List<IMessage>)chan.GetMessagesAsync(getOld);
            foreach (var m in messages)
            {
                if (m.Author.IsBot) await m.DeleteAsync();
            }*/

            var messages = await chan.GetMessagesAsync(25).FlattenAsync();
            List<IMessage> toDelete = new List<IMessage>();
            foreach (var message in messages)
            {
                if (message.Author.IsBot) toDelete.Add(message);
            }
            await (chan as SocketTextChannel).DeleteMessagesAsync(toDelete);
        }
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

                    var g2 = client.GetChannel(521166881529397258) as ISocketMessageChannel;//g2 CB chan
                    await DeleteOldBotMessages(g2, 25);
                    var g2NMTeam = client.GetGuild(514616202249895936).GetRole(614097750866526208);//g2
                    var g2BruTeam = client.GetGuild(514616202249895936).GetRole(614098077141303319);//g2
                    await g2.SendMessageAsync($"3 Hours to Clan Boss reset!! {g2BruTeam.Mention} {g2NMTeam.Mention}");

                    var skaro = client.GetChannel(518334869667840000) as ISocketMessageChannel;//g2 CB chan
                    await DeleteOldBotMessages(skaro, 25);
                    var sNMTeam = client.GetGuild(514616202249895936).GetRole(614099127583768581);//g2
                    var sBruTeam = client.GetGuild(514616202249895936).GetRole(614099242075553805);//g2
                    await skaro.SendMessageAsync($"3 Hours to Clan Boss reset!! {sNMTeam.Mention} {sBruTeam.Mention}");

                    var daleks = client.GetChannel(525048036280369162) as ISocketMessageChannel;//g2 CB chan
                    await DeleteOldBotMessages(daleks, 25);
                    var dNMTeam = client.GetGuild(514616202249895936).GetRole(614099624285831179);//g2
                    var dBruTeam = client.GetGuild(514616202249895936).GetRole(614099709375807508);//g2
                    await daleks.SendMessageAsync($"3 Hours to Clan Boss reset!! {dNMTeam.Mention} {dBruTeam.Mention}");

                    var headless = client.GetChannel(552884924571713566) as ISocketMessageChannel;//g2 CB chan
                    await DeleteOldBotMessages(headless, 25);
                    var hNMTeam = client.GetGuild(514616202249895936).GetRole(614098743326933024);//g2
                    var hBruTeam = client.GetGuild(514616202249895936).GetRole(614098324160643082);//g2
                    await headless.SendMessageAsync($"3 Hours to Clan Boss reset!! {hNMTeam.Mention} {hBruTeam.Mention}");

                    var arcadian = client.GetChannel(579711482653048852) as ISocketMessageChannel;//g2 CB chan
                    await DeleteOldBotMessages(arcadian, 25);
                    var aNMTeam = client.GetGuild(514616202249895936).GetRole(614098995731759125);//g2
                    var aBruTeam = client.GetGuild(514616202249895936).GetRole(614098870615670795);//g2
                    await arcadian.SendMessageAsync($"3 Hours to Clan Boss reset!! {aNMTeam.Mention} {aBruTeam.Mention}");
                }
                if (DateTime.UtcNow.TimeOfDay >= new TimeSpan(6, 0, 0) && DateTime.UtcNow.TimeOfDay <= new TimeSpan(6, 1, 0))
                {
                    var g1 = client.GetChannel(518332157731799043) as ISocketMessageChannel;
                    await DeleteOldBotMessages(g1, 25);
                    var g1NMTeam = client.GetGuild(514616202249895936).GetRole(614102098593972271);
                    var g1UNMTeam = client.GetGuild(514616202249895936).GetRole(614102098346377236);
                    await g1.SendMessageAsync($"4 Hours to Clan Boss reset!! {g1UNMTeam.Mention} {g1NMTeam.Mention}");

                    var faceless = client.GetChannel(532758210906423296) as ISocketMessageChannel;
                    await DeleteOldBotMessages(faceless, 25);
                    var fNMTeam = client.GetGuild(514616202249895936).GetRole(614098420323319809);
                    var fBruTeam = client.GetGuild(514616202249895936).GetRole(614098324093534236);
                    await faceless.SendMessageAsync($"4 Hours to Clan Boss reset!! {fNMTeam.Mention} {fBruTeam.Mention}");
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
                //await client.SetStatusAsync(UserStatus.Invisible);
            };
            //bot code that kinda make sense to me lol
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(new InteractiveService(client,TimeSpan.FromMinutes(10)))
                .BuildServiceProvider();

            commands = new CommandService();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
            //wehn bot sees a message do stuff
            client.MessageReceived += HandleCommandAsync;
            client.UserJoined += UserJoinedWelcome;
            timer.Start();
            AlarmClock();
            await Task.Delay(-1);
        }
        public bool IsThisChannelAllowed(SocketMessage m)
        {
            var jsonStr = File.ReadAllText("allowedChannels.json");//change this to config.json to build bot, has prefix and bot token to connect to discord
            var allowed = JsonConvert.DeserializeObject<AllowedChannels>(jsonStr);
            //if (m.Channel.Id == 577869684813201438 || m.Channel.Id == 581431959457366016 || m.Channel.Id == 619630134210854925 || m.Channel.Id == 620344940852936714 || m.Channel.Id == 622406661708972042) return true;
            if (allowed.Allowed.Contains(m.Channel.Id)) return true;
            else return false;
        }
        public async Task UserJoinedWelcome(SocketGuildUser u)
        {
            var rand = new Random();
            var message = welcome[rand.Next(welcome.Count - 1)];
            await (client.GetChannel(515092679164428289) as SocketTextChannel).SendMessageAsync($"{message}");
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
                await msg.Channel.SendMessageAsync($"Sorry that didn't work, {client.GetUser(269643701888745474).Mention} get in here!\n`{result.ErrorReason}`");
            }
        }
    }
}