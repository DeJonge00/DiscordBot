namespace DiscordBot.Main.GameObjects
{
    class GuessingSong
    {
        public string link;
        public string title;
        public string artist;
        public string knownFrom;

        public GuessingSong(string l, string t, string a)
        {
            link = l;
            title = t;
            artist = a;
        }

        public GuessingSong(string l, string t, string a, string kf) : this(l, t, a)
        {
            knownFrom = kf;
        }
    }
}
