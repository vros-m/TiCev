namespace TiCev.Server.Settings;
public class MongoDBSettings : IMongoDBSettings
{
    public string ConnectionString { get; set; } = String.Empty;
    public string DatabaseName { get; set; } = String.Empty;
    public string VideoCollectionName { get; set; } = String.Empty;
    public string UserCollectionName { get; set; } = String.Empty;
    public string TagCollectionName { get; set; } = String.Empty;
    public string PlaylistCollectionName { get; set; } = String.Empty;
    public string CommentCollectionName { get; set; } = String.Empty;
    //public string CollectionName { get; set; }
}