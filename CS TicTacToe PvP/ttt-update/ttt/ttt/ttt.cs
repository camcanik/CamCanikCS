using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System;

// ---------------
// TTT
// ---------------

public class TTT : Form
{
    // ------------
    // Constants
    // ------------

    enum State { NO_ACTIVE_GAME, MY_TURN, OTHER_TURN };

    private static readonly int MAX_START_SECONDS = 30;

    private static readonly int DEFAULT_LISTEN_PORT = 8888;
    private static readonly int DEFAULT_PORT = 9999;
    private static readonly string DEFAULT_HOST = "localhost";

    private static readonly string SCRIPT =
       "ttt" + "." + Server.SCRIPT_EXTENSION;

    private static readonly string ID = "id";
    private static readonly string MOVE_ROW = "move_row";
    private static readonly string MOVE_COLUMN = "move_column";

    private static readonly string PLAYER_ONE = "Player One";
    private static readonly string PLAYER_TWO = "Player Two";

    private static readonly char MARK_X = 'X';
    private static readonly char MARK_O = 'O';

    // ---------------------------
    // Our buttons
    // ---------------------------

    private Button mButtonQuit;
    private Button mButtonStart;
    private Button mButtonConnectSettings;

    private Button[][] mButtons = new Button[10][];

    private Label MessageLabel;

    // ---------------------------
    // Our connection settings
    // ---------------------------

    private string mDestHost;
    private int mDestPort;
    private Server mServer;
    private int bDim;

    // ---------------------------
    // No active game yet, we're
    // neither PLAYER_ONE or
    // PLAYER_TWO until the game
    // begins
    // ---------------------------

    private State mState = State.NO_ACTIVE_GAME;
    private string mWhichPlayer;

    // ---------------------------
    // A map between a button and
    // it's location (row/column)
    // in our grid
    // ---------------------------

    private Dictionary<Button, Location> mButtonToLocation =
       new Dictionary<Button, Location>();

    // ---------------------------
    // Game starts once these are
    // both filled out
    // ---------------------------

    private string mID;
    private string mOtherID;
    private int mOtherbDim;

    // ---------------------------
    // Our start timer
    // ---------------------------

    private Timer mTimer;
    private int mTimerCount;

    // ---------------------------
    // Constructor
    // ---------------------------

    public TTT(int listenPort, string destHost, int destPort, int boardDimensions)
    {
        mDestHost = destHost;
        mDestPort = destPort;
        bDim = boardDimensions;

        mServer = new Server(listenPort);

        mServer.RegisterScriptHandler(Server.SCRIPT_EXTENSION, HandleRequest);

        // Size ourself

        Size = new Size(400, 500);

        // Our input buttons

        for (int i = 0; i < bDim; i++)
        {
            mButtons[i] = new Button[bDim];

            for (int j = 0; j < bDim; j++)
            {
                mButtons[i][j] = new Button();                
                mButtons[i][j].Click += new EventHandler(DoButton);

                mButtonToLocation[mButtons[i][j]] = new Location(i, j);

                Controls.Add(mButtons[i][j]);
            }
        }

        // Quit/Start buttons

        mButtonQuit = new Button();
        mButtonQuit.Text = "Quit";
        Controls.Add(mButtonQuit);
        mButtonQuit.Click += new System.EventHandler(
            delegate{ Application.Exit(); }
        );

        mButtonStart = new Button();
        mButtonStart.Text = "Start";
        Controls.Add(mButtonStart);
        mButtonStart.Click += new System.EventHandler(DoStart);

        Resize += new System.EventHandler(
           delegate { DoLayout(); }
        );

        mButtonConnectSettings = new Button();
        mButtonConnectSettings.Text = "Connection Settings";
        Controls.Add(mButtonConnectSettings);
        mButtonConnectSettings.Click += new System.EventHandler(
            delegate { ConnectionSettings(); }
        );

        MessageLabel = new Label();
        MessageLabel.Text = "Press start to find a player!";
        Controls.Add(MessageLabel);

        DoLayout();
    }

