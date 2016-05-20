using System;
using System.Collections.Generic;

namespace ReversedLettersCS
{
    class Program
    {
        static List<string> words = new List<string>();

        static void seperateWords(string s)
        {
            string result=s;
            int pos=result.IndexOf(" ");

            if (pos == -1)
            {
                words.Add(result);
                return;
            }
            string theWord;
            while(pos!=-1)
            {
                theWord = result.Substring(0, pos);
                result = result.Substring(pos+1);
                pos = result.IndexOf(" ");
                words.Add(theWord);
            }
            words.Add(result);
        }

        static void reverse()
        {
            for(int x=0;x<words.Count;x++)
            {
                char[] reversal=words[x].ToCharArray();
                char[] temp =new char[reversal.Length];
                reversal.CopyTo(temp,0);

                int j = reversal.Length - 1;
                for(int i=0;i<reversal.Length;i++)
                {  
                        temp[i] = reversal[j];
                        j--;
                }
                string newWord="";
                foreach(char letter in temp)
                {
                    newWord += letter;
                }
                words[x] = newWord;
            }
        }

        static void Main(string[] args)
        {
            string s = "The Cake is a lie";

            seperateWords(s);

            reverse();

            foreach(string word in words)
            {
                Console.Write(word + " ");
            }
            Console.Read();
        }
    }
}
