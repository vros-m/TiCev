using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using TiCev.Server.Business.Repos;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;
using TiCev.Server.Error;

namespace TiCev.Server.Business.Services;

public class UserService(UserRepo repo,VideoRepo videoRepo,SocketService socketService):AService<User>(repo)
{
    private PasswordHasher<User> _hasher = new();
    private new readonly UserRepo _repo = repo;
    private readonly VideoRepo _videoRepo = videoRepo;
    private readonly SocketService _socketService = socketService;
    public override Task<User> AddAsync(User document)
    {
        document.PasswordHash = _hasher.HashPassword(document, document.PasswordHash);
        return base.AddAsync(document);
    }

    public async Task<SimpleUser?> Login(string username,string password)
    {
        var user = await _repo.GetByUsernameAsync(username);
        return _hasher.VerifyHashedPassword(user!, user!.PasswordHash, password)==PasswordVerificationResult.Failed?null:
        DTOManager.SimplifyUser(user!);
    }

    public async Task ChangePfp(string id,string newPfp)
    {
        await _repo.UpdateOneAsync(u => u.Id == ObjectId.Parse(id), Builders<User>.Update.Set(u => u.ProfilePicture, newPfp));
    }

    public async Task ChangeUsername(string id,string newUsername)
    {
        await _repo.UpdateOneAsync(u => u.Id == ObjectId.Parse(id), Builders<User>.Update.Set(u => u.Username, newUsername));
        await _videoRepo.UpdateManyAsync(v => v.ChannelId == id, Builders<Video>.Update.Set(v => v.ChannelName, newUsername));
        await _repo.UpdateManyAsync(Builders<User>.Filter.ElemMatch(u=>u.Subscriptions, s=>s.ChannelId==id),
        Builders<User>.Update.Set("Subscriptions.$.ChannelName", newUsername));
    }

    public async Task<UserView> GetViewByIdAsync(string id,string currentUserId)
    {
        return (await _repo.GetViewByIdAsync(id,currentUserId))!;
    }

    public async Task SubscribeTo(Subscription subscription)
    {
        await _repo.InsertIntoListAsync<Subscription>(u => u.Id == ObjectId.Parse(subscription.SubscriberId), u => u.Subscriptions,
        subscription);
        await _repo.InsertIntoListAsync(u => u.Id.ToString() == subscription.ChannelId, u => u.Subscribers,
        subscription.SubscriberId);
    }

    public async Task UnsubscribeFrom(string id,string fromId)
    {
        await _repo.RemoveFromListAsync(u => u.Id == ObjectId.Parse(id), u => u.Subscriptions,
        s=>s.ChannelId==fromId);
        await _repo.RemoveFromListAsync(u => u.Id == ObjectId.Parse(fromId), u => u.Subscribers,
        id);
    }
    public async Task<string> CreatePlaylist(Playlist playlist)
    {
        await _repo.InsertIntoListAsync(u => u.Id.ToString() == playlist.ChannelId, u => u.Playlists, playlist);
        return playlist.ObjectId;
    }

    public async Task DeletePlaylist(string id)
    {
        await _repo.RemoveFromListAsync(Builders<User>.Filter.ElemMatch(u=>
        u.Playlists,p=>p.Id==ObjectId.Parse(id)), u => u.Playlists, p => p.Id == ObjectId.Parse(id));
    }

    public async Task<PlaylistView> GetPlaylistContent(string id)
    {
        var playlist =await _repo.GetPlaylistContent(id);
        return DTOManager.FromDTOToPlaylistView(playlist);
    }

    public async Task AddVideoToPlaylist(string playlistId,string videoId)
    {
        var filter = Builders<User>.Filter.Eq("Playlists._id", ObjectId.Parse(playlistId));

        var update = Builders<User>.Update.Push("Playlists.$.VideoIds", videoId);
        await UpdateOneAsync(filter, update);
    }
    public async Task RemoveVideoFromPlaylist(string playlistId,string videoId)
    {
        var filter = Builders<User>.Filter.Eq("Playlists._id", new ObjectId(playlistId));

        var update = Builders<User>.Update.Pull("Playlists.$.VideoIds", videoId);
        await UpdateOneAsync(filter, update);
    }

    public async Task<List<VideoCardView>> GetSubscriptionVideos(string id,int skip)
    {
        return (await _repo.GetSubscriptionVideos(id, skip)).Select(DTOManager.FromVideoToCardView).ToList();
    }

    public async Task AddNotification(Notification notification)
    {
        await _repo.InsertIntoListAsync(u => u.Id == ObjectId.Parse(notification.RecipientId),
        u => u.Notifications, notification);

        await _socketService.NotifyUser(notification);
    }
    public async Task RemoveNotification(string id)
    {
        await _repo.RemoveFromListAsync(Builders<User>.Filter.ElemMatch(
            u => u.Notifications, n=>n.Id==ObjectId.Parse(id)), u => u.Notifications, n => n.Id == ObjectId.Parse(id));
    }
}