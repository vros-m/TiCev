using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TiCev.Server.Business.Services;
using TiCev.Server.Data;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Controllers;

[Route("[controller]")]
[ApiController]
public class UserController(UserService service):ControllerBase
{
    private readonly UserService _service = service;

    [HttpGet("GetUser/{id}/{currentUserId}")]
    public async Task<IActionResult> GetUser(string id,string currentUserId)
    {
        var user = await _service.GetViewByIdAsync(id,currentUserId);
        return Ok(user);
    }

    [HttpPost("RegisterUser")]
    public async Task<IActionResult> RegisterUser([FromBody]UserDTO user)
    {
        return Ok((await _service.AddAsync(DTOManager.FromDTOToUser(user))).ObjectId);
    }

    [HttpGet("Login/{username}/{password}")]
    public async Task<IActionResult> Login(string username,string password)
    {
        return Ok(await _service.Login(username, password));
    }

    [HttpPut("ChangeProfilePicture/{id}")]
    public async Task<IActionResult> ChangeProfilePicture(string id,[FromBody]string newPfp)
    {
        await _service.ChangePfp(id, newPfp);
        return Ok("New profile picture set.");
    }

    [HttpPut("ChangeUsername/{id}/{newUsername}")]
    public async Task<IActionResult> ChangeUsername(string id,string newUsername)
    {
        await _service.ChangeUsername(id,newUsername);
        return Ok("Username set.");
    }

    [HttpGet("GetUserPfp/{id}")]
    public async Task<IActionResult> GetUserPfp(string id)
    {
        var strPfp = await _service.GetFieldAsync(u => u.Id == ObjectId.Parse(id), u => u.ProfilePicture);
        
        var bytes =Convert.FromBase64String(strPfp[(strPfp.IndexOf(',')+1)..]);
        var startIndex = strPfp.IndexOf("image/");
        var endIndex = strPfp.IndexOf(';');
        string type = strPfp.Substring(startIndex,endIndex);
        return File(bytes,type);
    }

    [HttpPost("Subscribe/{direction}")]
    public async Task<IActionResult> Subscribe(Subscription subscription,bool direction)
    {
        if(direction)
        {
            await _service.SubscribeTo(subscription);
        }
        else
        {
            await _service.UnsubscribeFrom(subscription.SubscriberId, subscription.ChannelId);
        }
        return Ok();
    }

    [HttpPost("CreatePlaylist")]
    public async Task<IActionResult> CreatePlaylist(Playlist playlist)
    {
        string id =await _service.CreatePlaylist(playlist);
        return Ok(playlist);
    }

    [HttpGet("GetPlaylists/{id}")]
    public async Task<IActionResult> GetPlaylists(string id)
    {
        var playlists =await _service.GetFieldAsync(u => u.Id == ObjectId.Parse(id), u => u.Playlists);
        return Ok(playlists);
    }

    [HttpDelete("DeletePlaylist/{id}")]
    public async Task<IActionResult> DeletePlaylist(string id)
    {
        await _service.DeletePlaylist( id);
        return Ok();
    }

    [HttpGet("GetPlaylistContent/{id}")]
    public async Task<IActionResult> GetPlaylistContent(string id)
    {
        return Ok(await _service.GetPlaylistContent(id));
    }

    [HttpPost("AddVideoToPlaylist/{playlistId}/{videoId}")]
    public async Task<IActionResult> AddVideoToPlaylist(string playlistId,string videoId)
    {
        await _service.AddVideoToPlaylist( playlistId, videoId);
        return Ok();
    }
    [HttpDelete("RemoveVideoFromPlaylist/{playlistId}/{videoId}")]
    public async Task<IActionResult> RemoveVideoFromPlaylist(string playlistId,string videoId)
    {
        await _service.RemoveVideoFromPlaylist(playlistId, videoId);
        return Ok();
    }

        [HttpGet("GetFeed/{id}")]
    public async Task<IActionResult> GetFeed(string id,int skip=0)
    {
        return Ok(await _service.GetSubscriptionVideos(id, skip));
    }

    [HttpGet("GetNotifications/{id}")]
    public async Task<IActionResult> GetNotifications(string id)
    {
        return Ok((await _service.GetFieldAsync(u => u.Id == ObjectId.Parse(id),
        u => u.Notifications)).OrderByDescending(n=>n.Timestamp));
    }

    [HttpDelete("DeleteNotification/{id}")]
    public async Task<IActionResult> DeleteNotification(string id)
    {
        await _service.RemoveNotification(id);
        return Ok();
    }
}