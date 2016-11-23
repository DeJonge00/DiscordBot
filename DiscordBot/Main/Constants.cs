using DiscordBot.Main.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBot.Main
{
    static class Constants
    {
        // Stuff
        public static string SOUNDCLOUDid = "rick-de-jonge-780921910";
        public static string GOOGLEAPIkey = "AIzaSyC6Yy9fhFLYXID-5JsomZVN2dd456D09Gk";
        public static List<ulong> ServerBlacklist = new List<ulong>();
        public static List<ulong> ChannelBlacklist = new List<ulong>();
        public static List<ulong> UserBlacklist = new List<ulong>();
        // Bots
        public static ulong BIRIBIRIid = 244410964693221377;
        public static ulong MIKIid = 109379894718234624;
        public static ulong BONFIREid = 183749087038930944;
        public static ulong DANTHEMANid = 189702078958927872;
        public static ulong AUTISTICGIRLid = 150300454708838401;
        public static ulong TATSUMAKIid = 172002275412279296;
        public static ulong[] BOTids = { BIRIBIRIid, MIKIid, BONFIREid, DANTHEMANid, AUTISTICGIRLid, TATSUMAKIid };
        public static List<char> BOTprefix = new List<char> { '>', '!', '`', '-', '.' };

        // Special users
        public static ulong NYAid = 143037788969762816;
        public static ulong WIZZid = 224620110277509120;
        public static ulong TRISTANid = 214708282864959489;

        // Games
        // Biribiri interaction
        public static string[] karmaResponse =
        {
            "My love :heart:",              // Nya-only
            "You devil :imp:",              // #-2
            "Meanie :angry:",               // #-1
            "",                             // #0
            "Cuty",                         // #1
            "Sweetheart <3"                 // #2
            
        };

        // Music guessing
        private static GuessingSong mob99 = new GuessingSong("https://www.youtube.com/watch?v=M4uCRncR3_w", "99", "Mob Choir", "Mob Psycho 100");
        private static GuessingSong coolest = new GuessingSong("https://www.youtube.com/watch?v=f8d3bW42aFY", "Coolest", "Customi Z", "Sakamoto");
        private static GuessingSong judgelight = new GuessingSong("https://www.youtube.com/watch?v=o2w3D3u1LBI", "Judgelight", "Fripside", "Railgun");
        private static GuessingSong onlyMyRaingun = new GuessingSong("https://www.youtube.com/watch?v=FeOo3qT3isU", "Only My Railgun", "Fripside", "Railgun");
        private static GuessingSong sistersNoise = new GuessingSong("https://www.youtube.com/watch?v=o2w3D3u1LBI", "Sisters Noise", "Fripside", "Railgun");
        public static List<GuessingSong> songs = new List<GuessingSong>
        {
            mob99, coolest, judgelight, onlyMyRaingun, sistersNoise
        };
        // Quiz
        private static QuizQuestion bestgirl = new QuizQuestion("Who is best animu girl?", new List<string> { "Biribiri", "Misaka", "Mikoto" });
        private static QuizQuestion bestboy = new QuizQuestion("Who is best animu boy?", new List<string> { "Viktor", "Nikiforov" });
        private static QuizQuestion spacecowboy = new QuizQuestion("Name the anime about a space cowboy", new List<string> { "Cowboy", "Bebop" });
        private static QuizQuestion railgun = new QuizQuestion("Name the anime the level 5 electromaster Misaka Mikoto is in", new List<string> { "Railgun", "Toaru", "Index" });
        private static QuizQuestion fmab = new QuizQuestion("Name the anime about two brothers who tried to bring back their mother and failed", new List<string> { "FMA", "FMAB", "FMA:B", "Brotherhood", "Full", "Metal", "Alchemist" });
        private static QuizQuestion op99 = new QuizQuestion("Give me the opening theme of Mob Psycho 100", new List<string> { "99", "Choir" });
        private static QuizQuestion countryroads = new QuizQuestion("Name the song in the movie: Whispers of the Heart", new List<string> { "Country", "Roads"});
        public static List<QuizQuestion> animeQuestions = new List<QuizQuestion>
        {
            bestgirl, bestboy, spacecowboy, railgun, fmab, op99, countryroads
        };
        private static QuizQuestion song1 = new QuizQuestion("Name the song: *Hello darkness my old friend*", new List<string> { "Sound", "Silence"});
        private static QuizQuestion song2 = new QuizQuestion("Name the song: *But in the end, it doesn't even matter*", new List<string> { "Linkin", "Park", "end" });
        private static QuizQuestion song3 = new QuizQuestion("Name the song: *I hear jeruzalem bells aringing, roman cavalry choirs are singing*", new List<string> { "Viva", "vida", "Coldplay" });
        public static List<QuizQuestion> musicQuestions = new List<QuizQuestion>
        {
            song1, song2, song3
        };

        // Music
        public static string musicPath = @"C:\Users\dejon\Music\DiscordBot";

        public static int KiB(this int value) => value * 1024;
        public static int KB(this int value) => value * 1000;

        public static int MiB(this int value) => value.KiB() * 1024;
        public static int MB(this int value) => value.KB() * 1000;

        public static int GiB(this int value) => value.MiB() * 1024;
        public static int GB(this int value) => value.MB() * 1000;

        public static ulong KiB(this ulong value) => value * 1024;
        public static ulong KB(this ulong value) => value * 1000;

        public static ulong MiB(this ulong value) => value.KiB() * 1024;
        public static ulong MB(this ulong value) => value.KB() * 1000;

        public static ulong GiB(this ulong value) => value.MiB() * 1024;
        public static ulong GB(this ulong value) => value.MB() * 1000;

        public static string TrimTo(string str, int num, bool hideDots = false)
        {
            if (num < 0)
                throw new ArgumentOutOfRangeException(nameof(num), "TrimTo argument cannot be less than 0");
            if (num == 0)
                return string.Empty;
            if (num <= 3)
                return string.Concat(str.Select(c => '.'));
            if (str.Length < num)
                return str;
            return string.Concat(str.Take(num - 3)) + (hideDots ? "" : "...");
        }
    }
}