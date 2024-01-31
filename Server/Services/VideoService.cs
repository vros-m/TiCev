using TiCev.Server.Repositories;
namespace TiCev.Server.Services;
public class VideoService(VideoRepo repo)
{
    private readonly VideoRepo _repo = repo;
}