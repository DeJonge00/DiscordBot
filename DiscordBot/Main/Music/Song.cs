namespace DiscordBot.Main.Music
{
    class Song
    {
        public string path;
        public string title;
        public string user;

        public Song(string path, string name, string username)
        {
            this.path = path;
            title = name;
            user = username;
        }
    }
}
