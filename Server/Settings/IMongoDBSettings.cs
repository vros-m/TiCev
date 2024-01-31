namespace TiCev.Server.Settings;
public interface IMongoDBSettings
{
    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
    string VideoCollectionName { get; set; }
    string UserCollectionName { get; set; }
    string TagCollectionName { get; set; }
    string PlaylistCollectionName { get; set; }
    string CommentCollectionName { get; set; }
}

