## TerraCmd - Fun & Admin Commands for tMod.

##### [Permissions](http://github.com/PwnCraft/Permissions) is required. TerraCmd will not function properly without it.

### Features

- Anti-hack: User is kicked on join if their health or mana is over their legitimate limits.
- Currently Online userlist.
- MOTD system. Editable in terracmd.txt (Settings file). Two variables at the moment. [player] = player name, [server] = server IP. (Don't think hostnames work)
- MOTD can be colored to your liking. Change the motdR, motdG, motdB values to your liking as if it were a RGB scale.
	- A RGB color chart can be found [here.](http://www.web-source.net/216_color_chart.htm)
- OP tag and coloring system. Same as the MOTD color system. Edit opR, opG, opB values as if it were a RGB scale.
- Automatic map saving. Saves every 10 minutes.
- Settings reload command.
- Change server password in-game. (Doesn't persist over restarts, you must set it manually to persist.)
- Our own [Permissions](http://github.com/PwnCraft/Permissions) support.

### Permissions nodes

- Antihack:
	- terracmd.antihack.bypass
		- Unaffected by antihack. Can enter the server with hacked stats.
- Userlist:
	- terracmd.playerlist
		- Can use /list, /playerlist, /online, or /players to access the playerlist.
- TerraCmd Settings reload:
	- terracmd.reload
		- Can use /terracmd in-game to reload plugin settings. (May be buggy)
		- User can also just be OP.
- In-game password changing:
	- terracmd.password
		- Can use /password [newpassword] to temporarily change the server password.
		- Minimum of 3 characters, 0 characters will remove the password.
		- User can also just be OP.

### To do

- Make the save interval editable.
- Add a settings value for antihack=OP and/or everyone.

### Changelog

#### v1.2.1

- Compatible with TerraChat and General Commands by adding setting to switch off OP tagging/coloring.
	- You must add "opSetting=false" to your terracmd.txt config file for it to be switched off.

#### v1.2

- Fixed chat messages showing before being logged in with Permissions.
- Misc. other things to improve Permissions binding.

#### v1.1.1

- Fixed the plugin reloading whenever someone did /login with Permissions and on /oplogin.
- Fixed /terracmd command.

#### v1.1

- Added [Permissions](http://github.com/PwnCraft/Permissions) support.
- Added in-game password changing & settings reload commands.
- Fixed up the OP tagging and coloring.
- Updated for tMod 009.

#### v1.0

- Initial release.