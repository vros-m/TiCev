namespace TiCev.Server.Data.Entities;

public class Subscription
{
    public string SubscriberId { get; set; } = null!;
    public string ChannelId { get; set; } = null!;
    public string ChannelName { get; set; } = "";
}