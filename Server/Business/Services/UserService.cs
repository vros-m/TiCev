using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using TiCev.Server.Business.Repos;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;
using TiCev.Server.Error;

namespace TiCev.Server.Business.Services;

public class UserService(UserRepo repo,VideoRepo videoRepo):AService<User>(repo)
{
    private PasswordHasher<User> _hasher = new();
    private new readonly UserRepo _repo = repo;
    private readonly VideoRepo _videoRepo = videoRepo;
    public override Task<User> AddAsync(User document)
    {
        document.PasswordHash = _hasher.HashPassword(document, document.PasswordHash);
        return base.AddAsync(document);
    }

    public async Task<SimpleUser?> Login(string username,string password)
    {
        var user = await _repo.GetByUsernameAsync(username);
        return _hasher.VerifyHashedPassword(user!, user!.PasswordHash, password)==PasswordVerificationResult.Failed?null:
        DTOManager.SimplifyUser(user);
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

    public async Task DeletePlaylist(string ownerId,string id)
    {
        await _repo.RemoveFromListAsync(u => u.Id.ToString() == ownerId, u => u.Playlists, p => p.Id == ObjectId.Parse(id));
    }

    public async Task<List<VideoCardView>> GetPlaylistContent(string ownerId,string id)
    {
        var list =await _repo.GetPlaylistContent(ownerId, id);
        return list.Select(DTOManager.FromVideoToCardView).ToList();
    }

    public async Task AddVideoToPlaylist(string ownerId,string playlistId,string videoId)
    {
        var filter = Builders<User>.Filter.And(
        Builders<User>.Filter.Eq("_id", ObjectId.Parse(ownerId)),
        Builders<User>.Filter.Eq("Playlists._id", ObjectId.Parse(playlistId))
            );

        var update = Builders<User>.Update.Push("Playlists.$.VideoIds", videoId);
        await UpdateOneAsync(filter, update);
    }
    public async Task RemoveVideoFromPlaylist(string ownerId,string playlistId,string videoId)
    {
        var filter = Builders<User>.Filter.And(
        Builders<User>.Filter.Eq("_id", ObjectId.Parse(ownerId)),
        Builders<User>.Filter.Eq("Playlists._id", new ObjectId(playlistId))
            );

        var update = Builders<User>.Update.Pull("Playlists.$.VideoIds", videoId);
        await UpdateOneAsync(filter, update);
    }
}