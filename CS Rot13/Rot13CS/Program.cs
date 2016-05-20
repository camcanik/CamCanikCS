using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rot13CS
{
    class Program
    {
        static Dictionary<char, char> Decoder=new Dictionary<char,char>();

        static void fillDecoder(char[] keys, char[] values)
        {
            for(int i=0;i<keys.Length;i++)
            {
                Decoder.Add(keys[i], values[i]);
            }
        }

        static string Encode(string s)
        {
            

            char[] cMessage = s.ToCharArray();

            for(int i=0;i<cMessage.Length;i++)
            {
                if (cMessage[i] == ' ' || cMessage[i] == ',' || cMessage[i] == '.' || cMessage[i] == '?' || cMessage[i] == '!' || cMessage[i] == '\'')
                    continue;
                cMessage[i] = Decoder[cMessage[i]];
            }

            string message = "";

            foreach(char letter in cMessage)
            {
                message += letter;
            }

            return message;
        }

        static void Main(string[] args)
        {
            char[] keys = new char[] { 'A','B','C','D','E','F','G','H','I','J','K','L','M',
                                       'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                                       'a','b','c','d','e','f','g','h','i','j','k','l','m',
                                       'n','o','p','q','r','s','t','u','v','w','x','y','z'};

            char[] values = new char[] {'N','O','P','Q','R','S','T','U','V','W','X','Y','Z',
                                        'A','B','C','D','E','F','G','H','I','J','K','L','M',
                                        'n','o','p','q','r','s','t','u','v','w','x','y','z',
                                        'a','b','c','d','e','f','g','h','i','j','k','l','m'};

            fillDecoder(keys, values);

            string message = "Its Dangerous to go Alone, Take This!";
            string encodedMessage = Encode(message);

            Console.WriteLine(message+" = "+encodedMessage);

            Console.Read();


        }
    }
}
