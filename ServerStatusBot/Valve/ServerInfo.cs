using System.Net;
using System.Net.Sockets;
using ServerStatusBot.Extensions;

namespace ServerStatusBot.Valve;

public class ServerInfo
{
    public string Host { get; }
    public int Port { get; }
    public string Name { get; }
    public string Map { get; }
    public string Game { get; }
    public byte Players { get; }
    public byte MaxPlayers { get; }
    public byte BotPlayers { get; }

    private ServerInfo(string host, int port, string name, string map, string game, byte players, byte maxPlayers, byte botPlayers)
    {
        Host = host;
        Port = port;
        Name = name;
        Map = map;
        Game = game;
        Players = players;
        MaxPlayers = maxPlayers;
        BotPlayers = botPlayers;
    }
    
    private static readonly byte[] ChallengePayload =
        { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };

    private static readonly byte[] ServerInfoPayload = { 0xFF, 0xFF, 0xFF, 0xFF, 0x54, 0x53, 0x6F, 0x75, 0x72, 0x63, 0x65, 0x20, 0x45, 0x6E, 0x67, 0x69, 0x6E, 0x65, 0x20, 0x51, 0x75, 0x65, 0x72, 0x79, 0x00 };
    
    private static byte[] BuildServerInfoPayload(IEnumerable<byte> challenge)
    {
        return ServerInfoPayload.Concat(challenge).ToArray();
    }
    
    public static async Task<ServerInfo?> QueryAsync(string host, int port)
    {
        try
        {
            using var udpClient = new UdpClient(host, port);
            udpClient.Client.ReceiveTimeout = 3000;
            udpClient.Connect(host, port); // connect to server

            // challenge
            await udpClient.SendAsync(ChallengePayload, ChallengePayload.Length); // send challenge
            IPEndPoint? remoteEnd = null; 
            var challengeResponse = udpClient.Receive(ref remoteEnd); // this sync method works with timeout; read challenge result
            var challenge = challengeResponse.Skip(5); // skip challenge code prefix
            var serverInfoPayload = BuildServerInfoPayload(challenge); // create query with the challenge number
            
            // query info
            await udpClient.SendAsync(serverInfoPayload, serverInfoPayload.Length); // send query
            var buffer = udpClient.Receive(ref remoteEnd); // got full info response
            
            // read info
            var readIndex = 0;
            buffer.SkipValues(ref readIndex, 6); // Skip first 6 bytes. prefix, protocol and header
            var name = buffer.ReadString(ref readIndex);
            var map = buffer.ReadString(ref readIndex);
            var game = buffer.ReadString(ref readIndex);
            buffer.SkipString(ref readIndex); // server description
            buffer.SkipValues(ref readIndex, 2); // skip stuff
            var players = buffer.ReadByte(ref readIndex);
            var maxPlayers = buffer.ReadByte(ref readIndex);
            var botPlayers = buffer.ReadByte(ref readIndex);

            return new ServerInfo(host, port, name, map, game, players, maxPlayers, botPlayers);
        }
        catch (Exception) // Could not get server info
        {
            return null;
        }
    }
}