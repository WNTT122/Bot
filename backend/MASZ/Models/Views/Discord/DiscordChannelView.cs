using Discord;

namespace MASZ.Models.Views
{
    public class DiscordChannelView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
        public int Type { get; set; }

        public DiscordChannelView(IGuildChannel channel)
        {
            Id = channel.Id.ToString();
            Name = channel.Name;
            Position = channel.Position;
            // forum channels currently throw an error, will be fixed in new d.net version that is not yet released
            // https://github.com/discord-net/Discord.Net/pull/2469
            try {
                Type = (int)channel.GetChannelType();
            }
            catch(Exception) {
                Type = -1;
            }
        }

        public static DiscordChannelView CreateOrDefault(IGuildChannel channel)
        {
            if (channel == null) return null;
            if (channel.Id == 0) return null;
            return new DiscordChannelView(channel);
        }
    }
}
