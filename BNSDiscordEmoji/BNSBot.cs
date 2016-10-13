using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BNSDiscordEmoji
{
    class BNSBot
    {
        DiscordClient discord;

        public BNSBot()
        {
            discord = new DiscordClient(x => 
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '~';
                x.AllowMentionPrefix = true;
            });

            var commands = discord.GetService<CommandService>();

            commands.CreateCommand("Hello!").Do(async (e) => 
            {
                await e.Channel.SendMessage("Hi");
            });

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("INSERT TOKEN HERE!!!!", TokenType.Bot);
            });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
