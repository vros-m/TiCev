using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TiCev.Server.Business.Services;
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
    public async Task<IActionResult> RegisterUser([FromBody]User user)
    {
        return Ok((await _service.AddAsync(user)).ObjectId);
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
        return Ok(id);
    }

    [HttpDelete("DeletePlaylist/{ownerId}/{id}")]
    public async Task<IActionResult> DeletePlaylist(string ownerId,string id)
    {
        await _service.DeletePlaylist(ownerId, id);
        return Ok();
    }

    [HttpGet("GetPlaylistContent/{ownerId}/{id}")]
    public async Task<IActionResult> GetPlaylistContent(string ownerId,string id)
    {
        return Ok(await _service.GetPlaylistContent(ownerId, id));
    }

    [HttpPost("AddVideoToPlaylist/{ownerId}/{playlistId}/{videoId}")]
    public async Task<IActionResult> AddVideoToPlaylist(string ownerId,string playlistId,string videoId)
    {
        await _service.AddVideoToPlaylist(ownerId, playlistId, videoId);
        return Ok();
    }
    [HttpDelete("RemoveVideoFromPlaylist/{ownerId}/{playlistId}/{videoId}")]
    public async Task<IActionResult> RemoveVideoFromPlaylist(string ownerId,string playlistId,string videoId)
    {
        await _service.RemoveVideoFromPlaylist(ownerId, playlistId, videoId);
        return Ok();
    }
}