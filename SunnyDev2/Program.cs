using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            Console.ReadLine();
        }
    }

    class Bot
    {
        TwitchClient client;

        public Bot()
        {
            ConnectionCredentials credentials = new ConnectionCredentials("The_Thousand_Sunny_Bot", "REDACTED");
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials);

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.Connect();
            client.JoinChannel("tukeloomey"); /// Add channel name here to connect, do NOT use the full link.


        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            client.SendMessage(e.Channel, "Hey guys! Just testing to make sure I work, don't mind me :) Have a free view!");
        }


        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Random rnd = new Random(); // Preparing to use the random function
            string[] badword = { "redacted", "redacted", "redacted" }; // These three lines are creating arrays for racial slurs the bot will autoban for using
            string[] basic = { "!hello", "!Hello!", "!Hello", "!Hi", "!hi", "!Sup" };
            string[] genericresponse = { "Hello there!", "Beep boop I'm a ship not a robot", "Hello!", "Welcome and greetings!" };
            string help = ("!Help");
            int genericindex = rnd.Next(genericresponse.Length); // Split array into index so response can be easily randomized
            int helpindex = rnd.Next(genericresponse.Length); // Split array into index so response can be easily randomized

            List<string> commands = new List<string>(); // Creates a list so commands can be added

            commands.Add("!Hello"); //Adds commands to the list, this will return a list of commands that currently function to users if !Help is typed
            commands.Add("!hello");
            commands.Add("!Hi");
            commands.Add("!hi");

            if (e.ChatMessage.Message.Contains(help))
                client.SendMessage(e.ChatMessage.Channel, "The current commands are: " + string.Join(", ", commands)); // Responding based on words in the list, providing full list to user. The list returned is current commands they can use.; 

            foreach (string x in badword) // Looking for words stored in the array

            {
                if (e.ChatMessage.Message.Contains(x))
                    client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "We don't tolerate those words here. Cya in a bit ;)"); // Responding to words from the array
            }
            foreach (string x in basic) // Looking for words stored in the array
            {
                if (e.ChatMessage.Message.Contains(x))
                    client.SendMessage(e.ChatMessage.Channel, genericresponse[helpindex]); // Responding based on words in array, at random
            }           
        }
    
        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "wordofkylar")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
    }
}