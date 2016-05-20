using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System;
using System.IO;

// ---------------------------------------------------
// Tile - Represents one particular boggle tile, keeps
//        track of the location of the tile, its size
//        and the letter it is displaying.
// ---------------------------------------------------

public class Tile
{
   public Tile(int x, int y, int size, char letter)
   {
      X      = x;
      Y      = y;
      Size   = size;
      Letter = letter;
   }

   public int  X      { get; private set; }
   public int  Y      { get; private set; }
   public int  Size   { get; private set; }
   public char Letter { get; private set; }

   // -------------------------------------------------------------
   // Return true if (x, y) falls within our Tile, false otherwise
   // -------------------------------------------------------------

   public bool Contains(int x, int y)
   {
      if (x < X || y < Y || x > X + Size || y > Y + Size)
         return (false);

      return (true);
   }
}

// -------------------------------------------------------------
// Board - Our boggle game board which displays our 16
//         tiles and the current word (if there is one).
// -------------------------------------------------------------

public class Board : Control
{
   // -----------------------------------------------------
   // Random number generator for selecting tile letters
   // -----------------------------------------------------
    private int TotalPoints;
   private Random mRandom = new Random();

   // -----------------------------------------------------
   // Flag indicating a game is active, also the letters
   // making up our current set of tiles and current word
   // -----------------------------------------------------

   private bool   mGameIsActive = false;
   private string mCurrentTiles;
   private string mCurrentWord;

   // -----------------------------------------------------
   // Our list of Tile objects along with the set of
   // letters we randomly choose from when creating
   // our Tiles.
   // -----------------------------------------------------

   private List<Tile> mTiles = new List<Tile>();
   private bool[] bTilesClicked = new bool[16];

   private List<string> submittedWords = new List<string>();

   private static readonly string[] LETTERS =
   {
     "AACIOT",
     "DENOSW",
     "ABILTY",
     "DKNOTU",
     "ABJMOQ",
     "EEFHIY",
     "ACDEMP",
     "EGINTV",
     "ACELSR",
     "EGKLUY",
     "ADENVZ",
     "EHINPS",
     "AHMORS",
     "ELPSTU",
     "BFIORX",
     "GILRUW"
   };

   // -----------------------------------------------------
   // Constructor - Use double buffering with resize
   //               redraw for smooth drawing and updates
   // -----------------------------------------------------

   public Board()
   {
      BackColor      = Color.White;
      ResizeRedraw   = true;
      DoubleBuffered = true;

      MouseDown += new MouseEventHandler(HandleClick);
   }

   // -----------------------------------------------------
   // Generate the 16 random letters that make up
   // the new set of tiles.
   // -----------------------------------------------------

   public string GetLetters()
   {
      char[] theLetters = new char[LETTERS.Length];

      for (int i = 0; i < theLetters.Length; i++)
      {
         string s = LETTERS[i];

         char letter = s[mRandom.Next(s.Length)];

         theLetters[i] = letter;
      }

      // We now need to shuffle 'theLetters' so AACIOT isn't always
      // the source of the top-left cube, etc.

      for (int i = theLetters.Length - 1; i > 0; i--)
      {
         // Swap the letter at this spot (theLetters[i]) with
         // a random letter from: 0 -> i

         int index = mRandom.Next(i + 1);

         char tmp          = theLetters[i];
         theLetters[i]     = theLetters[index];
         theLetters[index] = tmp;
      }

      return(new string(theLetters));
   }

   // -----------------------------------------------------
   // Empty out the current word and redraw
   // -----------------------------------------------------

   public void ClearCurrentWord()
   {
      if(mGameIsActive)
      {
         mCurrentWord = "";
         resetTilesClicked();
         Refresh();
      }
   }

   // -----------------------------------------------------
   // Display the current word (if one is present) and
   // then clear it.
   // -----------------------------------------------------

   public void SubmitCurrentWord()
   {
      if (mGameIsActive)
      {
         string theMessage;

         if (mCurrentWord == "")
            theMessage = "No current word found";

         else if (submittedWords.Contains(mCurrentWord))
         {
             theMessage = "This word has already been submitted";
         }
         else if(!acceptedWords.Contains(mCurrentWord))
         {
             theMessage = "This word is not on the accepted words list";
         }
         else
         {
             submittedWords.Add(mCurrentWord);
             int points = mCurrentWord.Length;
             TotalPoints += points;
             theMessage = "Added Word: [" + mCurrentWord + "]\nTotal Score: "+TotalPoints;
             ClearCurrentWord();
             resetTilesClicked();
         }

         MessageBox.Show(theMessage);
        
      }
   }