    private static string GetMark(string whichPlayer)
    {
        string theMark = "";

        if (whichPlayer == PLAYER_ONE)
            theMark += MARK_X;

        else if (whichPlayer == PLAYER_TWO)
            theMark += MARK_O;



        return (theMark);
    }

    private static string GetMarkOther(string whichPlayer)
    {
        if (whichPlayer == PLAYER_ONE)
            return (GetMark(PLAYER_TWO));

        if (whichPlayer == PLAYER_TWO)
            return (GetMark(PLAYER_ONE));

        return ("");
    }

    private void ShowMessage(string theMessage)
    {
        string prefix =
           (mWhichPlayer == null ? "Message" : mWhichPlayer) + " - ";

        Text = prefix + theMessage;
    }

    public void HandleGuiRequest(Dictionary<string, string> request)
    {
        if (request.ContainsKey(ID))
        {
            mOtherID = request[ID];
        }
        if(request.ContainsKey("DIM"))
        {
            mOtherbDim = Convert.ToInt32(request["DIM"]);
        }
        else if (request.ContainsKey(MOVE_ROW) &&
                request.ContainsKey(MOVE_COLUMN))
        {
            int row = Convert.ToInt32(request[MOVE_ROW]);
            int column = Convert.ToInt32(request[MOVE_COLUMN]);

            mButtons[row][column].Text = GetMarkOther(mWhichPlayer);
            mButtons[row][column].Font = new Font("Arial", 14, FontStyle.Bold);

            mState = State.MY_TURN;
            mButtonConnectSettings.Enabled = false;
            MessageLabel.Text = "Your Turn!";
            CheckForGameOver();
        }
    }

    public string HandleRequest(
       string script, Dictionary<string, string> request
    )
    {
        // Moved to HandleGuiRequest()

        /*if (request.ContainsKey(ID))
        {
            mOtherID = request[ID];
        }
        else if (request.ContainsKey(MOVE_ROW) &&
                request.ContainsKey(MOVE_COLUMN))
        {
            int row = Convert.ToInt32(request[MOVE_ROW]);
            int column = Convert.ToInt32(request[MOVE_COLUMN]);

            mButtons[row][column].Text = GetMarkOther(mWhichPlayer);

            mState = State.MY_TURN;
            ShowMessage("Your Turn");
            CheckForGameOver();
        }*/

        this.Invoke
            ((MethodInvoker)delegate() { HandleGuiRequest(request); });

        string s =
           "<!doctype>\n" +
           "<html>" +
           "<head><title>HandleRequest</title></head>" +
           "<body>HandleRequest</body>" +
           "</html>";

        return (s);
    }

   private void ClearBoard()
   {
      for(int i = 0; i < bDim; i ++)
         for(int j = 0; j < bDim; j ++)
            mButtons[i][j].Text = "";
   }

   public void CheckForGameOver()
   {
      if(CheckForVictory() || CheckForDraw())
      {
         mState = State.NO_ACTIVE_GAME;
         mButtonConnectSettings.Enabled = true;
         mButtonStart.Enabled = true;

         mID = mOtherID = null;

         mWhichPlayer = null;
      }
   }

   public void DoLayout()
   {
      int theSize   = (int) (( (Math.Min(GetWidth(), GetHeight())) -(10*bDim) )/bDim );
      int theHeight = (int)(.8 * GetHeight());

      // Our input buttons

      for(int i = 0; i < bDim; i ++)
      {
         for(int j = 0; j < bDim; j ++)
         {
            mButtons[i][j].Size = new Size(theSize, theSize);

            mButtons[i][j].Left = (int)(10 + j*theSize);

            mButtons[i][j].Top = (int)(10 + i * theSize);

             /*
              mButtons[i][j].Top =
               (int)((i + 1) / (bDim + 2.0) * theHeight) -
               mButtons[i][j].Height / 2;
              */
         }
      }

      // Quit/ Connection /Start buttons

      

      mButtonQuit.Left = GetWidth() / 4 - mButtonQuit.Size.Width / 2;
      mButtonQuit.Top  = (int)(.9 * GetHeight());

      mButtonConnectSettings.Left = (2 * GetWidth() / 4) - mButtonQuit.Size.Width / 2;
      mButtonConnectSettings.Top = (int)(.9 * GetHeight());

      mButtonStart.Left = (3 * GetWidth() / 4) - mButtonQuit.Size.Width / 2;
      mButtonStart.Top  = (int)(.9 * GetHeight());

      MessageLabel.Left = 10;
      MessageLabel.Top = mButtonStart.Top- 40;
      MessageLabel.Width = GetWidth() - 2*(MessageLabel.Left);

      MessageLabel.TextAlign = ContentAlignment.MiddleCenter;
      MessageLabel.Font = new Font("Arial", 12, FontStyle.Bold);
      
   }

