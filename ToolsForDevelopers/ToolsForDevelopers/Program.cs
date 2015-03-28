using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace ToolsForDevelopers
{
    class Program
    {
        private static Menu config;
        private static Menu buyItemConfig;

        private static readonly string fileLoggingSend = "LogSend.txt";
        private static readonly string fileLoggingReceive = "LogReceive.txt";

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            config = new Menu("Tools For Developers", "ToolsForDevelopers", true);

            config.AddItem(new MenuItem("SendPacket", "Log Send Packet").SetValue(false));
            config.AddItem(new MenuItem("SendPacket_Console", " ^ Console Write?").SetValue(false));

            config.AddItem(new MenuItem("RecvPacket", "Log Recv Packet").SetValue(false));
            config.AddItem(new MenuItem("RecvPacket_Console", " ^ Console Write?").SetValue(false));
            config.AddItem(new MenuItem("RecvPacket_OnlyMine", " ^ Only My Network Id").SetValue(false));

            config.AddItem(new MenuItem("MoveToPos", "Move To Position").SetValue(false));

            config.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnSendPacket += Game_OnSendPacket;
            Game.OnGameProcessPacket += Game_OnGameProcessPacket;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (config.Item("MoveToPos").GetValue<bool>())
            {
                Game.PrintChat(Game.CursorPos.ToString());
            }
        }

        public static void Game_OnSendPacket(GamePacketEventArgs args)
        {
            if (config.Item("SendPacket").GetValue<bool>())
            {
                var gPacket = new GamePacket(args);

                if (config.Item("SendPacket_Console").GetValue<bool>())
                {
                    Console.WriteLine("---- Logging Send Packet ----");
                    Console.WriteLine(gPacket.Dump());
                }

                if (Config.LeagueSharpDirectory != null &&
                    Directory.Exists(Config.LeagueSharpDirectory))
                {
                    File.AppendAllText(Path.Combine(Config.LeagueSharpDirectory, fileLoggingSend), gPacket.Dump() + Environment.NewLine);
                }
            }
        }

        public static void Game_OnGameProcessPacket(GamePacketEventArgs args)
        {
            if (config.Item("RecvPacket").GetValue<bool>())
            {
                var gPacket = new GamePacket(args);

                if (config.Item("RecvPacket_OnlyMine").GetValue<bool>() && gPacket.ReadInteger(2) != ObjectManager.Player.NetworkId)
                    return;

                if (config.Item("RecvPacket_Console").GetValue<bool>())
                {
                    Console.WriteLine("---- Logging Receive Packet ----");
                    Console.WriteLine(gPacket.Dump());
                }

                if (Config.LeagueSharpDirectory != null &&
                    Directory.Exists(Config.LeagueSharpDirectory))
                {
                    File.AppendAllText(Path.Combine(Config.LeagueSharpDirectory, fileLoggingReceive), gPacket.Dump() + Environment.NewLine);
                }
            }
        }
    }
}
