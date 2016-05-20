using System.Windows.Forms;
using System.Drawing;
using System;

public class Boggle : Form
{
   private Board mBoard;

   private Button mQuit;
   private Button mNewGame;
   private Button mClearCurrentWord;
   private Button mSubmitCurrentWord;

   public Boggle()
   {
      // ---------------------------
      // Size our application
      // ---------------------------

      Size = new Size(500, 500);

      // -----------------------------------------------------------
      // Create our Board object and add it to our list of Controls
      // -----------------------------------------------------------

      mBoard = new Board();
      Controls.Add(mBoard);

      // ---------------------------
      // Our 'Quit' button
      // ---------------------------

      mQuit = new Button();
      mQuit.AutoSize = true;
      mQuit.Text = "&Quit";
      Controls.Add(mQuit);

      mQuit.Click += new EventHandler(
         delegate { Application.Exit(); }
      );

      // ---------------------------
      // Our 'New Game' button
      // ---------------------------

      mNewGame = new Button();
      mNewGame.AutoSize = true;
      mNewGame.Text = "&New Game";
      Controls.Add(mNewGame);

      mNewGame.Click += new EventHandler(
         delegate { DoBeginGame(); }
      );

      // ---------------------------
      // Our 'Clear Current Word' button
      // ---------------------------

      mClearCurrentWord = new Button();
      mClearCurrentWord.AutoSize = true;
      mClearCurrentWord.Text = "&Clear Current Word";
      Controls.Add(mClearCurrentWord);

      mClearCurrentWord.Click += new EventHandler(
         delegate { mBoard.ClearCurrentWord(); }
      );

      mClearCurrentWord.Enabled = false;

      // ---------------------------
      // Our 'Submit Current Word' button
      // ---------------------------

      mSubmitCurrentWord = new Button();
      mSubmitCurrentWord.AutoSize = true;
      mSubmitCurrentWord.Text = "&Submit Current Word";
      Controls.Add(mSubmitCurrentWord);

      mSubmitCurrentWord.Click += new EventHandler(
         delegate { mBoard.SubmitCurrentWord(); }
      );

      mSubmitCurrentWord.Enabled = false;

      // -----------------------------------------
      // Arrange for our controls to be redrawn
      // if our application is resized
      // -----------------------------------------

      Resize += new System.EventHandler(
         delegate { DoLayout(); }
      );

      // -----------------------------------------
      // Lay out our controls
      // -----------------------------------------

      DoLayout();
   }

   public void DoLayout()
   {
      // -------------------------------
      // Position and size our Board
      // -------------------------------

      mBoard.Left = 10;
      mBoard.Top  = 10;

      mBoard.Size = new Size(
         GetWidth() - 2 * mBoard.Left,
         (int)(.8 * GetHeight())
      );

      // -------------------------------
      // Now, lay out our buttons
      // -------------------------------

      int gap = 10;

      int total = mQuit.Width + gap + mNewGame.Width + gap +
               mClearCurrentWord.Width + gap + mSubmitCurrentWord.Width;

      mQuit.Left              = (GetWidth() - total) / 2;
      mNewGame.Left           = mQuit.Right + gap;
      mClearCurrentWord.Left  = mNewGame.Right + gap;
      mSubmitCurrentWord.Left = mClearCurrentWord.Right + gap;

      mQuit.Top              =
      mNewGame.Top           =
      mClearCurrentWord.Top  =
      mSubmitCurrentWord.Top = (int)(.9 * GetHeight());
   }

   private void DoBeginGame()
   {
      // ------------------------------------------------
      // Enable the disabled 'Clear Current Word' and
      // 'Submit Current Word' buttons then tell Board
      // to start the game
      // ------------------------------------------------

      mClearCurrentWord.Enabled  = true;
      mSubmitCurrentWord.Enabled = true;
      mBoard.BeginGame();
   }

   // ---------------------------------------------------------
   // Our application's real width and height (takes borders
   // and frames into account)
   // ---------------------------------------------------------

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

   // ---------------------------------------------------------
   // By overriding 'OnFormClosing' we can give the user a
   // chance to reconsider exiting the application
   // ---------------------------------------------------------

   protected override void OnFormClosing(FormClosingEventArgs e)
   {
      base.OnFormClosing(e);

      if (e.CloseReason == CloseReason.WindowsShutDown)
         return;

      DialogResult result = MessageBox.Show(
         "Are you sure you want to quit?",
         "Quit - Confirm",
         MessageBoxButtons.YesNo,
         MessageBoxIcon.Question
      );

      if (result != DialogResult.Yes)
      {
         e.Cancel = true;
      }
      else
      {
         // We're exiting, do any final
         // cleanup here
      }
   }

   public static void Main()
   {
      Application.Run(new Boggle());
   }
}
