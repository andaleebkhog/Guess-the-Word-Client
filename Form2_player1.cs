using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading;
using tryPlayer2;
using createRooms2;
using System.Net;

namespace Player1
{
    public partial class Form2_player1 : Form
    {

        string imgsPath; // the path of images displayed in the form - sent from form 1

        private string tbWord;                  //HOLDS THE WORD FROM TEXTBOX
        string[] wordList = new string[10];     //WORD ARRAY
        //int advanceCounter = 0;                 //USED TO GET THE NEXT WORD
        string newWord;

        string chosenCategory; // the category chosen by player1


        NetworkStream nstream; // network stream between player1 and player2
        TcpClient client;
        Player p = new Player();

        int scoreFlag; //1: won, 0: lost

        HomeForm frm3; // object from home page form used to close the application if ext button is clicked
        Form1 frm1; // object from form1 (used in the rematch and to pass the imgsPath)
        public Form2_player1()
        {
            InitializeComponent();
            p.TurnFlag = 1; //player1 is always the one who takes turn first
        }
        public Form2_player1(TcpClient client1, NetworkStream nstream1, string _newWord, string _chosenCategory, HomeForm _frm3, string _userName, Form1 _frm1)
        {
            InitializeComponent();
            //this.Text = this.GetType().Namespace; // sets name of form
            this.Text = _userName;

            client = client1;
            nstream = nstream1;
            newWord = _newWord;
            frm3 = _frm3;

            chosenCategory = _chosenCategory;

            frm1 = _frm1;

            imgsPath = frm1.imgsPath;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

            disableLetters(); // letters btns are disabled once started until "play" btn is clicked

            //receive from player2 in a parallel thread
            new Thread(() =>
            {
                try
                {
                    while(true)
                    {
                        int while2Flag = 1; //tol mana 3lia el dor  avoid flashing
                        Player p2 = new Player();
                        //MessageBox.Show("TurnFlag: " + p.TurnFlag);
                        while (p.TurnFlag == 2)
                        {
                            disableBtns();

                            byte[] data = new byte[600];
                            int bytes = nstream.Read(data, 0, data.Length);
                            string JsonString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                            //MessageBox.Show("Message: " + JSONString);
                            //Player p = new Player();
                            p = JsonConvert.DeserializeObject<Player>(JsonString); // to recignize once the player take any action
                            
                            // apply the GUI changes received from player2
                            p2 = applyOnPlayer(p);

                            //----------------------------
                            if (p.TurnFlag == 1) // turn of player1
                            {
                                if (while2Flag == 1) // flag to enter only one time when the turn comes to player1 (avoid flashing of letters btns)
                                {
                                    enableLetters(p2); // enable letter for player1 to play
                                    //3shan ashof p2 das 3la aih f a5leha disabled 3ndy
                                    while2Flag = 0;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }).Start();
        }
        void disableLetters()
        {
            A.Enabled = false;
            B.Enabled = false;
            C.Enabled = false;
            D.Enabled = false;
            E.Enabled = false;
            F.Enabled = false;
            G.Enabled = false;
            H.Enabled = false;
            I.Enabled = false;
            J.Enabled = false;
            K.Enabled = false;
            L.Enabled = false;
            M.Enabled = false;
            N.Enabled = false;
            O.Enabled = false;
            P.Enabled = false;
            Q.Enabled = false;
            R.Enabled = false;
            S.Enabled = false;
            T.Enabled = false;
            U.Enabled = false;
            V.Enabled = false;
            W.Enabled = false;
            X.Enabled = false;
            Y.Enabled = false;
            Z.Enabled = false;
        }
        
        private void button2_Click(object sender, EventArgs e) //play
        {
            //MessageBox.Show(newWord);
            textBox1.Text= "";
            for (int i = 0; i < newWord.Length; i++)
            {
                textBox1.Text += "_";
            }
            enableLetters();       //ENABLE ALL THE LETTER BUTTONS
            button2.Enabled = false;
        }

        private void rematch() // showresult
        {
            if (scoreFlag == 1) //won
            {
                resultPanel.Visible = true;
                pictureBox1.Image = Image.FromFile(imgsPath + @"CSharpProject_G4\tryPlayer2\winner.jpg");
                wordLabel.Visible = false;
            }
            else if (scoreFlag == 0) //lost
            {
                resultPanel.Visible = true;
                pictureBox1.Image = Image.FromFile(imgsPath + @"CSharpProject_G4\tryPlayer2\loser.jpg");
                wordLabel.Visible = true;
                wordLabel.Text = "The word was: " + newWord;
            }

        }
        private void checkGuessedLetter(string wordToGuess, string guessedLetter, Button butName)
        {
            #region game logic
            //butName.Enabled = false;
            tbWord = textBox1.Text;  //klmt el sa7               //GET THE TEXT THAT'S CURRENTLY IN THE WORD TEXTBOX

            int strLen = wordToGuess.Length;        //USE THE LENGTH PROPERTY OF THE wordToGuess STRING

            string guessesLeft = (textBox1.Text); //SEE HOW MANY GUESSES A PLAYER HAS LEFT

            int result = 0;
            int counter = 0;
            int foundLen = 0;  //fadl kam dash
            string newChar = "";
            int guessedCorrectlyCounter = 0;
            for (int i = 0; i < strLen; i++)
            {// foundlen 3shan aktar mn char kol el feh
                result = wordToGuess.IndexOf(guessedLetter, foundLen, strLen - foundLen);

                if (result != -1)
                {
                    foundLen = result + 1;
                    counter++;

                    newChar = wordToGuess.Substring((result), 1);   //grab the letter to be replaced

                    tbWord = tbWord.Remove(result, 1);              //Remove the * character at this position

                    tbWord = tbWord.Insert(result, newChar);        //insert the new character
                    guessedCorrectlyCounter++;
                }
            }

            p.TurnFlag = 1; //player1 turn
          
            if (guessedCorrectlyCounter == 0)
            {

               
                
                p.TurnFlag = 2; //player2 turn (player1 guessed wrong)
                
            }
            
            //PLACE THE NEW VERSION OF THE WORD BACK INTO THE TEXTBOX
            textBox1.Text = tbWord;
            if (tbWord == wordToGuess) //winning condition
            {
                p.GuessedWord = textBox1.Text;
                p.EndGame = 1;
                p.btns.Clear();
                foreach (Control c in groupBox1.Controls)
                {                            // return the dimmed characters
                    if (c is Button)
                    {
                        if (c.Enabled == false)
                        {
                            p.btns.Add(c.Name);
                        }
                    }
                }
#endregion
                //rematch

                string _JsonString = JsonConvert.SerializeObject(p);
                byte[] _JSONString_byte = System.Text.Encoding.ASCII.GetBytes(_JsonString);

                nstream.Write(_JSONString_byte, 0, _JSONString_byte.Length);
                scoreFlag = 1; //won
                rematch();
                //MessageBox.Show("You Won - Well Done!");
                disableLetters();
               // button2.Enabled = true;
            }
            
            //=================================================================================
            //      THIS IS WHERE WE USE THE BUTTON OBJECT THAT WE PASSED OVER TO THE METHOD.
            //      ALL WE'RE DOING IS SWITCHING THE LETTER BUTTON OFF
            //=================================================================================
            butName.Enabled = false;
              ///ykml el game llplayer  el tany w y sent leh el characters el das 3liha
            //-------------------sending to player 2-------------------------
            //MessageBox.Show(guessesLeft);
            p.TheWord = newWord;
            p.theCategory = chosenCategory;
            p.GuessedWord = textBox1.Text;
            p.btns.Clear(); // to prevent repeating of letters 
            foreach (Control c in groupBox1.Controls)
            {
                if (c is Button)
                {
                    if(c.Enabled == false)
                    {
                        p.btns.Add(c.Name);
                    }
                }
            }
            
            string JsonString = JsonConvert.SerializeObject(p);
            byte[] JSONString_byte = System.Text.Encoding.ASCII.GetBytes(JsonString);
            //MessageBox.Show("json,p1: " + JsonString);

            nstream.Write(JSONString_byte, 0, JSONString_byte.Length);
            //nstream.Flush();
            //MessageBox.Show("send from 1 to 2");
            
            //player2 turn so buttons at player1 are disabled
            if(p.TurnFlag == 2)
            {
                foreach (Control c in groupBox1.Controls)
                {
                    if (c is Button)
                    {
                        c.Enabled = false;
                    }
                }
            }
        }
        
        private Player applyOnPlayer(Player p) // apply the GUI changes received from player2
        {
            if (this.InvokeRequired) // solve cross-threading (accessing form controls from other thread) 
            {
                this.Invoke((MethodInvoker)delegate {
                    applyOnPlayer(p);
                });
            }
            else
            {
                textBox1.Text = p.GuessedWord;

                if (p.EndGame == 1) //player2 endded the game by guessing last letter, so player1 lost 
                {
                    scoreFlag = 0; //player1 lost
                    rematch(); // ask if want to play again
                }

                for (int m = 0; m < groupBox1.Controls.Count; m++)
                {
                    if (groupBox1.Controls[m] is Button)
                    {
                        if (p.btns.Contains(groupBox1.Controls[m].Name))
                        {
                            groupBox1.Controls[m].Enabled = false;
                        }
                    }
                }
            }
            //  a when switching again they  know what is the dimmed buttons and the last word 
            return p;
        }

        private void disableBtns()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    disableBtns();
                });
            }
            else
            {
                foreach (Control c in groupBox1.Controls)
                {
                    if (c is Button)
                    {
                        c.Enabled = false;
                    }
                }
            }
        }

        void enableLetters(Player p) // overloaded method, the one that takes parameter is 
                                     // to enable the unClicked letters btns
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    enableLetters(p);
                });
            }
            else
            {
                for (int m = 0; m < groupBox1.Controls.Count; m++)
                {
                    if (groupBox1.Controls[m] is Button)
                    {
                        if (p.btns.Contains(groupBox1.Controls[m].Name))
                        {
                            groupBox1.Controls[m].Enabled = false;
                        }
                        else
                        {
                            groupBox1.Controls[m].Enabled = true;
                        }
                    }
                }
            }

        }

        private void enableLetters() // overloaded method, the one that takes NO parameter is 
                                     // to enable all letters btns at the beginning of the game
        {
            #region enabilng 26 letters btns
            A.Enabled = true;
            B.Enabled = true;
            C.Enabled = true;
            D.Enabled = true;
            E.Enabled = true;
            F.Enabled = true;
            G.Enabled = true;
            H.Enabled = true;
            I.Enabled = true;
            J.Enabled = true;
            K.Enabled = true;
            L.Enabled = true;
            M.Enabled = true;
            N.Enabled = true;
            O.Enabled = true;
            P.Enabled = true;
            Q.Enabled = true;
            R.Enabled = true;
            S.Enabled = true;
            T.Enabled = true;
            U.Enabled = true;
            V.Enabled = true;
            W.Enabled = true;
            X.Enabled = true;
            Y.Enabled = true;
            Z.Enabled = true;
#endregion
        }

        #region 26 letters btns
        private void A_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), A.Text, A);
        }
        private void B_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), B.Text, B);
        }

        private void C_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), C.Text, C);
        }

        private void D_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), D.Text, D);
        }

        private void E_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), E.Text, E);
        }

        private void F_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), F.Text, F);
        }

        private void G_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), G.Text, G);
        }

        private void H_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), H.Text, H);
        }

        private void I_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), I.Text, I);
        }

        private void J_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), J.Text, J);
        }

        private void K_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), K.Text, K);
        }

        private void L_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), L.Text, L);
        }

        private void M_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), M.Text, M);
        }

        private void N_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), N.Text,N);
        }

        private void O_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), O.Text, O);
        }

        private void P_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), P.Text, P);
        }

        private void Q_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), Q.Text, Q);
        }

        private void R_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), R.Text, R);
        }

        private void S_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), S.Text, S);
        }

        private void T_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), T.Text, T);
        }

        private void U_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), U.Text, U);
        }

        private void V_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), V.Text, V);
        }

        private void W_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), W.Text, W);
        }

        private void X_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), X.Text, X);
        }

        private void Y_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), Y.Text, Y);
        }

        private void Z_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), Z.Text, Z);
        }
