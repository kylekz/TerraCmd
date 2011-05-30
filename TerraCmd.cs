using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;
using System.IO;

namespace Terraria
{
    class TerraCmd : Plugin
    {

        public string opTag;
        public string opTagColor;
        public ArrayList rules = new ArrayList();

        public override void Initialize()
        {
            pluginName = "TerraCmd";
            pluginDescription = "Misc. commands for tMod";
            pluginAuthor = "Kaikz";
            pluginVersion = "v1.0";

            this.registerHook(Hook.PLAYER_COMMAND);
            this.registerHook(Hook.PLAYER_CHAT);

            opTag = ConfigurationManager.AppSettings.Get("opTag");
            opTagColor = ConfigurationManager.AppSettings.Get("opTagColor");

            string loaded = "loaded!";
            Console.WriteLine(string.Concat(new object[] { ConsoleColor.Red, "[TerraCmd] ", loaded }));
        }

        public override void Unload()
        {
            string unloaded = "unloaded!";
            Console.WriteLine(string.Concat(new object[] { ConsoleColor.Red, "[TerraCmd] ", unloaded }));
        }

        public bool equalsIgnoreCase(string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        public Player getPlayerFromString(string p)
        {
            foreach (Player player in Main.player)
            {
                if (this.equalsIgnoreCase(player.name, p))
                {
                    return player;
                }
            }
            return null;
        }

        public override void onPlayerCommand(CommandEvent ev)
        {
            string[] cmd = ev.getCommandArray();
            Player player = ev.getPlayer();
            if (cmd[0].ToLower() == "/list" || cmd[0] == "/playerlist" || cmd[0].ToLower() == "/online" || cmd[0].ToLower() == "/players")
            {
                string str = "";
                for (int i = 0; i < 8; i++)
                {
                    if (Main.player[i].active)
                    {
                        if (str == "")
                        {
                            str = str + Main.player[i].name;
                        }
                        else
                        {
                            str = str + ", " + Main.player[i].name;
                        }
                    }
                }
                ev.getPlayer().sendMessage("Players Online: " + str + ".", 0xff, 240, 20);
                ev.setState(true);
            }
            else if (cmd[0].ToLower() == "/rules")
            {
                loadRules(ev.getPlayer());
                foreach (string stringRules in this.rules)
                {
                    player.sendMessage(stringRules, 51, 255, 0);
                }
                ev.setState(true);
            }
            else if (cmd[0] == "/heal")
            {
                ev.getPlayer().statLife = ev.getPlayer().statLifeMax;
                ev.getPlayer().statMana = ev.getPlayer().statManaMax;
                ev.getPlayer().sendMessage("You have just been healed!", 51, 255, 0);
                NetMessage.syncPlayers();
                ev.setState(true);
            }
            else if (cmd[0] == "/kill" && cmd[1] != null)
            {
                Player killPlayer = getPlayerFromString(cmd[1]);
                if (killPlayer.active)
                {
                    killPlayer.Hurt(99999, 0, true, false);
                    NetMessage.syncPlayers();
                    ev.setState(true);
                }
                else
                {
                    ev.getPlayer().sendMessage("Player: " + killPlayer + " isn't online!", 51, 255, 0);
                    ev.setState(true);
                }
            }
            else if (cmd[0] == "/god" && ev.getPlayer().isOP)
            {
                if (cmd[1] == "on")
                {
                    ev.getPlayer().statLife = 99999;
                    ev.getPlayer().statMana = 99999;
                    ev.getPlayer().noFallDmg = true;
                    ev.getPlayer().noKnockback = true;
                    ev.getPlayer().breath = 99999;
                    NetMessage.syncPlayers();
                    ev.getPlayer().sendMessage("God mode activated!", 51, 255, 0);
                    ev.setState(true);
                }
                else if (cmd[1] == "off")
                {
                    ev.getPlayer().statLife = ev.getPlayer().statLifeMax;
                    ev.getPlayer().statMana = ev.getPlayer().statManaMax;
                    ev.getPlayer().noFallDmg = false;
                    ev.getPlayer().noKnockback = false;
                    ev.getPlayer().breath = ev.getPlayer().breathMax;
                    NetMessage.syncPlayers();
                    ev.getPlayer().sendMessage("God mode deactivated!", 51, 255, 0);
                    ev.setState(true);
                }
            }
        }

        public override void onPlayerChat(ChatEvent ev)
        {
            string str = ev.getChat();
            Player player = ev.getPlayer();

            if (player.isOP)
            {
                switch (opTagColor)
                {
                    case "Red":
                        NetMessage.broadcastMessage(string.Concat(new object[] { "<", ConsoleColor.Red, opTag, player.name, "> ", str }));
                        break;
                    case "Blue":
                        NetMessage.broadcastMessage(string.Concat(new object[] { "<", ConsoleColor.Blue, opTag, player.name, "> ", str }));
                        break;
                    case "Green":
                        NetMessage.broadcastMessage(string.Concat(new object[] { "<", ConsoleColor.Green, opTag, player.name, "> ", str }));
                        break;
                    case "Cyan":
                        NetMessage.broadcastMessage(string.Concat(new object[] { "<", ConsoleColor.Cyan, opTag, player.name, "> ", str }));
                        break;
                    case "Magenta":
                        NetMessage.broadcastMessage(string.Concat(new object[] { "<", ConsoleColor.Magenta, opTag, player.name, "> ", str }));
                        break;
                    case "Yellow":
                        NetMessage.broadcastMessage(string.Concat(new object[] { "<", ConsoleColor.Yellow, opTag, player.name, "> ", str }));
                        break;
                    default:
                        NetMessage.broadcastMessage(string.Concat(new object[] { "<", ConsoleColor.Red, opTag, player.name, "> ", str }));
                        break;
                }
            }
        }

        public void loadRules(Player player)
        {
            string rulesFile = "rules.txt";
            string rulesOutput;
            if (!File.Exists(rulesFile))
            {
                player.sendMessage("Rules file not found! Create a rules.txt file!", 255, 0, 0);
            }
            else
            {
                TextReader reader = new StreamReader(rulesFile);
                while ((rulesOutput = reader.ReadLine()) != null)
                {
                    this.rules.Add(rulesOutput);
                }
            }
        }
    }
}