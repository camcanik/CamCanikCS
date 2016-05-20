using System.Windows.Forms;
using System.Drawing;
using System;

// --------------------------------
// Settings
// --------------------------------

public class Settings : Form
{
   private int    mListen;
   private string mHost;
   private int    mPort;
   private int mBoardDimension = 3;

   private TextBox mListenBox;
   private TextBox mHostBox;
   private TextBox mPortBox;
   private TextBox mDimensionsBox;

   public Settings(int listenPort, string destHost, int destPort)
   {
      mListen = listenPort;
      mHost   = destHost;
      mPort   = destPort;

      // Set our size (and don't allow resizing)

      ClientSize = new Size(300, 200);
      FormBorderStyle = FormBorderStyle.FixedDialog;

      const double factorOne   = .1;
      const double factorTwo   = .25;
      const double factorThree = .4;
      const double factorFour  = .55;
      const double factorFive  = .8;

      // Listen Port

      Label listenLabel = new Label();
      listenLabel.Text  = "Listen on port";
      listenLabel.Top   = (int)(factorOne * Height);

      Controls.Add(listenLabel);

      mListenBox      = new TextBox();
      mListenBox.Text = "" + mListen;
      mListenBox.Top  = listenLabel.Top;
      mListenBox.Left = GetWidth() / 2 - mListenBox.Size.Width / 2;

      Controls.Add(mListenBox);

      // DestHost

      Label hostLabel = new Label();
      hostLabel.Text  = "Destination host";
      hostLabel.Top   = (int)(factorTwo * Height);

      Controls.Add(hostLabel);

      mHostBox     = new TextBox();
      mHostBox.Text = mHost;
      mHostBox.Top  = hostLabel.Top;
      mHostBox.Left = GetWidth() / 2 - mHostBox.Size.Width / 2;

      Controls.Add(mHostBox);

      // Dest Port

      Label destLabel = new Label();
      destLabel.Text  = "Destination port";
      destLabel.Top   = (int)(factorThree * Height);

      Controls.Add(destLabel);

      mPortBox      = new TextBox();
      mPortBox.Text  = "" + mPort;
      mPortBox.Top  = destLabel.Top;
      mPortBox.Left = GetWidth() / 2 - mPortBox.Size.Width / 2;

      Controls.Add(mPortBox);

       // Board Dimensions

      Label DimensionsLabel = new Label();
      DimensionsLabel.Text = "Board Dimensions (i.e. 3x3)";
      DimensionsLabel.Top = (int)(factorFour * Height);

      Controls.Add(DimensionsLabel);

      mDimensionsBox = new TextBox();
      mDimensionsBox.Text = "" + mBoardDimension;
      mDimensionsBox.Top = DimensionsLabel.Top;
      mDimensionsBox.Left = GetWidth() / 2 - mDimensionsBox.Size.Width / 2;

      Controls.Add(mDimensionsBox);

       // OK
      Button mOk = new Button();
      mOk.Text   = "Ok";

      mOk.Top  = (int)(factorFive * GetHeight());
      mOk.Left = GetWidth() / 2 - mOk.Size.Width / 2;

      Controls.Add(mOk);

      mOk.Click += new EventHandler(DoOk);

      // Lastly, attach a click handler to both
      // port labels so that clicking either
      // swaps the values

      listenLabel.Click += new EventHandler(delegate { SwapPorts(); });
      destLabel.Click   += new EventHandler(delegate { SwapPorts(); });
   }

   private void SwapPorts()
   {
      string tmp      = mPortBox.Text;
      mPortBox.Text   = mListenBox.Text;
      mListenBox.Text = tmp;
   }

   // ----------------------------------------------
   // Properties for our listen port and our dest
   // host and port
   // ----------------------------------------------

   public int    Listen { get { return(mListen); } }
   public string Host   { get { return(mHost);   } }
   public int    Port   { get { return(mPort);   } }
   public int Dimensions { get { return (mBoardDimension); } }

   // ----------------------------------------------
   // Close on 'Ok' if all fields are valid
   // ----------------------------------------------

   private void DoOk(object sender, EventArgs args)
   {
      // First, verify that our listen port is valid

      int listenPort = 0;

      if(!int.TryParse(mListenBox.Text, out listenPort) || listenPort <= 0)
      {
         MessageBox.Show(
            "Listen port should be an integer > 0 " +
            "(" + mListenBox.Text + ")"
         );

         return;
      }

      string host = "";

      if((host = mHostBox.Text.Trim()) == "")
      {
         MessageBox.Show("No destination host name supplied");
         return;
      }

      int port = 0;

      if(!int.TryParse(mPortBox.Text, out port) || port <= 0)
      {
         MessageBox.Show(
            "Destination port should be an integer > 0 " +
            "(" + mPortBox.Text + ")"
         );

         return;
      }

      int bDim = 2;

      if (!int.TryParse(mDimensionsBox.Text, out bDim) || bDim <= 2)
      {
          MessageBox.Show(
             "Dimensions: uh oh" +
             "(" + mDimensionsBox.Text + ")"
          );

          return;
      }

      mListen = listenPort;
      mHost   = host;
      mPort   = port;
      mBoardDimension = bDim;

      Dispose();
   }

   // -----------------------
   // Prevent 'x' from
   // closing the dialog
   // -----------------------

   protected override void OnFormClosing(FormClosingEventArgs e)
   {
      base.OnFormClosing(e);

      if(e.CloseReason == CloseReason.WindowsShutDown)
         return;

      e.Cancel = true;
   }

   private int GetWidth()
   {
      return(Width - 2 * SystemInformation.FrameBorderSize.Width);
   }

   private int GetHeight()
   {
      return(
         Height -
         (2 * SystemInformation.FrameBorderSize.Height +
         SystemInformation.CaptionHeight)
      );
   }

#if SETTINGS_MAIN

   public static void Main()
   {
      const int    DEFAULT_LISTEN_PORT = 8888;
      const string DEFAULT_DEST_HOST   = "localhost";
      const int    DEFAULT_DEST_PORT   = DEFAULT_LISTEN_PORT;

      Settings theSettings = new Settings(
         DEFAULT_LISTEN_PORT, DEFAULT_DEST_HOST, DEFAULT_DEST_PORT
      );

      Console.WriteLine("Calling ShowDialog()");

      theSettings.ShowDialog();

      Console.WriteLine("Back From Calling ShowDialog()");

      Console.WriteLine(
         "[{0}][{1}][{2}]",
         theSettings.Listen, theSettings.Host, theSettings.Port
      );
   }

#endif
}
