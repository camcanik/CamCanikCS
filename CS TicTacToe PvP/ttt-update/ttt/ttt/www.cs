using System.Collections.Generic;
using System;

public class WWW
{
   public static readonly string CRLF = "\r\n";

   // ---------------------------------------------------------------
   // URL-Encode a string
   // ---------------------------------------------------------------

   public static string Encode(string s)
   {
      // url encode 's'

      string result = "";

      for(int i = 0; i < s.Length; i ++)
      {
         char c = s[i];

         if(c == ' ')
         {
            result += "+";
         }
         else if(char.IsLetter(c) || char.IsDigit(c))
         {
            result += s[i];
         }
         else
         {
            result += "%" + ((int)c).ToString("X2");
         }
      }

      return(result);
   }

   // ---------------------------------------------------------------
   // URL-Decode a string
   // ---------------------------------------------------------------

   public static string Decode(string s)
   {
      string result = "";

      int i = 0;

      while(i < s.Length)
      {
         if(s[i] == '+')
         {
            result += " ";
         }
         else if(s[i] == '%')
         {
            string value = s.Substring(i + 1, 2);

            result += (char)Convert.ToUInt32(value, 16);

            i += 2;
         }
         else
         {
            result += s[i];
         }

         i++;
      }

      return(result);
   }

   // ---------------------------------------------------------------
   // Create a url-encoded resource given a fileName and
   // set of parameters.
   // ---------------------------------------------------------------

   public static string FormatResource(
      string                     fileName,
      Dictionary<string, string> parameters
   )
   {
      string resource = fileName;

      if(!resource.StartsWith("/"))
         resource = "/" + resource;

      if(parameters.Count > 0)
      {
         string suffix = "";

         foreach(var pair in parameters)
         {
            suffix +=
               (suffix != "" ? "&" : "?") +
               Encode(pair.Key) + "=" + Encode(pair.Value);
         }

         resource += suffix;
      }

      return(resource);
   }

   // ---------------------------------------------------------------
   // Given a url-encoded resource, extract the fileName and
   // parameters
   // ---------------------------------------------------------------


   public static bool UnformatResource(
      string                     resource,
      out string                 fileName,
      Dictionary<string, string> parameters,
      out string                 errMsg
   )
   {
      errMsg   = null;
      fileName = resource;

      if(fileName.StartsWith("/"))
         fileName = fileName.Substring(1);;

      int index = fileName.IndexOf('?');

      if(index != -1)
      {
         string queryString = fileName.Substring(index + 1);
         fileName           = fileName.Substring(0, index);

         string[] entries = queryString.Split('&');

         foreach(string entry in entries)
         {
            string[] kv = entry.Split('=');

            if(kv.Length != 2)
            {
               errMsg = "Invalid resource: [" + resource + "]";
               return(false);
            }

            parameters[Decode(kv[0])] = Decode(kv[1]);
         }
      }

      return(true);
   }
}
