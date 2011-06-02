using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections;
using System.IO;
using System.Threading;

namespace Terraria
{
    class TerraCmd : Plugin
    {
        // Settings
        public Dictionary<string, string> settings;
        public string settingsPath = "terracmd.txt";
        // OP config
        public string opTag;
        public int opR, opG, opB;
        // motd config
        public int motdR, motdG, motdB;
        // Save config
        public int saveInterval = 600000;
        private Thread saveThread;
        // Anti-hack config
        public string kickorban;

        public override void Initialize()
        {
            pluginName = "TerraCmd";
            pluginDescription = "Misc. commands for tMod";
            pluginAuthor = "Kaikz";
            pluginVersion = "v1.1";

            this.registerHook(Hook.PLAYER_COMMAND);
            this.registerHook(Hook.PLAYER_CHAT);
            this.registerHook(Hook.PLAYER_JOIN);

            loadSettings();

            Console.WriteLine("[TerraCmd] " + pluginVersion + " loaded!");

            this.saveThread = new Thread(new ThreadStart(this.saveMap));
            this.saveThread.Start();
        }

        public override void Unload()
        {
            Console.WriteLine("[TerraCmd] " + pluginVersion + " unloaded!");
        }

        public bool equalsIgnoreCase(string a, string b)
        {
            return a.Equals(b, StringComparison.OrdinalIgnoreCase);
        }

        public void motd(Player p)
        {
            string msg = this.settings["motd"];
            msg = msg.Replace("[player]", p.name).Replace("[server]", Main.getIP);
            p.sendMessage(msg, motdR, motdG, motdB);
        }

        public void saveMap()
        {
            while (true)
            {
                Thread.Sleep(this.saveInterval);
                NetMessage.broadcastMessage("Saving world... this might lag for a minute!");
                WorldGen.saveWorld(false);
            }
        }

        public void loadSettings()
        {
            this.settings = new Dictionary<string,string>();
            this.settings.Clear();
            if (!File.Exists(settingsPath))
            {
                TextWriter writer = new StreamWriter(settingsPath);
                writer.WriteLine("opTag=[OP]");
                writer.WriteLine("opR=255");
                writer.WriteLine("opG=0");
                writer.WriteLine("opB=0");
                writer.WriteLine("motd=Welcome to [server], [player]!");
                writer.WriteLine("motdR=51");
                writer.WriteLine("motdG=255");
                writer.WriteLine("motdB=0");
                writer.WriteLine("antihack=kick");
                writer.Close();
            }

            foreach (string str2 in File.ReadAllLines(settingsPath))
            {
                this.settings.Add(str2.Split(new char[] { '=' })[0], str2.Split(new char[] { '=' })[1]);
            }

            // OP Settings
            opTag = this.settings["opTag"];
            opR = int.Parse(this.settings["opR"]);
            opG = int.Parse(this.settings["opG"]);
            opB = int.Parse(this.settings["opB"]);

            // MOTD Settings
            motdR = int.Parse(this.settings["motdR"]);
            motdG = int.Parse(this.settings["motdG"]);
            motdB = int.Parse(this.settings["motdB"]);

            // Anti-hack Settings
            kickorban = this.settings["antihack"];
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
            if (cmd[0].ToLower() == "/list" || cmd[0] == "/playerlist" || cmd[0].ToLower() == "/online" || cmd[0].ToLower() == "/players" && player.hasPermissions("terracmd.playerlist"))
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
            else if (cmd[0] == "/terracmd")
            {
                if (cmd[1] == null)
                {
                    ev.getPlayer().sendMessage(pluginName + " v" + pluginVersion + " by " + pluginAuthor, 51, 255, 0);
                }
                else if (cmd[1] == "reload" && ev.getPlayer().isOP || player.hasPermissions("terracmd.reload"))
                {
                    loadSettings();
                }
            }
            else if (cmd[0] == "/password" && ev.getPlayer().isOP || player.hasPermissions("terracmd.password"))
            {
                if (cmd[1] == null)
                {
                    Netplay.password = "";
                    ev.getPlayer().sendMessage("Server password removed!", 0, 255, 0);
                }
                else if (cmd[1].Length > 3)
                {
                    Netplay.password = cmd[1];
                    ev.getPlayer().sendMessage("Password reset to: " + cmd[1] + "!", 0, 255, 0);
                }
                else
                {
                    ev.getPlayer().sendMessage("Invalid password!", 0, 255, 0);
                }
            }
        }

        public override void onPlayerChat(ChatEvent ev)
        {
            string str = ev.getChat();
            Player player = ev.getPlayer();

            if (player.isOP || player.hasPermissions("op") || player.hasPermissions("terracmd.op.color"))
            {
                foreach (Player player2 in Main.player)
                {
                    if (player2.name.Length > 0)
                    {
                        player2.sendMessage("<" + opTag + player.name + "> " + str, opR, opG, opB);
                    }
                }
                ev.setState(true);
            } else
            {
                base.onPlayerChat(ev);
                ev.setState(true);
            }
        }

        public override void onPlayerJoin(PlayerEvent ev)
        {
            this.motd(ev.getPlayer());
            if (this.settings["antihack"] == "kick" && !ev.getPlayer().hasPermissions("terracmd.antihack.bypass"))
            {
                if (ev.getPlayer().statLifeMax > 400 | ev.getPlayer().statManaMax > 200)
                {
                    ev.getPlayer().kick("You were kicked for hacked health and/or mana!");
                    Console.WriteLine(ev.getPlayer().name + " was kicked for hacking their health or mana!");
                }
            }
        }
    }

    public enum NPCCode
    {
        AngryBones = 0x1f,
        ArmsDealer = 0x13,
        BlueSlime = 1,
        BoneSerpentBody = 40,
        BoneSerpentHead = 0x27,
        BoneSerpentTail = 0x29,
        BuringSkull = 0x22,
        BurningSphere = 0x19,
        ChaosBall = 30,
        DarkCaster = 0x20,
        Demolitionist = 0x26,
        DemonEye = 2,
        DevourerBody = 8,
        DevourerHead = 7,
        DevourerTail = 9,
        Dryad = 20,
        EaterOfSouls = 6,
        EaterOfWorldaTail = 15,
        EaterOfWorldsBody = 14,
        EaterOfWorldsHead = 13,
        EyeOfCthulhu = 4,
        FireImp = 0x18,
        GiantWormBody = 11,
        GiantWormHead = 10,
        GiantWormTail = 12,
        GoblinPeon = 0x1a,
        GoblinSorcerer = 0x1d,
        GoblinThief = 0x1b,
        GoblinWarrior = 0x1c,
        Guide = 0x16,
        Hornet = 0x2a,
        ManEater = 0x2b,
        Merchant = 0x11,
        MeteorHead = 0x17,
        MotherSlime = 0x10,
        Nurse = 0x12,
        OldMan = 0x25,
        ServantOfCthulu = 5,
        Skeleton = 0x15,
        SkeletronHand = 0x24,
        SkeletronHead = 0x23,
        WaterSphere = 0x21,
        Zombie = 3
    }
}