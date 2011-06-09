using System;

using System.Collections.Generic;

using System.Linq;

using System.Text;

using System.Configuration;

using System.Collections.Specialized;

using System.Collections;

using System.IO;

using System.Threading;

using Permissions;



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

		public bool opSettings;

	// motd config

	public int motdR, motdG, motdB;

	public string motdString;

	// Save/backups config

	public int saveInterval;

	private Thread saveThread;

	public bool saving;

	public int backupInterval;

	private Thread backupThread;

	public bool backuping;

	public string worldName;

	public DateTime timeNow;

	public string backupPath = "plugins/TerraCmd/Backups";

	// Anti-hack config

	public string kickorban;



	public override void Initialize()

	{

	    pluginName = "TerraCmd";

	    pluginDescription = "Misc. commands for tMod";

	    pluginAuthor = "Kaikz";

	    pluginVersion = "v1.3";



	    this.registerHook(Hook.PLAYER_COMMAND);

	    this.registerHook(Hook.PLAYER_CHAT);

	    this.registerHook(Hook.PLAYER_JOIN);



	    loadSettings();



	    Console.WriteLine("[TerraCmd] " + pluginVersion + " loaded!");

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

	    string msg = motdString;

	    msg = msg.Replace("[player]", p.name);

	    msg = msg.Replace("[server]", Main.getIP);

	    p.sendMessage(msg, motdR, motdG, motdB);

	}



	public void saveMap()

	{

	    while (saving == true)

	    {

		Thread.Sleep(this.saveInterval);

		NetMessage.broadcastMessage("Saving world... this might lag for a minute!");

		WorldGen.saveWorld(false);

	    }

	}



	public void backupMap()

	{

	    while (backuping == true)

	    {

		timeNow = DateTime.Now;

		string timeString = timeNow.ToString();

		if (!System.IO.Directory.Exists("plugins/TerraCmd"));

		{

		    System.IO.Directory.CreateDirectory("plugins/TerraCmd");

		}

		if (!System.IO.Directory.Exists("plugins/TerraCmd/Backups"));

		{

		    System.IO.Directory.CreateDirectory("plugins/TerraCmd/Backups");

		}

		Thread.Sleep(this.backupInterval);

		string sourcePath = worldName;

		string destPath = "plugins/TerraCmd/Backups/" + timeString + "-" + worldName;

		System.IO.File.Copy(sourcePath, destPath, true);

		Console.WriteLine("[TerraCmd] " + worldName + " backed up to " + destPath + "!");

	    }

	}



	protected string getSetting(string setting, string settingValue)

	{

	    try

	    {

		string ret = settings[setting];

		return ret;

	    }

	    catch (Exception e)

	    {

		Console.WriteLine("[TerraCmd] Missing setting: " + setting);

		TextWriter t = new StreamWriter(settingsPath, true);

		t.WriteLine(setting + "=" + settingValue);

		t.Close();

		this.settings.Add(setting, settingValue);

		return settingValue;

	    }

	}



	public void loadSettings()

	{

	    this.settings = new Dictionary<string,string>();

	    this.settings.Clear();

	    if (!File.Exists(settingsPath))

	    {

		TextWriter writer = new StreamWriter(settingsPath);

				writer.WriteLine("opSettings=false");

		writer.WriteLine("opTag=[OP]");

		writer.WriteLine("opR=255");

		writer.WriteLine("opG=0");

		writer.WriteLine("opB=0");

		writer.WriteLine("motd=Welcome to our server, [player]!");

		writer.WriteLine("motdR=51");

		writer.WriteLine("motdG=255");

		writer.WriteLine("motdB=0");

		writer.WriteLine("antihack=kick");

		writer.WriteLine("saveInterval=600000");

		writer.WriteLine("savingEnabled=true");

		writer.WriteLine("backupInterval=1800000");

		writer.WriteLine("backupsEnabled=false");

		writer.Close();

	    }



	    foreach (string str2 in File.ReadAllLines(settingsPath))

	    {

		this.settings.Add(str2.Split(new char[] { '=' })[0], str2.Split(new char[] { '=' })[1]);

	    }



	    // OP Settings

			opSettings = bool.Parse(getSetting("opSettings", "false"));

	    opTag = getSetting("opTag", "[OP]");

	    opR = int.Parse(getSetting("opR", "255"));

	    opG = int.Parse(getSetting("opG", "0"));

	    opB = int.Parse(getSetting("opB", "0"));

	    // MOTD Settings

	    motdString = getSetting("motd", "Welcome to our server, [player]!");

	    motdR = int.Parse(getSetting("motdR", "51"));

	    motdG = int.Parse(getSetting("motdG", "255"));

	    motdB = int.Parse(getSetting("motdB", "0"));

	    // Anti-hack Settings

	    kickorban = getSetting("antihack", "kick");

	    // Save/Backup Settings

	    saveInterval = int.Parse(getSetting("saveInterval", "600000"));

	    saving = bool.Parse(getSetting("savingEnabled", "true"));

	    backupInterval = int.Parse(getSetting("backupInterval", "1800000"));

	    backuping = bool.Parse(getSetting("backupsEnabled", "false"));



	    if (saving == true)

	    {

		this.saveThread = new Thread(new ThreadStart(this.saveMap));

		this.saveThread.Start();

	    }



	    //if (backuping == true)

	    //{

	    //	  worldName = Main.properties["worldName"];

	    //	  this.backupThread = new Thread(new ThreadStart(this.backupMap));

	    //	  this.backupThread.Start();

	    //}

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

	    if ((cmd[0] == "/list" || cmd[0] == "/playerlist" || cmd[0] == "/online" || cmd[0] == "/players") && (player.hasPermissions("terracmd.playerlist")))

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

		player.sendMessage("Current players: " + str + ".", 0xff, 240, 20);

		ev.setState(true);

	    }

	    else if ((cmd[0] == "/terracmd") && (player.isOP || player.hasPermissions("terracmd.reload")))

	    {

		loadSettings();

		player.sendMessage(pluginName + " " + pluginVersion + " -- Reloaded!", 51, 255, 0);

		ev.setState(true);

	    }

	    else if ((cmd[0] == "/password") && (player.isOP || player.hasPermissions("terracmd.password")))

	    {

		if (cmd[1] == null)

		{

		    Netplay.password = "";

		    player.sendMessage("Server password removed!", 51, 255, 0);

		    ev.setState(true);

		}

		else if (cmd[1].Length > 3)

		{

		    Netplay.password = cmd[1];

		    player.sendMessage("Password reset to: " + cmd[1] + "!", 51, 255, 0);

		    ev.setState(true);

		}

		else

		{

		    player.sendMessage("Invalid password!", 255, 0, 0);

		    ev.setState(true);

		}

	    }

	    else if ((cmd[0] == "/meteor") && (player.isOP || player.hasPermissions("terracmd.meteor")))
	    {
		WorldGen.dropMeteor();
		ev.setState(true);
	    }
	    else if ((cmd[0] == "/bloodmoon) && (player.isOP || player.hasPermissions("terracmd.bloodmoon")))
	    {
		if (Main.bloodmoon)
		{
		    Main.bloodmoon = false;
		    ev.setState(true);
		}
		else
		{
		    Main.bloodmoon = true;
		    ev.setState(true);
		}
	}



	public override void onPlayerChat(ChatEvent ev)

	{

			if (opSettings == true)

			{

		string str = ev.getChat();

		Player player = ev.getPlayer();



		if (!AuthManager.isLoggedIn(player))

		{ 

		    return;

		}



		if (player.isOP)

		{

		    Player[] player2 = Main.player;

		    Player[] players = player2;

		    for (int i = 0; i < players.Length; i++)

		    {

			Player player3 = players[i];

		       if (player3.name.Length > 0)

			{

			      player3.sendMessage("<" + opTag + player.name + "> " + str, opR, opG, opB);

			 }

		      }

			ev.setState(true);

			return;

		}

		else

		{

			Player[] player4 = Main.player;

			Player[] players2 = player4;

			for (int i2 = 0; i2 < players2.Length; i2++)

			{

			Player player5 = players2[i2];

			if (player5.name.Length > 0)

			{

				player5.sendMessage("<" + player.name + "> " + str, 255, 255, 255);

			}

			}

			ev.setState(false);

		}

			}

	}



	public override void onPlayerJoin(PlayerEvent ev)

	{

	    this.motd(ev.getPlayer());

	    if (kickorban == "kick" && !ev.getPlayer().hasPermissions("terracmd.antihack.bypass"))

	    {

		if (ev.getPlayer().statLifeMax > 400 | ev.getPlayer().statManaMax > 200)

		{

		    NetMessage.SendData(2, ev.getPlayer().whoAmi, -1, "You were kicked for having hacked stats!", 0, 0f, 0f, 0f);

		    Console.WriteLine(ev.getPlayer().name + " was kicked for hacking their health or mana!");

		}

	    }

	}

    }

}