using DiscordBot.Main.GameObjects;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscordBot.Main
{
    static class Constants
    {
        // Stuff
        public static string botToken = "MjQ0NDEwOTY0NjkzMjIxMzc3.Cv9KRg.HltvxZMWG5uHF9p9JTz95jWW_h8";
        public static string gameStatsFile = Path.Combine(Environment.CurrentDirectory, "Stats", "stats.bin");
        public static string rpgStatsFile = Path.Combine(Environment.CurrentDirectory, "Stats", "rpgstats.bin");
        // Bots
        public static ulong BIRIBIRIid = 244410964693221377;
        public static ulong MIKIid = 109379894718234624;
        public static ulong BONFIREid = 183749087038930944;
        public static ulong DANTHEMANid = 189702078958927872;
        public static ulong AUTISTICGIRLid = 150300454708838401;
        public static ulong TATSUMAKIid = 172002275412279296;
        public static ulong LEIABOT = 251143453365239809;

        // Special users
        public static ulong NYAid = 143037788969762816;
        public static ulong WIZZid = 224620110277509120;
        public static ulong TRISTANid = 214708282864959489;
        public static ulong CATEid = 183977132622348288;

        // Games
        // Biribiri interaction
        public static string[] karmaResponse =
        {
            "My love :heart:",              // Nya-only
            "You devil :imp:",              // #-2
            "Meanie :angry:",               // #-1
            "",                             // #0
            "Good boi :smiley:",            // #1
            "Cuty",                         // #2
            "Sweetheart <3"                 // #3
            
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
        // Truth or dare
        public static string[] truth =
        {
            "What is the stupidest thing you have ever done?",
            "What is the most embarrassing moment of your life?",
            "If you could change one thing on your body, what would it be?",
            "What is the silliest thing you have an emotional attachment to?",
            "What are you most afraid of?",
            "What is the scariest dream you have ever had?",
            "Whom here do you hate the most?",
            "What you do when you are alone at home?",
            "What is the most embarrassing thing your parents have caught you doing?",
            "Who is your favourite person and why?",
            "Do you believe in love at first sight?",
            "What is your fetish?",
            "Does size matter?",
            "Who do you like the least in your family?",
            "Who is your celebrity husband or wife?",
            "If you could be any dinosaur, which would it be?",
            "On a scale from 1 to bananya, how do you rate yourself?",
            "What is the most childish thing you still do?",
            "Who here would you most like to make out with?",
            "What is your deepest darkest fear?",
            "Why did you break up with your last boyfriend or girlfriend?"

        };
        public static string[] dare =
        {
            "Send us a hand-pic!",
            "Sing a song in the voice channel!",
            "Ask a staffmember something really personal in a private message!",
            "Turn on your text-to-speech for an hour!",
            "Wear your headphones the wrong way around!",
            "Only talk in song lyrics for 15 minutes!",
            "Ask someone (that is online) to date you in another server!",
            "Send a picture of the muscles you are most proud of!",
            "Only talk in cancer/emoji for the next 15 minutes!",
            "End all your sentences with \"love you babe :heart:\" for the next hour!",
            "Make up a story about the item to your right!",
            "Sing everything you say for the next 10 minutes!",
            "Set the language of your phone to an (to you) unknown language for the next hour!",
            "All your sentences in the coming 30 minutes must contain (some form of) nya (cattalk)!",
            "Pretend that you are an airplane for 2 minutes!",
            "Say the alphabet backwards in a different language!",
            "10 pushups! Go!",
            "Send us a picture of your pajamas!"
        };
    }
}