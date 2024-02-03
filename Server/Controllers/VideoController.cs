using Microsoft.AspNetCore.Mvc;
using TiCev.Server.Business.Services;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoController(VideoService service) : ControllerBase
{
    private readonly VideoService _service = service;

    [HttpGet("GetVideo/{videoId}")]
    public async Task<ActionResult> GetVideo([FromRoute] string videoId)
    {
        return Ok(await _service.GetByIdAsync(videoId));
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
        var stream = await service.GetVideoAsync(id);
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

}