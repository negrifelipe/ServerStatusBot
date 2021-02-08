const Discord = require("discord.js");
const client = new Discord.Client;

const gamedig = require("gamedig");

const config = require("./config.json");

client.on("ready", () => {
    console.log("The bot is ready");

    setInterval(UpdateInfo, config.UpdateMessageInterval);
});

client.login(config.token);

function UpdateInfo() {
    let channel = client.channels.cache.get(config.ChannelId);
    channel.messages.fetch({ limit: 1 }).then(messages => {
        let lastMessage = messages.first();
        if(lastMessage == null || lastMessage.author.id != client.user.id){
            SendMessage(channel);
        }
        else{
            UpdateEmbed(lastMessage);
        }
    });
}

async function SendMessage(channel){
    let players = 0;
    let maxplayers = 0;
    let embed = new Discord.MessageEmbed()
    var server;
    try{
        for(var i = 0; i < config.Servers.length; i++){
            var x = config.Servers[i];
            server = await gamedig.query({
                type: config.GameName,
                host: x.ServerIp,
                port: x.ServerPort
            }).catch((error) => {
                console.log("Timeout or error on " + x.ServerIp + ":" + x.ServerPort)
            })
            if(server != null){
                players += server.players.length;
                maxplayers += server.maxplayers;
                embed.addFields(
                    {
                        name: config.EmbedInfo.ServerField1.replace("<servername>", server.name), value: config.EmbedInfo.ServerField2.replace("<ip>", server.connect).replace("<map>", server.map).replace("<players>", server.players.length).replace("<maxplayers>", server.maxplayers)
                    }
                )
                if(config.ShowPlayers){
                    for(var o = 0; o < server.players.length; o++){
                        var player = server.players[o];
                        embed.addFields({
                            name: ":bust_in_silhouette: " + player.name, value: ":hourglass_flowing_sand: " + GetTime(player.time)
                        })
                    }
                }
            }
        }
    }
    finally{
        embed.setColor(config.EmbedInfo.Color);
        embed.setTitle(config.EmbedInfo.Title)
        embed.setDescription(config.EmbedInfo.Description.replace("<players>", players).replace("<maxplayers>", maxplayers))
        channel.send(embed);
    }
}

async function UpdateEmbed (message){
    let players = 0;
    let maxplayers = 0;
    let embed = new Discord.MessageEmbed()
    var server;
    try{
        for(var i = 0; i < config.Servers.length; i++){
            var x = config.Servers[i];
            server = await gamedig.query({
                type: config.GameName,
                host: x.ServerIp,
                port: x.ServerPort
            }).catch((error) => {
                console.log("Timeout or error on " + x.ServerIp + ":" + x.ServerPort)
            })
            if(server != null){
                players += server.players.length;
                maxplayers += server.maxplayers;
                embed.addFields(
                    {
                        name: config.EmbedInfo.ServerField1.replace("<servername>", server.name), value: config.EmbedInfo.ServerField2.replace("<ip>", server.connect).replace("<map>", server.map).replace("<players>", server.players.length).replace("<maxplayers>", server.maxplayers)
                    }
                )
                if(config.ShowPlayers){
                    for(var o = 0; o < server.players.length; o++){
                        var player = server.players[o];
                        embed.addFields({
                            name: ":bust_in_silhouette: " + player.name, value: ":hourglass_flowing_sand: " + GetTime(player.time), inline: true
                        })
                    }
                }
            }
        }
    }
    finally{
        embed.setColor(config.EmbedInfo.Color);
        embed.setTitle(config.EmbedInfo.Title);
        embed.setDescription(config.EmbedInfo.Description.replace("<players>", players).replace("<maxplayers>", maxplayers));
        message.edit(embed);
    }
}

function GetTime(ms) {
    ms *= 1000;
    const sec = Math.floor((ms / 1000) % 60).toString()
    const min = Math.floor((ms / (1000 * 60)) % 60).toString()
    const hrs = Math.floor((ms / (1000 * 60 * 60)) % 60).toString()
    const days = Math.floor((ms / (1000 * 60 * 60 * 24)) % 60).toString()
    return `${days.padStart(1, `0`)}d ${hrs.padStart(2, `0`)}h ${min.padStart(2, `0`)}m ${sec.padStart(3, `0`)}s` 
}