#endregion

        private void PlayAgainBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            frm1.clientAsServer(this.Text); // calls method from form1 to re-Start the category choice form
            //frm1.startConnection(this.Text);
        }

        private void ExitGameBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bye Bye");
            //this.Close();
            frm3.Close();
        }

        private void Form2_player1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //System.Windows.Forms.Application.Exit();
            MessageBox.Show("in Player1 closing");
            //frm1.Close();
            //frm3.Close();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

    class Player
    {
        string playerName;
        public List<string> btns;
        string theWord;
        string guessedWord;
        
        public string theCategory { set; get; }
        public int TurnFlag { set; get; }
        public int EndGame { set; get; }
        public Player()
        {
            btns = new List<string>();
        }
        //public List<char> getBtns
        public string TheWord
        {
            set
            {
                theWord = value;
            }
            get
            {
                return theWord;
            }
        }
        public string GuessedWord
        {
            set
            {
                guessedWord = value;
            }
            get
            {
                return guessedWord;
            }
        }
    }

    public class Room
    {
        public string ownerName { set; get; }

        string ownerIP;
        public string Player2IP { set; get; }
        int roomID;
        int busyFlag;
        string clientsNames; //array of two strings(names) of the two players in the room

        public string ClientsNames
        {
            set
            {
                clientsNames = value;
            }
            get
            {
                return clientsNames;
            }
        }

        public string OwnerIP
        {
            set
            {
                ownerIP = value;
            }
            get
            {
                return ownerIP;
            }
        }
        public int RoomID
        {
            set
            {
                roomID = value;
            }
            get
            {
                return roomID;
            }
        }
        public int BusyFlag
        {
            set
            {
                busyFlag = value;
            }
            get
            {
                return busyFlag;
            }
        }
    }
}
