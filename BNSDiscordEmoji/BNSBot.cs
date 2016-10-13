using Discord;
using Discord.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BNSDiscordEmoji
{
    class BNSBot
    {
        DiscordClient discord;
        CommandService commands;

        string[] bnsEmojis = Directory.GetFiles("emojis", "*.png")
                                        .Select(path => Path.GetFileName(path))
                                        .ToArray();

        public BNSBot()
        {
            discord = new DiscordClient(x => 
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            discord.UsingCommands(x =>
            {
                x.PrefixChar = '/';
                x.AllowMentionPrefix = true;
            });

            commands = discord.GetService<CommandService>();

            commands.CreateCommand("Hello").Do(async (e) => 
            {
                await e.Channel.SendMessage("Hi");
            });

            BNSEmojiCommand();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("INSERT TOKEN HERE!!!!", TokenType.Bot);
            });
        }

        private void BNSEmojiCommand()
        {
            foreach (string emoji in bnsEmojis)
            {
                string command = emoji.Substring(0, emoji.LastIndexOf(".png"));
                commands.CreateCommand(command).Do(async (e) =>
                {
                    string emojiToPost = "emojis/" + emoji;
                    await e.Channel.SendFile(emojiToPost);
                });
            }
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
