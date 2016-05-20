using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System;

public class Client
{
   private static TcpClient GetClient(
      string host, int port, int timeoutSeconds
   )
   {
      TcpClient client     = null;
      bool      setTimeout = true;

      if(setTimeout)
      {
         client = new TcpClient();

         var result = client.BeginConnect(host, port, null, null);

         var success = result.AsyncWaitHandle.WaitOne(
            TimeSpan.FromSeconds(timeoutSeconds)
         );

         if(!success)
         {
            throw new Exception(
               "Connect to: " + host + ":" + port + " timed out"
            );
         }

         client.EndConnect(result);
      }
      else
      {
         client = new TcpClient(host, port);
      }

      return(client);
   }

   // ---------------------------------------------------------------
   // If the request is successful, 'contents' will contain
   // the returned page and true will be returned.
   //
   // If the request fails, 'errMsg' will contain the details
   // as to why and false will be returned.
   // ---------------------------------------------------------------

   public static bool DoGetRequest(
      string     theHost,                    // the host to contact
      int        thePort,                    // communicate on this port
      string     fileName,                   // GET this file
      Dictionary<string, string> parameters, // GET parameters
      out string contents,                   // store returned page here
      out string errMsg                      // populated on error
   )
   {
      contents = "";
      errMsg   = "";

      TcpClient     tc = null;
      NetworkStream ns = null;

      try
      {
         string resource = WWW.FormatResource(fileName, parameters);

         string theRequest =
            "GET " + resource + " HTTP/1.1" + WWW.CRLF + 
            "Host: " + theHost              + WWW.CRLF +
            "Connection: close"             + WWW.CRLF +
                                              WWW.CRLF;

         const int CONNECT_TIMEOUT_SECONDS = 3;

         tc = GetClient(theHost, thePort, CONNECT_TIMEOUT_SECONDS);
         ns = tc.GetStream();

         byte[] data = Encoding.ASCII.GetBytes(theRequest);

         ns.Write(data, 0, data.Length);

         data               = new byte[4096];
         string theResponse = "";

         for(;;)
         {
            int count = ns.Read(data, 0, data.Length);

            if(count <= 0)
               break;

            theResponse += Encoding.ASCII.GetString(data, 0, count);
         }

         contents = theResponse;
      }
      catch(Exception e)
      {
         errMsg = e.Message;
      }

      if(ns != null)
         ns.Close();

      if(tc != null)
         tc.Close();

      return(errMsg == "");
   }

   // ---------------------------------------------------------------
   // If the request is successful, 'contents' will contain
   // the returned page and true will be returned.
   //
   // If the request fails, 'errMsg' will contain the details
   // as to why and false will be returned.
   // ---------------------------------------------------------------

   public static bool DoGetRequest(
      string     theHost,  // the host to contact
      int        thePort,  // communicate on this port
      string     fileName, // GET this file
      out string contents, // store returned page here
      out string errMsg    // populated on error
   )
   {
      contents = errMsg = null;

      return(
         DoGetRequest(
            theHost,
            thePort,
            fileName,
            new Dictionary<string, string>(),
            out contents,
            out errMsg
         )
      );
   }
}
