# 🎮 ServerStatusBot 🎮
A powerful game server status bot for your discord server

Make sure to leave a star i will apreciate it a lot ⭐

On the next update i will add the way to know the player names !

# 😱 Features 😱
A unique message in a selected discord channel for displaying the server status

The message will be updated every time specified in the config.json file

It is fast

It isnt shit

# 👤 Usage 👤
1. Download the server status bot folder
2. Go to the server status folder and open the file called config.json and fill all the info. | Leave the port on 0 if you dont want to use a specified port
(you can get a bot token at https://discord.com/developers/applications)
3. Open your cmd and execute: cd folder path. For example cd C:\Users\Felipe\Documents.
4. Execute de command: npm i
5. Execute de command: node . ! Be sure that you have installed node.js on yout machine !
6. Check that all works fine if not re do all the steps if it dont work after that open an issue and i will fix the bug for you

# 🌠 Preview 🌠
![Preview](https://i.imgur.com/sYsF2Z3.png)
![Preview](https://i.imgur.com/xAmbfB9.png)

The servers there are from unturned and there are really good if you want you can join and play with your friends

# 💊 Issues 💊
If the bot has any issue or you want to improve the bot just create a pull request and if all is okey and working it will be acepted

# ✨ Contributing ✨
Fell free to contribute, create pull request and if all is okey and working it will be acepted

# 🔬 Tools 🔬
For this project i used discord.js and official node.js module for interacting with the discord API and gamedig for getting all the game server info

# 🥨 Config 🥨
```
{
    "token": "your_tocken_here",
    "UpdateMessageInterval": 5000,
    "ChannelId": "807320241570447391",
    "GameName": "unturned",
    "ShowPlayers": true,
    "EmbedInfo": {
        "Title": "Our Awesome Servers !",
        "Color": "#008000",
        "Description": "Check out our awesome servers feel free to join in all !\n\n**Players:** <players>/<maxplayers>",
        "ServerField1": "__<servername>__",
        "ServerField2": "**Adress: [<ip>](https://discord.gg/bNxqSWJU3d)  Map: [<map>](https://discord.gg/bNxqSWJU3d)  Players: [<players>/<maxplayers>](https://discord.gg/bNxqSWJU3d)**"
    },
    "Servers": [
        {
            "ServerIp": "64.74.163.82",
            "ServerPort": "28015"
        }
    ]
}
```

[![Github All Releases](https://img.shields.io/github/downloads/01-Feli/ServerStatusBot/total?label=Github%20Downloads)]()
