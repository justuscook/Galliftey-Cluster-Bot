using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Net.Http;
using Discord.Addons.Interactive;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tesseract;
using System.Globalization;

namespace GCB
{
    public class ChampionBuilds : InteractiveBase<SocketCommandContext>//needed for command modules
    {

        private string randomString = ""; /*Creates a empty string*/
        public static List<string> damageList = new List<string>();
        public async Task<SixLabors.ImageSharp.Image<Rgba32>> StartStreamAsync(IUser user = null, string url = null, string path = null) /*Creates a async Task that returns a ImageSharp image with a user and url param*/
        {
            HttpClient httpClient = new HttpClient(); /*Creates a new HttpClient*/
            HttpResponseMessage response = null;
            SixLabors.ImageSharp.Image<Rgba32> image = null; /*Creates a null ImageSharp image*/

            if (url != null)
            {
                response = await httpClient.GetAsync(url); /*sets the response to the url*/
                Stream inputStream = await response.Content.ReadAsStreamAsync(); /*creates a inputStream variable and reads the url*/
                image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream); /*Loads the image to the ImageSharp image we created earlier*/
                inputStream.Dispose();
            }
            return image; /*returns the image*/
        }

        public async Task StopStreamAsync(IUserMessage msg, SixLabors.ImageSharp.Image<Rgba32> image, string type = null) /*Creates an async task with UserMessage as a paramater */
        {
            string input = "abcdefghijklmnopqrstuvwxyz0123456789"; /*alphabet abcdefghijklmaopqrstuvwxy and z! now I know my abc's!*/
            char ch;
            Random rand = new Random();
            for (int i = 0; i < 8; i++)/*loops through the alphabet and creates a random string with 8 random characters */
            {
                ch = input[rand.Next(0, input.Length)];
                randomString += ch;
            }
            if (image != null) /*Checks if the image we created is not null if it is then this part won't run*/
            {
                Stream outputStream = new MemoryStream();
                if (type == null)
                {
                    image.SaveAsPng(outputStream); /*saves the image as a jpg you can of course change this*/
                    type = "png";
                }
                else
                {
                    image.SaveAsGif(outputStream);
                }
                outputStream.Position = 0;
                await msg.Channel.SendFileAsync(outputStream, $"{randomString}.{type}");
            }
        }

        public async Task<EmbedBuilder> StopStreamReturnEmbedAsync(SocketCommandContext context, IUserMessage msg, SixLabors.ImageSharp.Image<Rgba32> image, string type = null) /*Creates an async task with UserMessage as a paramater */
        {
            IUserMessage imageUrl = null;
            var embed = new EmbedBuilder();
            string input = "abcdefghijklmnopqrstuvwxyz0123456789"; /*alphabet abcdefghijklmaopqrstuvwxy and z! now I know my abc's!*/
            char ch;
            Random rand = new Random();
            for (int i = 0; i < 8; i++)/*loops through the alphabet and creates a random string with 8 random characters */
            {
                ch = input[rand.Next(0, input.Length)];
                randomString += ch;
            }
            if (image != null) /*Checks if the image we created is not null if it is then this part won't run*/
            {
                if (type == null)
                {
                    Stream outputStream = new MemoryStream();
                    image.SaveAsPng(outputStream); /*saves the image as a jpg you can of course change this*/
                    outputStream.Position = 0;
                    var file = File.Create($"./images/{randomString}.png"); /*creates a file with the random string as the name*/
                    await outputStream.CopyToAsync(file);
                    file.Dispose();
                    var chan = context.Client.GetChannel(620344940852936714) as IMessageChannel;
                    imageUrl = await chan.SendFileAsync($"images/{randomString}.png"); /*sends the image we just created*/
                    embed.ImageUrl = imageUrl.Attachments.FirstOrDefault().Url;
                    File.Delete($"images/{randomString}.png"); /*deletes the image after sending*/
                    //_ = Task.Run(() => DelayDeleteAsync(Context, imageUrl, 1));
                }
            }

            return embed;
        }

        public async Task DelayDeleteAsync(SocketCommandContext context = null, IMessage message = null, int? timeDelay = null)
        {
            if (context != null)
            {
                if (context.Channel is SocketDMChannel) return;
            }
            if (timeDelay == null) timeDelay = 15;
            await Task.Delay(timeDelay.Value * 1000);
            if (message.Channel.GetMessageAsync(message.Id) != null) await message.DeleteAsync();
        }
        public double NewHeight(double h, double w)
        {
            return 800.0 / (w / h);
        }
        public class ChampionBuild
        {
            public string name { get; set; } = "";
            public string instance { get; set; } = "";
            public string allImages { get; set; } = "";
            public string gearImage { get; set; } = null;
            public string masteryImage { get; set; } = null;
            public string statsImage { get; set; } = null;
            public string note { get; set; } = "";
            public ulong authorID { get; set; }
            public string guid { get; set; }
        }

