using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Main.GameObjects
{
    class QuizQuestion
    {
        public string question;
        public List<string> answer;

        public QuizQuestion(string q, List<string> a)
        {
            question = q;
            answer = a;
        } 

        public bool IsAnswer(string guess)
        {
            foreach(string s in answer)
            {
                if (guess.Split(' ').Contains(s.ToLower()))
                    return true;
            }
            return false;
        }
    }
}