   // -----------------------------------------------------
   // Start a new game
   // -----------------------------------------------------
   private List<string> acceptedWords = new List<string>();
   public void BeginGame()
   {
      mCurrentTiles = GetLetters();
      mCurrentWord  = "";
      mGameIsActive = true;
      submittedWords.Clear();
      TotalPoints = 0;
      resetTilesClicked(); 

      try
      {
          // Only get files that begin with the letter "c." 
          
          string[] dirs = Directory.GetFiles(Directory.GetCurrentDirectory(), "acceptedWords.txt");
         
          foreach (string line in File.ReadLines(dirs[0]))
          {
              if(!acceptedWords.Contains(line))
              {
                  acceptedWords.Add(line.ToUpper());
              }
          }
      }
      catch (Exception e)
      {
          Console.WriteLine("The process failed: {0}", e.ToString());
      }

      foreach (string word in acceptedWords)
          Console.WriteLine(word);

      Refresh();

      
   }
    private void resetTilesClicked()
   {
       for (int i = 0; i < bTilesClicked.Length; i++)
           bTilesClicked[i] = false;
   }
   // -----------------------------------------------------
   // Draw the current state of the game to a Bitmap
   // -----------------------------------------------------

   private Bitmap GetPicture()
   {
      Bitmap bm = new Bitmap(Width, Height);

      Graphics g = Graphics.FromImage((Image)bm);

      g.FillRectangle(
         new SolidBrush(Color.Black),
         0, 0, Width, Height
      );

      g.FillRectangle(
         new SolidBrush(BackColor),
         1, 1, Width - 2, Height - 2
      );

      if(mGameIsActive)
      {
         // Draw our board

         double w = (Width / 5.0);
         double h = (Height / 5.0);

         int size = (int)(.8 * Math.Min(w, h));

         int fontSize = Width / 25;
         int offset = (int)(.55 * fontSize + 0.5);
         Font theFont = new Font("Arial", fontSize);

         // We recalculate our tiles' dimensions each time

          mTiles = new List<Tile>();
         

         int index = 0;

         for(int i = 0; i < 4; i++)
         {
            int y = (int)(Height - (i + 1) * h + 0.5);

            for(int j = 0; j < 4; j++)
            {
               int x = (int)((j + 1) * w + 0.5);

               int xValue = x - size / 2;
               int yValue = y - size / 2;
               char letter = mCurrentTiles[index];

               g.DrawRectangle(
                  new Pen(Color.Black),
                  xValue, yValue, size, size
               );

               mTiles.Add(
                  new Tile(xValue, yValue, size, letter)
               );

               g.DrawString(
                  "" + mCurrentTiles[index],
                  theFont,
                  new SolidBrush(Color.Black),
                  x - offset,
                  y - offset
               );

               ++index;
            }
         }

         string theText = "Current Word: ";

         // Draw current word

         if (mCurrentWord != "")
            theText += mCurrentWord;

         g.DrawString(
            theText,
            new Font("Arial", (int)(.8 * fontSize + 0.5)),
            new SolidBrush(Color.Black),
            (int)(.1 * Width),
            (int)(.925 * Height)
         );
      }

      g.Dispose();

      return(bm);
   }

   // -----------------------------------------------------
   // Draw the game by rendering it to an image and
   // then drawing that image
   // -----------------------------------------------------

   protected override void OnPaint(PaintEventArgs e)
   {
      e.Graphics.DrawImage(GetPicture(), new Point(0, 0));
   }

   // -----------------------------------------------------
   // Respond to mouse clicks
   // -----------------------------------------------------

   private void HandleClick(object sender, MouseEventArgs e)
   {
      if(mGameIsActive)
      {
         // Run through our list of tiles seeing if any
         // contains the mouse click (e.X, e.Y)

         bool doRefresh = false;

         foreach(Tile t in mTiles)
         {
             
             if (t.Contains(e.X, e.Y) && mCurrentWord.Length < 16 && bTilesClicked[mTiles.IndexOf(t)]==false)
            {
                bTilesClicked[mTiles.IndexOf(t)] = true;
               mCurrentWord += t.Letter;
               doRefresh     = true;
            }
         }

         if(doRefresh)
            Refresh();
      }
   }
}
