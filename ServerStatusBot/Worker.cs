using DSharpPlus;
using System.Net;
using System.Reflection;
using DSharpPlus.Entities;
using ServerStatusBot.Valve;
using SmartFormat;

namespace ServerStatusBot;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DiscordRestClient _restClient;
    private readonly IConfiguration _configuration;
    private DiscordUser? _botUser;
    
    public Worker(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<Worker>();
        _restClient = new DiscordRestClient(new DiscordConfiguration
        {
            TokenType = TokenType.Bot,
            Token = configuration["Bot:Token"],
            LoggerFactory = loggerFactory
        });
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botUser = await _restClient.GetCurrentUserAsync();
        
        _logger.LogInformation("Executing worker with version {version}", Assembly.GetExecutingAssembly().GetName().Version);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var servers = _configuration.GetSection("Servers").Get<List<string>>();

            if (servers == null)
                return;
            
            _logger.LogDebug("Getting information of {Count} servers", servers.Count);

            var infos = await Task.WhenAll(servers.Select(server =>
            {
                var sections = server.Split(":");
                var ip = sections[0];

                if (!IPAddress.TryParse(ip, out _))
                {
                    _logger.LogWarning("Server: {server} does not have a valid ip or port separated by ':'", server);
                    return Task.FromResult(default(ServerInfo));                
                }

                if (!int.TryParse(sections[1], out var port))
                {                   
                    _logger.LogWarning("Server: {server} does not have a valid ip or port separated by ':'", server);
                    return Task.FromResult(default(ServerInfo));
                }
                _logger.LogDebug("Getting status of server {ip}:{port}", ip, port);
                return ServerInfo.QueryAsync(ip, port);   
            }));
            
            infos = infos.Where(info => info is not null).ToArray();
            
            _logger.LogDebug("Successfully gotten information of {lenght} servers", infos.Length);
            
            var channelId = _configuration.GetSection("Bot:StatusChannelId").Get<ulong>();

            var oldStatusMessage = await GetStatusMessageAsync(channelId);
            
            var statusMessage = await BuildStatusMessage(infos!);
            
            if (oldStatusMessage is not null)
            {
                _logger.LogDebug("Editing message with id {Id} with new content", oldStatusMessage.Id);
                await _restClient.EditMessageAsync(channelId, oldStatusMessage.Id, statusMessage);
            }
            else
            {
                _logger.LogDebug("Creating message with status and pinned it");
                var createdMessage = await _restClient.CreateMessageAsync(channelId, statusMessage);
                await _restClient.PinMessageAsync(channelId, createdMessage.Id);
            }
            
            await Task.Delay(_configuration.GetSection("Bot:UpdateInterval").Get<int>(), stoppingToken);
        }
    }

    private async Task<DiscordMessage?> GetStatusMessageAsync(ulong channelId)
    {
        var pinnedMessages = await _restClient.GetPinnedMessagesAsync(channelId);

        return pinnedMessages?.FirstOrDefault(m => m.Author.Id == _botUser?.Id);
    }

    private Task<DiscordEmbed> BuildStatusMessage(ServerInfo[] infos)
    {
        var players = infos.Sum(s => s.Players);
        var maxPlayers = infos.Sum(s => s.MaxPlayers);
        
        var statusMessageBuilder = new DiscordEmbedBuilder()
            .WithTitle(_configuration["StatusMessage:Title"])
            .WithColor(new DiscordColor(_configuration["StatusMessage:Color"]))
            .WithDescription(Smart.Format(_configuration["StatusMessage:Description"] ?? string.Empty, new { players, maxPlayers }))
            .WithTimestamp(DateTimeOffset.Now);

        foreach (var info in infos)
        {
            var title = _configuration["StatusMessage:ServerDisplay:Title"]?? string.Empty;
            var content = _configuration["StatusMessage:ServerDisplay:Content"]?? string.Empty;

            statusMessageBuilder.AddField(Smart.Format(title, info), Smart.Format(content, info));
        }

        return Task.FromResult(statusMessageBuilder.Build());
    }
}