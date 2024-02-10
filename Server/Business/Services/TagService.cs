using TiCev.Server.Business.Repos;
using TiCev.Server.Data.Entities;

namespace TiCev.Server.Business.Services;


public class TagService(TagRepo repo):AService<Tag>(repo)
{
    
}