    public void ConnectionSettings()
   {
        //quick and dirty way of redoing connection settings. Has its own problems (i.e. cancelling opens up a second window without closing the first)
       Application.Restart();
            
   }

    private void DoStart(object sender, EventArgs args)
    {
        // Disable the button while we try to start the game

        mButtonStart.Enabled = false;
        mButtonConnectSettings.Enabled = false;

        // Ok, generate our unique id and send it to the other player

        string theID = System.Guid.NewGuid().ToString();
        var kv = new Dictionary<string, string>();

        kv[ID] = theID;
        kv["DIM"] = bDim.ToString();

        string errMsg;
        string contents;

        bool result = Client.DoGetRequest(
           mDestHost,
           mDestPort,
           SCRIPT,
           kv,
           out contents,
           out errMsg
        );

        if (!result)
        {
            MessageBox.Show("[" + errMsg + "]");
            mButtonStart.Enabled = true;
            mButtonConnectSettings.Enabled = true;
        }
        else
        {
            // Store off our id and wait for the other player
            // contact us

            mID = theID;
            mTimerCount = 0;

            mTimer = new Timer();
            mTimer.Tick += new EventHandler(DoCheckStart);
            mTimer.Interval = 1000;

            mTimer.Start();

            ClearBoard();
        }
    }

    private void DoCheckStart(object sender, EventArgs args)
    {
        // If the other player has contacted us, see who
        // should be player one

        if (mOtherID != null)
        {
            mTimer.Stop();

            if(bDim != mOtherbDim)
            {
                mTimer.Stop();

                MessageBox.Show(
                   "Your board Dimensions dont match!"
                );

                mID = null;
                mButtonStart.Enabled = true;
                mButtonConnectSettings.Enabled = true;

                MessageLabel.Text = "Press start to find a player!";

                return;
            }

            if (String.Compare(mID, mOtherID) > 0)
            {
                mWhichPlayer = PLAYER_ONE;
                mState = State.MY_TURN;
                mButtonConnectSettings.Enabled = false;

                MessageLabel.Text = "Your Turn!";
            }
            else
            {
                mWhichPlayer = PLAYER_TWO;
                mState = State.OTHER_TURN;
                mButtonConnectSettings.Enabled = false;

                MessageLabel.Text = "Other Player's Turn!";
                
            }
        }
        else if (++mTimerCount == MAX_START_SECONDS)
        {
            mTimer.Stop();

            MessageBox.Show(
               "Start - timed out (" + MAX_START_SECONDS + ") seconds"
            );

            mID = null;
            mButtonStart.Enabled = true;
            mButtonConnectSettings.Enabled = true;
        }
        else
        {
            MessageLabel.Text =
               "Awaiting other player " +
               "(" + mTimerCount + "/" + MAX_START_SECONDS + ")";
        }
    }

