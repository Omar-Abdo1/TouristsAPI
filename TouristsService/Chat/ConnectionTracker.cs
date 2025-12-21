using System.Collections.Concurrent;

namespace TouristsAPI.Hubs;

public class ConnectionTracker
{
    private static readonly ConcurrentDictionary<string, List<string>> OnlineUsers = new();

    public Task UserConnected(string userId, string connectionId)
    {
        OnlineUsers.AddOrUpdate(userId, new List<string> { connectionId }, 
            (key, list) =>
        {
          list.Add(connectionId);
          return list;
        });
        return Task.CompletedTask;
    }

    public Task UserDisconnected(string userId, string connectionId)
    {
        if (OnlineUsers.TryGetValue(userId, out var list))
        {
            list.Remove(connectionId);
            if(list.Count==0) OnlineUsers.TryRemove(userId, out _);   
        }
        return Task.CompletedTask;
    }
    public Task<IEnumerable<string>> GetConnections(string userId)
    {
        if (OnlineUsers.TryGetValue(userId, out var list))
            return Task.FromResult((IEnumerable<string>)list);

        return Task.FromResult(Enumerable.Empty<string>());
    }
}