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
                                        .Select(path => Path.GetFileNameWithoutExtension(path))
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
            
            BNSMessageCommand();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("INSERT TOKEN HERE!!!!", TokenType.Bot);
            });
        }

        private void BNSMessageCommand()
        {
            discord.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor)
                {
                    string message = e.Message.ToString();
                    string[] messageContents = message.Substring(message.IndexOf(":") + 1).Split(' ');

                    foreach (string word in messageContents)
                    {
                        if (word.Length!= 0 && bnsEmojis.Contains(word.Substring(1)))
                        {
                            await e.Channel.SendFile("emojis" + word + ".png");
                        }
                    }
                }
            };
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
