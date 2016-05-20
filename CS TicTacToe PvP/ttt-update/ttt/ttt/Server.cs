using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Net;
using System.IO;
using System;

public delegate string ScriptHandler(
   string fileName, Dictionary<string, string> parameters
);

public class Server
{
   public static readonly string DEFAULT_PAGE     = "index.html";
   public static readonly string SCRIPT_EXTENSION = "jfd";

   private int                               mPort;
   private Thread                            mThread;
   private TcpListener                       mListener;
   private Dictionary<string, ScriptHandler> mCallbacks;
      
   public Server(int portNumber)
   {
      mCallbacks = new Dictionary<string, ScriptHandler>();

      mPort = portNumber;

      mThread = new Thread(new ThreadStart(Begin));

      mThread.Start();
   }

   public void Begin()
   {
      // Attempt to bind to the specified port

      try
      {
         mListener = new TcpListener(IPAddress.Any, mPort);
         mListener.Start();
      }
      catch(Exception e)
      {
         Console.WriteLine(e.Message);
         return;
      }

      // ---------------------------------------------------------
      // Console.WriteLine("Server listening on port: " + mPort);
      // ---------------------------------------------------------

      bool go = true;

      while(go)
      {
         TcpClient client = null;

         try
         {
            client = mListener.AcceptTcpClient();

            string errMsg;

            if(!ProcessClient(client, out errMsg))
               Console.WriteLine(errMsg);
         }
         catch(Exception e)
         {
            if(mListener == null)
            {
               // If we're here, our 'End' method was
               // called and we need to exit

               go = false;

            }
            else
            {
               Console.WriteLine(e.Message);
            }
         }

         if(client != null)
            client.Close();
      }
   }

   public bool End(out string errMsg)
   {
      errMsg = "";

      try
      {
         if(mListener != null)
            mListener.Stop();

         mListener = null;
      }
      catch(Exception e)
      {
         errMsg = e.Message;
      }

      return(errMsg == "");
   }

   public void RegisterScriptHandler(string extension, ScriptHandler handler)
   {
      // Ensure the extension starts with a dot

      if(!extension.StartsWith("."))
         extension = "." + extension;
 
      mCallbacks[extension] = handler;
   }

   public static string GetContentType(string fileName)
   {
      string result = "text/html";

      string[][] types =
      {
         new string[] { "pdf",  "application/pdf" },
         new string[] { "jpeg", "image/jpeg"      },
         new string[] { "jpg",  "image/jpeg"      },
         new string[] { "png",  "image/png"       },
         new string[] { "csv",  "text/csv"        },
         new string[] { "css",  "text/css"        },
         new string[] { "cs",   "text/plain"      },
         new string[] { "txt",  "text/plain"      },
      };

      foreach(string[] row in types)
      {
         if(fileName.EndsWith("." + row[0]))
         {
            result = row[1];
            break;
         }
      }

      return(result);
   }

   static bool GetBytes(string fileName, out byte[] data)
   {
      data = null;

      try
      {
         data = File.ReadAllBytes(fileName);
      }
      catch(Exception)
      {
         return(false);
      }

      return(true);
   }

   private bool ProcessClient(TcpClient client, out string errMsg)
   {
      errMsg               = null;
      NetworkStream stream = null;

      try
      {
         stream = client.GetStream();

         byte[] requestData = new byte[4096];
         string theRequest  = "";

         for(;;)
         {
            int count = stream.Read(requestData, 0, requestData.Length);

            if(count <= 0)
               break;

            theRequest += Encoding.ASCII.GetString(requestData, 0, count);

            if(theRequest.EndsWith(WWW.CRLF + WWW.CRLF))
               break;
         }


         string[] values = theRequest.Split(
            new string[] { WWW.CRLF },
            StringSplitOptions.None
         );

         string[] tokens = Regex.Split(values[0], "\\s+");

         if(tokens.Length != 3     ||
            tokens[0]     != "GET" ||
            !tokens[2].StartsWith("HTTP/"))
         {
            errMsg = "Invalid request: " + theRequest;
            return(false);
         }

         string resource =
            (tokens[1] == "/" ? DEFAULT_PAGE : tokens[1].Substring(1));

         string fileName   = null;
         var    parameters = new Dictionary<string, string>();

         if(!WWW.UnformatResource(resource, out fileName,
            parameters, out errMsg))
         {
            return(false);
         }

         // See if we have a registered handler for
         // this type of file

         string ext = Path.GetExtension(fileName);

         if(mCallbacks.ContainsKey(ext))
         {
            string response      = mCallbacks[ext](fileName, parameters);
            byte[] responseBytes = Encoding.ASCII.GetBytes(response);

            stream.Write(responseBytes, 0, responseBytes.Length);
         }
         else
         {
            byte[] fileData = null;

            if(GetBytes(fileName, out fileData))
            {
               string headers =
                  "HTTP/1.1 200 OK" + WWW.CRLF + 
                  "Connection: close" + WWW.CRLF +
                  "Content-Type: " + GetContentType(fileName) + WWW.CRLF +
                  "Content-Length: " + fileData.Length + WWW.CRLF + WWW.CRLF;

               byte[] headerBytes = Encoding.ASCII.GetBytes(headers);

               stream.Write(headerBytes, 0, headerBytes.Length);
               stream.Write(fileData, 0, fileData.Length);
            }
            else 
            {
               string response =
                  "HTTP/1.1 404 Not Found" + WWW.CRLF +
                  "Connection: close"      + WWW.CRLF + WWW.CRLF;

               byte[] responseBytes = Encoding.ASCII.GetBytes(response);

               stream.Write(responseBytes, 0, responseBytes.Length);
            }
         }
      }
      catch(Exception e)
      {
         errMsg = e.Message;
      }

      if(stream != null)
         stream.Close();

      return(errMsg == null);
   }

   public static string Handler(
      string fileName,
      Dictionary<string, string> parameters
   )
   {
      // Build our html response page

      string table = "<table border='1'>\n";

      foreach(var pair in parameters)
      {
         table +=
            "<tr><td>" + pair.Key + "</td>" +
            "<td>" + pair.Value + "</td></tr>\n";
      }

      table += "</table>\n";

      string html =
         "<html>\n" +
         " <head><title>" + fileName + "</head></title>\n" +
         " <body>\n" +
         table + 
         " </body>\n" + 
         "</html>\n";

      // Now, create our response (includes the headers)

      string headers =
         "HTTP/1.1 200 OK"                + WWW.CRLF + 
         "Connection: close"              + WWW.CRLF +
         "Content-Type: text/html"        + WWW.CRLF + 
         "Content-Length: " + html.Length + WWW.CRLF + WWW.CRLF;

      return(headers + html);
   }
}
