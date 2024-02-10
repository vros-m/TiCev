using Microsoft.AspNetCore.Mvc;
using TiCev.Server.Business.Services;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoController(VideoService service) : ControllerBase
{
    private readonly VideoService _service = service;

    [HttpGet("GetVideo/{videoId}/{userId}")]
    public async Task<ActionResult> GetVideo([FromRoute] string videoId,[FromRoute]string userId)
    {
        return Ok(await _service.GetVideoViewAsync(videoId,userId));
    }

    [HttpPost("PostVideo")]
    public async Task<ActionResult> PostVideo([FromForm] VideoDTO dto)
    {
        var vid =await _service.UploadVideoAsync(dto);
        return Ok(vid!);
    }

    [HttpGet("GetVideoContent/{id}")]
    [Produces("video/mp4")]
    public async Task<IActionResult> GetVideoContent([FromRoute] string id)
    {
        var stream = await _service.GetVideoAsync(id);
        return File(stream,"video/mp4");
    }

    [HttpPut("EditVideoDescription/{id}")]
    public async Task<IActionResult> EditVideoDescription([FromRoute]string id,[FromBody]string newDesc)
    {
        await _service.UpdateDescription(id, newDesc);
        return Ok("Description changed.");
    }

    [HttpDelete("DeleteVideo/{id}")]
    public async Task<IActionResult> DeleteVideo(string id)
    {
        await _service.DeleteVideoAsync(id);
        return Ok("Video deleted.");
    }

    [HttpGet("GetThumbnail/{id}")]
    public async Task<IActionResult> GetThumbnail([FromRoute] string id)
    {
        var file = await _service.GetThumbnailAsync(id);
        return File(file.Item1,file.Item2);
    }

    [HttpGet("GetThumbnailByVideoId/{id}")]
    public async Task<IActionResult> GetThumbnailByVideoId([FromRoute] string id)
    {
        var vid = await _service.GetByIdAsync(id);
        var file = await _service.GetThumbnailAsync(vid.ThumbnailId);
        return File(file.Item1, file.Item2);
    }

    [HttpPut("IncrementViews")]
    public async Task<IActionResult> IncrementViews(string id)
    {
        await _service.IncrementViews(id);
        return Ok();
    }

    [HttpPut("RateVideo/{videoId}/{userId}/{rating}")]
    public async Task<IActionResult> RateVideo(string videoId,string userId,double rating)
    {
       await _service.RateVideo(videoId, userId, rating);
        return Ok();
    }

    [HttpGet("SearchForVideo/{query}")]
    public async Task<IActionResult> SearchForVideo(string query)
    {
        return Ok(await _service.SearchForVideosAsync(query));
    }

    [HttpGet("GetVideos")]
    public async Task<IActionResult> GetVideos(int skip=0,int limit=20)
    {
        return Ok(await _service.GetAllAsync(skip,limit));
    }

    [HttpGet("GetRecommendedVideos/{id}")]
    public async Task<IActionResult> GetRecommendedVideos(string id)
    {
        return Ok(await _service.GetRecommendedVideosAsync(id));
    }

    //add comments and notifications

}