    private void DoButton(object sender, EventArgs args)
    {
        // Button presses only "count" when it is our turn

        if (mState != State.MY_TURN)
            return;

        Button theButton = (Button)sender;

        // Is this spot already occupied?

        if (theButton.Text != "")
            return;

        // Tell the other player the row/column we chose

        Location theLocation = mButtonToLocation[theButton];
        var kv = new Dictionary<string, string>();

        kv[MOVE_ROW] = "" + theLocation.Row;
        kv[MOVE_COLUMN] = "" + theLocation.Column;

        string errMsg;
        string contents;

        bool result = Client.DoGetRequest(
           mDestHost,
           mDestPort,
           SCRIPT,
           kv,
           out contents,
           out errMsg
        );

        if (!result)
        {
            MessageBox.Show(errMsg);
            return;
        }

        // Update our display, our state and our title message

        theButton.Text = GetMark(mWhichPlayer);
        theButton.Font = new Font("Arial", 14, FontStyle.Bold);
        //theButton.Text
        mState = State.OTHER_TURN;
        mButtonConnectSettings.Enabled = false;

        MessageLabel.Text = "Other Player's Turn!";

        CheckForGameOver();
    }

    private bool Victory(string markString)
    {
        bool found;

        for (int i = 0; i < bDim; i++)
        {
            // Check row 'i'

            found = true;

            for (int j = 0; found && j < bDim; j++)
            {
                if (markString != mButtons[i][j].Text)
                    found = false;
            }

            if (found)
                return (true);

            // Check column 'i'

            found = true;

            for (int j = 0; found && j < bDim; j++)
            {
                if (markString != mButtons[j][i].Text)
                    found = false;
            }

            if (found)
                return (true);
        }

        // now, diagonal

        found = true;

        for (int i = 0; found && i < bDim; i++)
        {
            if (markString != mButtons[i][i].Text)
                found = false;
        }

        if (found)
            return (true);

        // opposite diagonal

        found = true;

        for (int i = 0; found && i < bDim; i++)
        {
            if (markString != mButtons[i][bDim - (i + 1)].Text)
                found = false;
        }

        if (found)
            return (true);

        return (false);
    }

    private bool CheckForVictory()
    {
        bool result = true;

        if (Victory(GetMark(mWhichPlayer)))
        {
            MessageLabel.Text = "You Win!!!";
        }
        else if (Victory(GetMarkOther(mWhichPlayer)))
        {
            MessageLabel.Text = "Other Player Wins!!!";
        }
        else
        {
            result = false;
        }

        return (result);
    }

    private bool CheckForDraw()
    {
        bool flag = true;

        for (int i = 0; flag && i < bDim; i++)
            for (int j = 0; flag && j < bDim; j++)
                if (mButtons[i][j].Text == "")
                    flag = false;

        if (flag)
            MessageLabel.Text = "Game is a Draw!";

        return (flag);
   }

   protected override void OnFormClosing(FormClosingEventArgs e)
   {
      base.OnFormClosing(e);

      if(e.CloseReason == CloseReason.WindowsShutDown)
         return;

      DialogResult result = MessageBox.Show(
         "Are you sure you want to exit game?", 
         "Exit - Confirm",
         MessageBoxButtons.YesNo,
         MessageBoxIcon.Question
      );

      if(result == DialogResult.No)
      {
         e.Cancel = true;
      }
      else
      {
         string errMsg;

         if(!mServer.End(out errMsg))
            MessageBox.Show(errMsg);
      }
   }

    private int GetWidth()
    {
        return (
           Width - (2 * SystemInformation.FrameBorderSize.Width)
        );
    }

    private int GetHeight()
    {
        return (
           Height - (2 * SystemInformation.FrameBorderSize.Height +
            SystemInformation.CaptionHeight)
        );
    }

    public static void Main()
    {
        Settings theSettings = new Settings(
           DEFAULT_LISTEN_PORT,
           DEFAULT_HOST,
           DEFAULT_PORT
        );

        theSettings.ShowDialog();

        Application.Run(
           new TTT(
              theSettings.Listen, theSettings.Host, theSettings.Port, theSettings.Dimensions
           )
        );
    }
}

// ---------------
// Location
// ---------------

public class Location
{
    public Location(int row, int column)
    {
        Row = row;
        Column = column;
    }

    public int Row { get; set; }
    public int Column { get; set; }

    public override string ToString()
    {
        return ("{ row: " + Row + ", column: " + Column + "}");
    }
}
