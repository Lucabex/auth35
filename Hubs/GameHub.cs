using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;

namespace auth35.Hubs;

public class GameHub : Hub
{
    private static readonly ConcurrentDictionary<string,string> _awaitingPlayer = new ConcurrentDictionary<string, string>();
    private static readonly ConcurrentDictionary<string,string> _connectionToRoom = new ConcurrentDictionary<string, string>();

    public async Task FindNewMatch(string userName)
    {
        var opponent = _awaitingPlayer.FirstOrDefault();
        if (opponent.Key != null)
        {
            var roomId = Guid.NewGuid().ToString();
            _connectionToRoom[Context.ConnectionId]=roomId;
            _connectionToRoom[opponent.Key] = roomId;

            await Groups.AddToGroupAsync(Context.ConnectionId,roomId);
            await Groups.AddToGroupAsync(opponent.Key,roomId);

            await Clients.Client(opponent.Key).SendAsync("MatchFound",roomId,userName);
            await Clients.Caller.SendAsync("MatchFound",roomId,opponent.Value);
        }
        else
        {
            _awaitingPlayer.TryAdd(Context.ConnectionId,userName);
            await Clients.Caller.SendAsync("WaitingForMatch");
        }
    }
    public async Task MakeOnlinemove(string roomId,object moveData)
    {
        await Clients.OthersInGroup(roomId).SendAsync("OpponentMove",moveData);
    }
    public async Task CancelSearch()
    {
        _awaitingPlayer.TryRemove(Context.ConnectionId,out _);
        await Clients.Caller.SendAsync("CancelSearch");
    }
}