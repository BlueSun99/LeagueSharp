using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace ToolsForDevelopers
{
    class Program
    {
        private static Menu config;
        private static Menu buyItemConfig;

        private static readonly string fileLoggingSend = "LogSend.txt";
        private static readonly string fileLoggingReceive = "LogReceive.txt";

        private static GamePacket tempData;

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

            config.AddItem(new MenuItem("GetUnitVector", "Get Unit Vector").SetValue(false));
            config.AddItem(new MenuItem("GetMouseVector", "Get Mouse Vector").SetValue(false));
            config.AddItem(new MenuItem("WIP", "WIP").SetValue(false));

            config.AddToMainMenu();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnSendPacket += Game_OnSendPacket;
            Game.OnProcessPacket += Game_OnProcessPacket;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (config.Item("GetUnitVector").GetValue<bool>())
            {
                Game.PrintChat(ObjectManager.Player.Position.ToString());
            }
			
            if (config.Item("GetMouseVector").GetValue<bool>())
            {
                Game.PrintChat(Game.CursorPos.ToString());
            }

            if (config.Item("WIP").GetValue<bool>() && ObjectManager.Player.Team == GameObjectTeam.Chaos)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, new Vector3(396f, 462f, 182.1325f));
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

                /*if (Config.LeagueSharpDirectory != null &&
                    Directory.Exists(Config.LeagueSharpDirectory))
                {
                    File.AppendAllText(Path.Combine(Config.LeagueSharpDirectory, fileLoggingSend), gPacket.Dump() + Environment.NewLine);
                }*/
            }
        }

        public static void Game_OnProcessPacket(GamePacketEventArgs args)
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

                /*if (Config.LeagueSharpDirectory != null &&
                    Directory.Exists(Config.LeagueSharpDirectory))
                {
                    File.AppendAllText(Path.Combine(Config.LeagueSharpDirectory, fileLoggingReceive), gPacket.Dump() + Environment.NewLine);
                }*/
            }
        }
    }
}
