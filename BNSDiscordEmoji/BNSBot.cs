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
        String dirQuestions = "questions";
        String fileQuestion = "question.txt";

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
            QuizQuestion();
            PostQuestion();

            discord.ExecuteAndWait(async () =>
            {
                await discord.Connect("MjM2MjAwOTU1NjY4OTg3OTA0.CuFsPA.V77jPzGR8grLwMRwZKV3c8dRtmM", TokenType.Bot);
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

        private void QuizQuestion()
        {
            commands.CreateCommand("dailyquestion").Do(async (e) => 
            {
                String question = File.ReadAllText(Directory.GetCurrentDirectory() + "\\" + dirQuestions + "\\" + fileQuestion);
                await e.Channel.SendMessage(question);
            });
        }

        private void PostQuestion()
        {
            commands.CreateCommand("postquestion")
                .Parameter("Question", ParameterType.Required)
                .Do(async (e) =>
                {
                    if (!Directory.Exists(dirQuestions))
                    {
                        Directory.CreateDirectory(dirQuestions);
                    }

                    String message = e.GetArg("Question");

                    StreamWriter file = new StreamWriter(Directory.GetCurrentDirectory() + "\\" + dirQuestions + "\\" + fileQuestion);
                    file.Write(message);
                    file.Close();

                    await e.Channel.SendMessage("Got it!");
                });
        }

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