        [Command("submit", RunMode = RunMode.Async)]
        public async Task SubmitBuild()
        {
            try
            {
                var roles = (Context.Message.Author as SocketGuildUser).Roles;
                if (!roles.Contains(Context.Guild.GetRole(540712854177841173)) && !roles.Contains(Context.Guild.GetRole(619927345838686209)) && Context.Message.Author.Id != 269643701888745474 && !roles.Contains(Context.Guild.GetRole(514619966125768705)))
                {
                    await ReplyAndDeleteAsync("Puny mortal you are not worthy of submitting builds!", timeout: TimeSpan.FromSeconds(10));
                    return;
                }
                await ReplyAndDeleteAsync("How many images do you have for this build?");
                var exitStrings = new List<string> { "exit", "cancel", "keep" };
                var numberOfImages = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                if (exitStrings.Contains(numberOfImages.Content.ToLower()) || numberOfImages == null)
                {
                    await ReplyAndDeleteAsync("Submission canceled.", timeout: TimeSpan.FromMinutes(10));
                    return;
                }
                else if (numberOfImages.Content == "3")
                {
                    var champion = new ChampionBuild();
                    await ReplyAndDeleteAsync("Champion name?", timeout: TimeSpan.FromMinutes(10));
                    var name = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(name.Content.ToLower()) || name == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.", timeout: TimeSpan.FromMinutes(10));
                        if (name.Content.ToLower() == "exit" || name.Content == null) return;
                    }

                    await ReplyAndDeleteAsync("Instance?", timeout: TimeSpan.FromMinutes(10));
                    var instance = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(instance.Content.ToLower()) || instance == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.", timeout: TimeSpan.FromMinutes(10));
                        if (instance.Content.ToLower() == "exit" || instance.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Gear image?", timeout: TimeSpan.FromMinutes(10));
                    var gear = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(gear.Content.ToLower()) || gear == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (gear.Content.ToLower() == "exit" || gear.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Stats image?");
                    var stats = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(stats.Content.ToLower()) || stats == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (stats.Content.ToLower() == "exit" || stats.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Masteries image?");
                    var mastieries = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(mastieries.Content.ToLower()) || mastieries == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (mastieries.Content.ToLower() == "exit" || mastieries.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Build notes?");
                    var note = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(note.Content.ToLower()) || note == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (note.Content.ToLower() == "exit" || note.Content == null) return;
                    }
                    IUserMessage buildImage;
                    using (Image<Rgba32> image = new Image<Rgba32>(1, 1))
                    {
                        var gearUrl = "";
                        if (gear.Content == "") gearUrl = gear.Attachments.FirstOrDefault().Url;
                        else gearUrl = gear.Content;
                        var gearImage = await StartStreamAsync(url: gearUrl);
                        var newHeight = NewHeight(gearImage.Height, gearImage.Width);
                        gearImage.Mutate(x => x.Resize(800, (int)NewHeight(gearImage.Height, gearImage.Width)));
                        var statsUrl = "";
                        if (stats.Content == "") statsUrl = stats.Attachments.FirstOrDefault().Url;
                        else statsUrl = stats.Content;
                        var statsImage = await StartStreamAsync(url: statsUrl);
                        statsImage.Mutate(x => x.Resize(800, (int)NewHeight(statsImage.Height, statsImage.Width)));
                        var mastUrl = "";
                        if (mastieries.Content == "") mastUrl = mastieries.Attachments.FirstOrDefault().Url;
                        else mastUrl = mastieries.Content;
                        var masteriesImage = await StartStreamAsync(url: mastieries.Content ?? mastieries.Attachments.FirstOrDefault().Url);
                        masteriesImage.Mutate(x => x.Resize(800, (int)NewHeight(masteriesImage.Height, masteriesImage.Width)));
                        image.Mutate(x => x.Resize(800, gearImage.Height + statsImage.Height + masteriesImage.Height));
                        image.Mutate(x => x.DrawImage(gearImage, new SixLabors.Primitives.Point(0, 0), 1));

                        image.Mutate(x => x.DrawImage(statsImage, new SixLabors.Primitives.Point(0, gearImage.Height), 1));
                        image.Mutate(x => x.DrawImage(masteriesImage, new SixLabors.Primitives.Point(0, gearImage.Height * 2), 1));
                        await ReplyAndDeleteAsync("Thanks for the build!");
                        champion.name = name.Content;
                        champion.instance = instance.Content;
                        champion.authorID = Context.Message.Author.Id;
                        champion.guid = DateTime.UtcNow.Ticks.ToString();
                        if (exitStrings.Contains(note.Content.ToLower())) champion.note = "N/A";
                        else
                        {
                            champion.note = note.Content;
                        }
                        var embed = await StopStreamReturnEmbedAsync(Context, Context.Message, image);
                        embed.Title = $"{champion.name} {champion.instance} build review.";
                        embed.AddField("Champion:", champion.name);
                        embed.AddField("Instance:", champion.instance);
                        embed.AddField("Created by:", Context.Client.GetUser(champion.authorID).Username);
                        embed.AddField("Build notes:", champion.note);
                        embed.AddField("GUID:", champion.guid);
                        buildImage = await ReplyAndDeleteAsync("", false, embed.Build(), timeout: TimeSpan.FromMinutes(60));
                        champion.allImages = buildImage.Embeds.FirstOrDefault().Image.Value.Url;
                        champion.gearImage = gearUrl;
                        champion.masteryImage = mastUrl;
                        champion.statsImage = statsUrl;
                    }

                    var filePath = "builds.json";
                    var jsonData = File.ReadAllText(filePath);
                    var buildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(jsonData) ?? new List<ChampionBuild>();
                    buildList.Add(champion);
                    jsonData = JsonConvert.SerializeObject(buildList);
                    File.WriteAllText(filePath, jsonData);
                }
                else if (numberOfImages.Content == "2")
                {
                    var champion = new ChampionBuild();
                    await ReplyAndDeleteAsync("Champion name?");
                    var name = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(name.Content.ToLower()) || name == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (name.Content.ToLower() == "exit" || name.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Instance?");
                    var instance = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(instance.Content.ToLower()) || instance == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (instance.Content.ToLower() == "exit" || instance.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Gear/Stats image?");
                    var stats = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(stats.Content.ToLower()) || stats == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (stats.Content.ToLower() == "exit" || stats.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Masteries image?");
                    var mastieries = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(mastieries.Content.ToLower()) || mastieries == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (mastieries.Content.ToLower() == "exit" || mastieries.Content == null) return;
                    }
                    await ReplyAndDeleteAsync("Build notes?");
                    var note = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(note.Content.ToLower()) || note == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        if (note.Content.ToLower() == "exit" || note.Content == null) return;
                    }
                    IUserMessage buildImage;
                    using (Image<Rgba32> image = new Image<Rgba32>(1, 1))
                    {
                        var statsurl = "";
                        if (stats.Content == "") statsurl = stats.Attachments.FirstOrDefault().Url;
                        else statsurl = stats.Content;
                        var statsGearIage = await StartStreamAsync(url: statsurl);
                        var newHeight = NewHeight(statsGearIage.Height, statsGearIage.Width);
                        statsGearIage.Mutate(x => x.Resize(800, (int)NewHeight(statsGearIage.Height, statsGearIage.Width)));
                        var masteriesUrl = "";
                        if (mastieries.Content == "") masteriesUrl = mastieries.Attachments.FirstOrDefault().Url;
                        else masteriesUrl = mastieries.Content;
                        var masteriesImage = await StartStreamAsync(url: masteriesUrl);
                        masteriesImage.Mutate(x => x.Resize(800, (int)NewHeight(masteriesImage.Height, masteriesImage.Width)));
                        image.Mutate(x => x.Resize(800, statsGearIage.Height + masteriesImage.Height));
                        image.Mutate(x => x.DrawImage(statsGearIage, new SixLabors.Primitives.Point(0, 0), 1));
                        image.Mutate(x => x.DrawImage(masteriesImage, new SixLabors.Primitives.Point(0, statsGearIage.Height), 1));
                        await ReplyAndDeleteAsync("Thanks for the build!");
                        champion.name = name.Content;
                        champion.instance = instance.Content;
                        champion.authorID = Context.Message.Author.Id;
                        champion.guid = DateTime.UtcNow.Ticks.ToString();
                        if (exitStrings.Contains(note.Content.ToLower())) champion.note = "N/A";
                        else
                        {
                            champion.note = note.Content;
                        }
                        var embed = await StopStreamReturnEmbedAsync(Context, Context.Message, image);
                        embed.Title = $"{champion.name} {champion.instance} build review.";
                        embed.AddField("Champion:", champion.name);
                        embed.AddField("Instance:", champion.instance);
                        embed.AddField("Created by:", Context.Client.GetUser(champion.authorID).Username);
                        embed.AddField("Build notes:", champion.note);
                        embed.AddField("GUID:", champion.guid);
                        buildImage = await ReplyAndDeleteAsync("", false, embed.Build(), timeout: TimeSpan.FromMinutes(60));
                        champion.allImages = buildImage.Embeds.FirstOrDefault().Image.Value.Url;
                        champion.gearImage = statsurl;
                        champion.masteryImage = masteriesUrl;
                        champion.statsImage = null;
                    }

                    var filePath = "builds.json";
                    var jsonData = File.ReadAllText(filePath);
                    var buildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(jsonData) ?? new List<ChampionBuild>();
                    buildList.Add(champion);
                    jsonData = JsonConvert.SerializeObject(buildList);
                    File.WriteAllText(filePath, jsonData);
                }

                else
                {
                    await ReplyAndDeleteAsync("Please respond with 2 or 3, so I know how images you have.  Try again.");
                }

            }
            catch (Exception e)
            {
                var ran = new Random();
                var eEmbed = new EmbedBuilder();
                eEmbed.WithFooter(e.Message);
                eEmbed.Title = "Whoopsy...";
                eEmbed.AddField($"Error {ran.Next(10000, 99999)}:", "*sigh*, what did you do wrong this time...*");
                await ReplyAndDeleteAsync("", embed: eEmbed.Build());
            }
            //messageList.ForEach(x => x.DeleteAsync());
        }

        [Command("edit", RunMode = RunMode.Async)]
        public async Task Edit(string guid, string part = "")
        {
            try
            {
                if (part == "")
                {
                    var roles = (Context.Message.Author as SocketGuildUser).Roles;
                    //540712854177841173 team builders
                    if (!roles.Contains(Context.Guild.GetRole(540712854177841173)) && !roles.Contains(Context.Guild.GetRole(619927345838686209)) && Context.Message.Author.Id != 269643701888745474 && !roles.Contains(Context.Guild.GetRole(514619966125768705)))
                    {
                        await ReplyAndDeleteAsync("Puny mortal you are not worthy of submitting builds!", timeout: TimeSpan.FromSeconds(10));
                        return;
                    }

                    var editFilePath = "builds.json";
                    var editJsonData = File.ReadAllText(editFilePath);
                    var editBuildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(editJsonData);
                    var build = editBuildList.FirstOrDefault(x => x.guid == guid);

                    if (build.authorID != Context.User.Id && !roles.Contains(Context.Guild.GetRole(514619966125768705)))
                    {
                        await ReplyAndDeleteAsync("You can only edit your own builds.");
                        return;
                    }
                    editBuildList.Remove(build);
                    await ReplyAndDeleteAsync("The previous info will be displayed, to keep it unchanged type `keep`.");
                    await ReplyAndDeleteAsync("How many images do you have for this build?");
                    var exitStrings = new List<string> { "exit", "cancel" };
                    var numberOfImages = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    if (exitStrings.Contains(numberOfImages.Content.ToLower()) || numberOfImages == null)
                    {
                        await ReplyAndDeleteAsync("Submission canceled.");
                        return;
                    }
                    else if (numberOfImages.Content == "3")
                    {
                        //var champion = new ChampionBuild();
                        await ReplyAndDeleteAsync($"Champion name? Currently: *{build.name}*");
                        var name = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(name.Content.ToLower()) || name == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (name.Content.ToLower() == "exit" || name.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Instance? Currently: *{build.instance}*");
                        var instance = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(instance.Content.ToLower()) || instance == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (instance.Content.ToLower() == "exit" || instance.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Gear image? Currently: {build.gearImage ?? "NULL"}");
                        var gear = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(gear.Content.ToLower()) || gear == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (gear.Content.ToLower() == "exit" || gear.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Stats image? Currently: {build.statsImage ?? "NULL"}");
                        var stats = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(stats.Content.ToLower()) || stats == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (stats.Content.ToLower() == "exit" || stats.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Masteries image? Currently: {build.masteryImage ?? "NULL"}");
                        var mastieries = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(mastieries.Content.ToLower()) || mastieries == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (mastieries.Content.ToLower() == "exit" || mastieries.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Build notes? Currently: {build.note}");
                        var note = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(note.Content.ToLower()) || note == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (note.Content.ToLower() == "exit" || note.Content == null) return;
                        }
                        IUserMessage buildImage;
                        using (Image<Rgba32> image = new Image<Rgba32>(1, 1))
                        {
                            var gearUrl = "";
                            if (gear.Content == "") gearUrl = gear.Attachments.FirstOrDefault().Url;
                            else gearUrl = gear.Content;
                            var gearImage = await StartStreamAsync(url: gearUrl);
                            var newHeight = NewHeight(gearImage.Height, gearImage.Width);
                            gearImage.Mutate(x => x.Resize(800, (int)NewHeight(gearImage.Height, gearImage.Width)));
                            var statsUrl = "";
                            if (stats.Content == "") statsUrl = stats.Attachments.FirstOrDefault().Url;
                            else statsUrl = stats.Content;
                            var statsImage = await StartStreamAsync(url: statsUrl);
                            statsImage.Mutate(x => x.Resize(800, (int)NewHeight(statsImage.Height, statsImage.Width)));
                            var mastUrl = "";
                            if (mastieries.Content == "") mastUrl = mastieries.Attachments.FirstOrDefault().Url;
                            else mastUrl = mastieries.Content;
                            var masteriesImage = await StartStreamAsync(url: mastUrl);
                            masteriesImage.Mutate(x => x.Resize(800, (int)NewHeight(masteriesImage.Height, masteriesImage.Width)));
                            image.Mutate(x => x.Resize(800, gearImage.Height + statsImage.Height + masteriesImage.Height));
                            image.Mutate(x => x.DrawImage(gearImage, new SixLabors.Primitives.Point(0, 0), 1));
                            image.Mutate(x => x.DrawImage(statsImage, new SixLabors.Primitives.Point(0, gearImage.Height), 1));
                            image.Mutate(x => x.DrawImage(masteriesImage, new SixLabors.Primitives.Point(0, gearImage.Height + statsImage.Height), 1));
                            await ReplyAndDeleteAsync("Thanks for the build!");
                            if (name.Content != "keep") build.name = name.Content;
                            if (instance.Content != "keep") build.instance = instance.Content;
                            build.authorID = Context.Message.Author.Id;
                            build.guid = guid;
                            if (exitStrings.Contains(note.Content.ToLower())) build.note = "N/A";
                            else if (note.Content != "keep") build.note = note.Content;
                            build.gearImage = gearUrl;
                            build.masteryImage = mastUrl;
                            build.statsImage = statsUrl;
                            var embed = await StopStreamReturnEmbedAsync(Context, Context.Message, image);
                            embed.Title = $"{build.name} {build.instance} build **edit** review.";
                            embed.AddField("Champion:", build.name);
                            embed.AddField("Instance:", build.instance);
                            embed.AddField("Created by:", Context.Client.GetUser(build.authorID).Username);
                            embed.AddField("Build notes:", build.note);
                            embed.AddField("GUID: ", build.guid);
                            buildImage = await ReplyAndDeleteAsync("", false, embed.Build(), timeout: TimeSpan.FromMinutes(60));
                            build.allImages = buildImage.Embeds.FirstOrDefault().Image.Value.Url;
                        }
                        /*var editFilePath = "builds.json";
                    var editJsonData = File.ReadAllText(editFilePath);
                    var editBuildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(editJsonData);
                    var championBuilds = editBuildList.Where(x => x.guid == guid);*/
                        editBuildList.Add(build);
                        editJsonData = JsonConvert.SerializeObject(editBuildList);
                        File.WriteAllText(editFilePath, editJsonData);
                    }
                    else if (numberOfImages.Content == "2")
                    {
                        await ReplyAndDeleteAsync($"Champion name? Currently: {build.name}");
                        var name = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(name.Content.ToLower()) || name == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (name.Content.ToLower() == "exit" || name.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Instance? Currently: {build.instance}");
                        var instance = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(instance.Content.ToLower()) || instance == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (instance.Content.ToLower() == "exit" || instance.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Gear/Stats image? Currently: {build.statsImage ?? "NULL"}");
                        var stats = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(stats.Content.ToLower()) || stats == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (stats.Content.ToLower() == "exit" || stats.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Masteries image? Currently: {build.masteryImage ?? "NULL"}");
                        var mastieries = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(mastieries.Content.ToLower()) || mastieries == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (mastieries.Content.ToLower() == "exit" || mastieries.Content == null) return;
                        }
                        await ReplyAndDeleteAsync($"Build notes? Currently: {build.note}");
                        var note = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                        if (exitStrings.Contains(note.Content.ToLower()) || note == null)
                        {
                            await ReplyAndDeleteAsync("Submission canceled.");
                            if (note.Content.ToLower() == "exit" || note.Content == null) return;
                        }
                        IUserMessage buildImage;
                        using (Image<Rgba32> image = new Image<Rgba32>(1, 1))
                        {
                            if (build.masteryImage == null && build.gearImage == null && build.statsImage == null)
                            {
                                var mastImg2 = await StartStreamAsync(url: build.allImages);
                                var gearUrl2 = "";
                                if (stats.Content == "") gearUrl2 = stats.Attachments.FirstOrDefault().Url;
                                else gearUrl2 = stats.Content;
                                var gearImage2 = await StartStreamAsync(url: gearUrl2);
                                var newH = NewHeight(gearImage2.Height, gearImage2.Width);
                                gearImage2.Mutate(x => x.Resize(800, (int)newH));
                                image.Mutate(x => x.Resize(mastImg2.Width, mastImg2.Height));
                                image.Mutate(x => x.DrawImage(gearImage2, new SixLabors.Primitives.Point(0, 0), 1));
                                image.Mutate(x => x.DrawImage(mastImg2, new SixLabors.Primitives.Point(0, gearImage2.Height), 1));
                                if (name.Content.ToLower() != "keep") build.name = name.Content;
                                if (instance.Content.ToLower() != "keep") build.instance = instance.Content;
                                build.authorID = Context.Message.Author.Id;
                                build.guid = guid;
                                if (exitStrings.Contains(note.Content.ToLower())) build.note = "N/A";
                                else if (note.Content.ToLower() != "keep") build.note = note.Content;
                                build.gearImage = null;
                                build.masteryImage = null;
                                build.statsImage = gearUrl2;
                                build.authorID = Context.Message.Author.Id;
                                build.guid = guid;
                            }
                            else
                            {
                                var statsGearUrl = "";
                                if (stats.Content == "") statsGearUrl = stats.Attachments.FirstOrDefault().Url;
                                else if (stats.Content.ToLower() == "keep") statsGearUrl = build.statsImage;
                                else statsGearUrl = stats.Content;
                                var statsGearIage = await StartStreamAsync(url: statsGearUrl);
                                var newHeight = NewHeight(statsGearIage.Height, statsGearIage.Width);
                                statsGearIage.Mutate(x => x.Resize(800, (int)NewHeight(statsGearIage.Height, statsGearIage.Width)));
                                var mastUrl = "";
                                if (mastieries.Content == "") mastUrl = mastieries.Attachments.FirstOrDefault().Url;
                                else if (mastieries.Content.ToLower() == "keep") mastUrl = build.masteryImage;
                                else mastUrl = mastieries.Content;
                                var masteriesImage = await StartStreamAsync(url: mastUrl);
                                masteriesImage.Mutate(x => x.Resize(800, (int)NewHeight(masteriesImage.Height, masteriesImage.Width)));
                                image.Mutate(x => x.Resize(800, statsGearIage.Height + masteriesImage.Height));
                                image.Mutate(x => x.DrawImage(statsGearIage, new SixLabors.Primitives.Point(0, 0), 1));
                                image.Mutate(x => x.DrawImage(masteriesImage, new SixLabors.Primitives.Point(0, statsGearIage.Height), 1));
                                await ReplyAndDeleteAsync("Thanks for the build!");
                                if (name.Content.ToLower() != "keep") build.name = name.Content;
                                if (instance.Content.ToLower() != "keep") build.instance = instance.Content;
                                build.authorID = Context.Message.Author.Id;
                                build.guid = guid;
                                if (exitStrings.Contains(note.Content.ToLower())) build.note = "N/A";
                                else if (note.Content.ToLower() != "keep") build.note = note.Content;
                                build.gearImage = null;
                                build.masteryImage = mastUrl;
                                build.statsImage = statsGearUrl;
                                build.authorID = Context.Message.Author.Id;
                                build.guid = guid;
                            }
                            var embed = await StopStreamReturnEmbedAsync(Context, Context.Message, image);
                            embed.Title = $"{build.name} {build.instance} build **edit** review.";
                            embed.AddField("Champion:", build.name);
                            embed.AddField("Instance:", build.instance);
                            embed.AddField("Created by:", Context.Client.GetUser(build.authorID).Username);
                            embed.AddField("Build notes:", build.note);
                            embed.AddField("GUID: ", build.guid);
                            buildImage = await ReplyAndDeleteAsync("", false, embed.Build(), timeout: TimeSpan.FromMinutes(60));
                            build.allImages = buildImage.Embeds.FirstOrDefault().Image.Value.Url;
                        }
                        editBuildList.Add(build);
                        editJsonData = JsonConvert.SerializeObject(editBuildList);
                        File.WriteAllText(editFilePath, editJsonData);
                    }

                    else
                    {
                        await ReplyAndDeleteAsync("Please respond with 2 or 3, so I know how images you have.  Try again.");
                    }

                }

                else
                {
                    var editFilePath = "builds.json";
                    var editJsonData = File.ReadAllText(editFilePath);
                    var editBuildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(editJsonData);
                    var build = editBuildList.FirstOrDefault(x => x.guid == guid);
                    var roles = (Context.Message.Author as SocketGuildUser).Roles;
                    if (build.authorID != Context.User.Id && !roles.Contains(Context.Guild.GetRole(514619966125768705)))
                    {
                        await ReplyAndDeleteAsync("You can only edit your own builds.");
                        return;
                    }
                    editBuildList.Remove(build);
                    await ReplyAndDeleteAsync("What do you want the new value to be?");
                    var input = await NextMessageAsync(true, true, TimeSpan.FromSeconds(30));
                    switch (part.ToLower())
                    {
                        case "name":
                            await ReplyAndDeleteAsync($"The name for your build has been updated {build.name}>>>{input}");
                            build.name = input.Content;
                            editBuildList.Add(build);
                            editJsonData = JsonConvert.SerializeObject(editBuildList);
                            File.WriteAllText(editFilePath, editJsonData);
                            return;
                        case "instance":
                            await ReplyAndDeleteAsync($"The name for your build has been updated {build.instance}>>>{input}");
                            build.instance = input.Content;
                            editBuildList.Add(build);
                            editJsonData = JsonConvert.SerializeObject(editBuildList);
                            File.WriteAllText(editFilePath, editJsonData);
                            return;
                        case "notes":
                            await ReplyAndDeleteAsync($"The name for your build has been updated {build.note}>>>{input}");
                            build.note = input.Content;
                            editBuildList.Add(build);
                            editJsonData = JsonConvert.SerializeObject(editBuildList);
                            File.WriteAllText(editFilePath, editJsonData);
                            return;
                        case "gear":
                            if (input.Content == "") build.gearImage = input.Attachments.FirstOrDefault().Url;
                            else build.gearImage = input.Content;
                            break;
                        case "stats":
                            if (input.Content == "") build.statsImage = input.Attachments.FirstOrDefault().Url;
                            else build.statsImage = input.Content;
                            break;
                        case "masteries":
                        case "mast":
                            if (input.Content == "") build.masteryImage = input.Attachments.FirstOrDefault().Url;
                            else build.masteryImage = input.Content;
                            break;
                    }
                    int images = 0;
                    Image<Rgba32> statsImage = new Image<Rgba32>(1, 1);
                    Image<Rgba32> masteryImage = new Image<Rgba32>(1, 1);
                    Image<Rgba32> gearImage = new Image<Rgba32>(1, 1);

                    if (build.statsImage != null)
                    {
                        statsImage = await StartStreamAsync(url: build.statsImage);
                        var newHeight = NewHeight(statsImage.Height, statsImage.Width);
                        statsImage.Mutate(x => x.Resize(800, (int)NewHeight(statsImage.Height, statsImage.Width)));
                        images += 1;
                    }
                    if (build.gearImage != null)
                    {
                        gearImage = await StartStreamAsync(url: build.gearImage);
                        var newHeight = NewHeight(gearImage.Height, gearImage.Width);
                        gearImage.Mutate(x => x.Resize(800, (int)NewHeight(gearImage.Height, gearImage.Width)));
                        images += 1;
                    }
                    if (build.masteryImage != null)
                    {
                        masteryImage = await StartStreamAsync(url: build.masteryImage);
                        var newHeight = NewHeight(masteryImage.Height, masteryImage.Width);
                        masteryImage.Mutate(x => x.Resize(800, (int)NewHeight(masteryImage.Height, masteryImage.Width)));
                        images += 1;
                    }
                    using (Image<Rgba32> image = new Image<Rgba32>(1, 1))
                    {

                        if (images > 2)
                        {
                            image.Mutate(x => x.Resize(800, (gearImage.Height + masteryImage.Height + statsImage.Height)));
                            image.Mutate(x => x.DrawImage(statsImage, new SixLabors.Primitives.Point(0, 0), 1));
                            image.Mutate(x => x.DrawImage(gearImage, new SixLabors.Primitives.Point(0, gearImage.Height), 1));
                            image.Mutate(x => x.DrawImage(masteryImage, new SixLabors.Primitives.Point(0, gearImage.Height + masteryImage.Height), 1));
                            var embed = await StopStreamReturnEmbedAsync(Context, Context.Message, image);
                            embed.Title = ("Heres the new image for your build.");
                            var buildImage = await ReplyAndDeleteAsync("", false, embed.Build(), timeout: TimeSpan.FromMinutes(10));
                            build.allImages = buildImage.Embeds.FirstOrDefault().Image.Value.Url;
                        }
                        else
                        {
                            image.Mutate(x => x.Resize(800, (gearImage.Height + masteryImage.Height + statsImage.Height)));
                            image.Mutate(x => x.DrawImage(statsImage, new SixLabors.Primitives.Point(0, 0), 1));
                            image.Mutate(x => x.DrawImage(masteryImage, new SixLabors.Primitives.Point(0, gearImage.Height), 1));
                            var embed = await StopStreamReturnEmbedAsync(Context, Context.Message, image);
                            embed.Title = ("Heres the new image for your build.");
                            var buildImage = await ReplyAndDeleteAsync("", false, embed.Build(), timeout: TimeSpan.FromMinutes(10));
                            build.allImages = buildImage.Embeds.FirstOrDefault().Image.Value.Url;
                        }
                    }
                    editBuildList.Add(build);
                    editJsonData = JsonConvert.SerializeObject(editBuildList);
                    File.WriteAllText(editFilePath, editJsonData);
                }
            }
            catch (Exception e)
            {
                var ran = new Random();
                var eEmbed = new EmbedBuilder();
                eEmbed.WithFooter(e.Message);
                eEmbed.Title = "Whoopsy...";
                eEmbed.AddField($"Error {ran.Next(10000, 99999)}:", "*sigh*, what did you do wrong this time...*");
                await ReplyAndDeleteAsync("", embed: eEmbed.Build());
            }
            //messageList.ForEach(x => x.DeleteAsync());
        }

        [Command("remove", RunMode = RunMode.Async)]
        public async Task RemoveBuild(string guid)
        {
            try
            {
                var roles = (Context.Message.Author as SocketGuildUser).Roles;
                if (!roles.Contains(Context.Guild.GetRole(540712854177841173)) && !roles.Contains(Context.Guild.GetRole(619927345838686209)) && Context.Message.Author.Id != 269643701888745474 && !roles.Contains(Context.Guild.GetRole(514619966125768705)))
                {
                    await ReplyAndDeleteAsync("Puny mortal you are not worthy of submitting builds!", timeout: TimeSpan.FromSeconds(10));
                    return;
                }

                var editFilePath = "builds.json";
                var editJsonData = File.ReadAllText(editFilePath);
                var editBuildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(editJsonData);
                var build = editBuildList.FirstOrDefault(x => x.guid == guid);

                if (build.authorID != Context.User.Id)
                {
                    await ReplyAndDeleteAsync("You can only remove your own builds.");
                    return;
                }

                editBuildList.Remove(build);
                editJsonData = JsonConvert.SerializeObject(editBuildList);
                File.WriteAllText(editFilePath, editJsonData);
                await ReplyAndDeleteAsync($"Your build for `{build.name}` with GUID#`{build.guid}` has been removed.");
            }
            catch (Exception e)
            {
                var ran = new Random();
                var eEmbed = new EmbedBuilder();
                eEmbed.WithFooter(e.Message);
                eEmbed.Title = "Whoopsy...";
                eEmbed.AddField($"Error {ran.Next(10000, 99999)}:", "*sigh*, I mean this is like the easiest command bro...*");
                await ReplyAndDeleteAsync("", embed: eEmbed.Build());
            }
        }

        [Command("champs", RunMode = RunMode.Async)]
        [Alias("list", "champions", "listchamps", "listhchampions")]
        public async Task GetCurrentChampList()
        {
            var champs = new List<string>();
            var filePath = "builds.json";
            var jsonData = File.ReadAllText(filePath);
            var buildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(jsonData) ?? new List<ChampionBuild>();
            buildList = buildList.OrderBy(x => x.name).ToList();
            foreach (var build in buildList)
            {
                if (!champs.Contains(build.name)) champs.Add(build.name);
                else continue;
            }
            var champString = "";
            foreach (var champ in champs)
            {
                champString += $" **{champ}** |";
            }
            await ReplyAndDeleteAsync($"Here is a list of champions with builds currently with ~{champs.Count} champions:\n|{champString}");
        }

        [Command("instances", RunMode = RunMode.Async)]
        [Alias("inst", "listinstances")]
        public async Task GetCurrentInstances()
        {
            var instances = new List<string>();
            var filePath = "builds.json";
            var jsonData = File.ReadAllText(filePath);
            var buildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(jsonData) ?? new List<ChampionBuild>();
            buildList = buildList.OrderBy(x => x.instance).ToList();
            foreach (var build in buildList)
            {
                if (!instances.Contains(build.instance)) instances.Add(build.instance);
                else continue;
            }
            var champString = "";
            foreach (var instance in instances)
            {
                champString += $" **{instance}** |";
            }
            await ReplyAndDeleteAsync($"Here is a list of instances with builds currently ~{instances.Count}:\n|{champString}");
        }
        [Command("get", RunMode = RunMode.Async)]
        public async Task GetBuild([Remainder]string championName = "")
        {
            try
            {
                var filePath = "builds.json";
                var jsonData = File.ReadAllText(filePath);
                var buildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(jsonData);
                var championBuilds = buildList.FindAll(x => x.name.ToLower().Contains(championName.ToLower()));
                var pages = new List<PaginatedMessage.Page>();
                foreach (var build in championBuilds)
                {
                    var page = new PaginatedMessage.Page();
                    page.ImageUrl = build.allImages;
                    page.Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "Champion:",
                        Value = build.name
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Created by:",
                        Value = Context.Client.GetUser( build.authorID).Username
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Instance:",
                        Value = build.instance
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Build notes:",
                        Value = build.note
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "GUID:",
                        Value = build.guid
                    },

            };
                    pages.Add(page);
                }
                var pager = new PaginatedMessage
                {
                    Pages = pages,
                    Options = new PaginatedAppearanceOptions { Timeout = TimeSpan.FromHours(1) }
                };

                await PagedReplyAsync(pager, new ReactionList
                {
                    Forward = true,
                    Backward = true,
                    Trash = true,
                });
            }
            catch (Exception e)
            {
                var ran = new Random();
                var eEmbed = new EmbedBuilder();
                eEmbed.WithFooter(e.Message);
                eEmbed.Title = "Whoopsy...";
                eEmbed.AddField($"Error {ran.Next(10000, 99999)}:", "I Couldn't find a champion with that name, you do know how to spel don't u humen?");
                await ReplyAndDeleteAsync("", embed: eEmbed.Build());
            }
        }


        [Command("getinstance", RunMode = RunMode.Async)]
        [Alias("geti", "getinst")]
        public async Task GetInstanceBuilds(string instanceName)
        {
            try
            {
                var filePath = "builds.json";
                var jsonData = File.ReadAllText(filePath);
                var buildList = JsonConvert.DeserializeObject<List<ChampionBuild>>(jsonData);
                var championBuilds = buildList.Where(x => x.instance.ToLower() == instanceName.ToLower()).OrderBy(x => x.name);
                var pages = new List<PaginatedMessage.Page>();
                foreach (var build in championBuilds)
                {
                    var page = new PaginatedMessage.Page();
                    page.ImageUrl = build.allImages;
                    page.Fields = new List<EmbedFieldBuilder>
                {
                    new EmbedFieldBuilder
                    {
                        Name = "Champion:",
                        Value = build.name
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Created by:",
                        Value = Context.Client.GetUser( build.authorID).Username
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Instance:",
                        Value = build.instance
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "Build notes:",
                        Value = build.note
                    },
                    new EmbedFieldBuilder
                    {
                        Name = "GUID:",
                        Value = build.guid
                    },

            };
                    pages.Add(page);
                }
                var pager = new PaginatedMessage
                {
                    Pages = pages,
                    Options = new PaginatedAppearanceOptions { Timeout = TimeSpan.FromHours(1) }
                };

                await PagedReplyAsync(pager, new ReactionList
                {
                    Forward = true,
                    Backward = true,
                    Trash = true,
                });
            }
            catch (Exception e)
            {
                var ran = new Random();
                var eEmbed = new EmbedBuilder();
                eEmbed.WithFooter(e.Message);
                eEmbed.Title = "Whoopsy...";
                eEmbed.AddField($"Error {ran.Next(10000, 99999)}:", "I Couldn't find a instance with that name,  lern 2 spel :facepalm:?");
                await ReplyAndDeleteAsync("", embed: eEmbed.Build());
            }
        }

        [Command("damage", RunMode = RunMode.Async)]
        public async Task UpdateCBDamage(string url = null)
        {
            try
            {
                if (url == null)
                {
                    await ReplyAndDeleteAsync("Would you kindly upload an SS of CB damage, TY.");
                    var ss = await NextMessageAsync();
                    if (ss.Content == "") url = ss.Attachments.FirstOrDefault().Url;
                    else if (ss.Content.ToLower() == "keep") url = ss.Content;
                }
                HttpClient httpClient = new HttpClient(); /*Creates a new HttpClient*/
                HttpResponseMessage response = null;
                SixLabors.ImageSharp.Image<Rgba32> image = null; /*Creates a null ImageSharp image*/
                response = await httpClient.GetAsync(url); /*sets the response to the url*/
                Stream inputStream = await response.Content.ReadAsStreamAsync(); /*creates a inputStream variable and reads the url*/
                image = SixLabors.ImageSharp.Image.Load<Rgba32>(inputStream); /*Loads the image to the ImageSharp image we created earlier*/
                image.Mutate(x => x.Resize(image.Width * 2, image.Height * 2));
                image.Mutate(x => x.BinaryThreshold(.5f));
                Stream outputStream = new MemoryStream();
                image.SaveAsPng(outputStream); /*saves the image as a jpg you can of course change this*/
                outputStream.Position = 0;
                var file = File.Create("./images/upload.png"); /*creates a file with the random string as the name*/
                await outputStream.CopyToAsync(file);
                file.Dispose();
                /*deletes the image after sending*/
                inputStream.Dispose();
                var text = "";
                using (var engine = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractOnly))
                {
                    using (var img = Pix.LoadFromFile("./images/upload.png"))
                    {
                        using (var page = engine.Process(img))
                        {
                            text = page.GetText();
                        }
                    }
                }
                List<string> lines = text.Split("\n").ToList();
                var boss = lines.Find(x => x.Contains("Demon Lord"));
                boss = boss.Replace("u, Skip>", "", true, CultureInfo.CurrentCulture);
                boss = boss.Remove(boss.IndexOf(',') - 1);
                var damage = lines.Find(x => x.Contains("Damage"));
                //damage = damage.Replace("é", "e");
                var output = await ReplyAndDeleteAsync($"{Context.User.Username}:\n{boss}\n{damage}");
                damageList.Add(output.Content);
                File.Delete($"./images/upload.png");
            }

            catch
            {
                await ReplyAndDeleteAsync("Yea... That did't work.. Its prbably bad image quailty try a better SS.  Are you playing on a flip phone?");
            }
        }
        [Command("check", RunMode = RunMode.Async)]
        public async Task CheckDamageList()
        {
            var output = "";
            foreach (var i in damageList)
            {
                output += $"{i}\n";
            }
            await ReplyAndDeleteAsync(output);
        }
    }
}




