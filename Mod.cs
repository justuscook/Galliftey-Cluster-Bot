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

namespace GCB
{
    public class Mod : InteractiveBase<SocketCommandContext>
    {
    }
}