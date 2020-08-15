using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Player1;
using createRooms2;

namespace tryPlayer2
{
    public partial class Form2_player2 : Form
    {
        private string tbWord;                  //HOLDS THE WORD FROM TEXTBOX
        string[] wordList = new string[10];     //WORD ARRAY
        //int advanceCounter = 0;                 //USED TO GET THE NEXT WORD
        string newWord;
        int randomNumber;
        Random ranNumberGenerator;//STORE THE NEW WORD
        string chosenCategory;

        NetworkStream nstream;
        NetworkStream nstream2;
        TcpClient client;
        Player p = new Player();
        string JSONString;

        int scoreFlag;
        HomeForm frm3;
        Form1 frm1;
        string imgsPath;
        public Form2_player2()
        {
            InitializeComponent();
            this.Text = "Player 2";
            //this.Text = this.GetType().Namespace; // sets name of form "Player 2"
            //myTurnFlag = 0;
        }
        public Form2_player2(TcpClient client1, NetworkStream nstream1, NetworkStream _nstream2, HomeForm _frm3, string _userName, Form1 _frm1 )
        {
            InitializeComponent();
            this.Text = this.GetType().Namespace;

            client = client1;
            nstream = nstream1;
            nstream2 = _nstream2;
            frm3 = _frm3;
            this.Text = _userName;
            frm1 = _frm1;
            imgsPath = frm1.imgsPath;
        }

        private void rematch()
        {
            label1.Visible = false;
            categoryLabel.Visible = false;

            PlayAgainBtn.Enabled = false;

            //string result = null;
            if (scoreFlag == 1)
            {
                resultPanel.Visible = true;
                pictureBox2.Image = Image.FromFile(imgsPath + @"CSharpProject_G4\tryPlayer2\winner.jpg");
                //result = "Congratulations, You won";
                wordLabel.Visible = false;
            }
            else if (scoreFlag == 0)
            {
                resultPanel.Visible = true;
                pictureBox2.Image = Image.FromFile(imgsPath + @"CSharpProject_G4\tryPlayer2\loser.jpg");
                wordLabel.Visible = true;
                wordLabel.Text = "The word was: " + newWord;
                //result = "You lost, the word was " + "'" + newWord + "'";
            }
            Thread.Sleep(5000);
            PlayAgainBtn.Enabled = true;
            
        }
        private Player applyOnPlayer(Player p)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    applyOnPlayer(p);
                });
            }
            else
            {
                textBox1.Text = p.GuessedWord;
                //for (int k = 0; k < groupBox1.Controls.Count; k++)
                //{
                //    if (groupBox1.Controls[k] is Button)
                //    {
                //        groupBox1.Controls[k].Enabled = p[k];
                //    }
                //}
                if(p.EndGame == 1)
                {
                    scoreFlag = 0; //lost
                    rematch();
                    //MessageBox.Show("You lost, The word was: " + newWord);
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
            return p;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        
        void disableLetters()
        {
            #region disabling 26 letters btns
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
#endregion
        }

        private void A_Click(object sender, EventArgs e)
        {
            checkGuessedLetter(newWord.ToUpper(), A.Text, A);
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
        private void button2_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    string JsonString;
                    Player p2 = new Player();
                    //MessageBox.Show("TurnFlag: " + p.TurnFlag);
                    while (true)
                    {
                        if (p.TurnFlag == 1)
                        {
                            disableBtns();

                            byte[] data = new byte[600];
                            int bytes = nstream.Read(data, 0, data.Length);
                            JsonString = string.Empty;
                            JsonString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                            //MessageBox.Show("Message, p2: " + JSONString);
                            //Player p = new Player();
                            p = JsonConvert.DeserializeObject<Player>(JsonString);
                            //MessageBox.Show(JsonString);
                            applyOnPlayer(p);
                            newWord = p.TheWord;
                            chosenCategory = p.theCategory;
                            setCategoryLabel();
                        }

                        int while2Flag = 1;
                        if (p.TurnFlag == 2)
                        {
                            if (while2Flag == 1)
                            {
                                enableLetters(p);
                                while2Flag = 0;
                            }

                            byte[] data = new byte[1000];
                            int bytes = nstream.Read(data, 0, data.Length);
                            JsonString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                            //MessageBox.Show("Message, p2: " + JSONString);
                            //Player p = new Player();
                            p = JsonConvert.DeserializeObject<Player>(JsonString);
                            //MessageBox.Show(JsonString);
                            applyOnPlayer(p);
                            newWord = p.TheWord;
                            chosenCategory = p.theCategory;
                            setCategoryLabel();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error at Player 2: \n"+ex.ToString());
                    /*DialogResult dd = MessageBox.Show("Do you want to play again?", " ", MessageBoxButtons.YesNo);
                    if (dd == DialogResult.Yes)
                    {
                        MessageBox.Show("Go Study!");
                    }
                    else if (dd == DialogResult.No)
                    {
                        MessageBox.Show("No pressed.");
                    }*/
                }


            }).Start();
        }

        private void setCategoryLabel()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    setCategoryLabel();
                });
            }
            else
            {
                label1.Visible = true;
                categoryLabel.Visible = true;
                categoryLabel.Text = chosenCategory;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkGuessedLetter(string wordToGuess, string guessedLetter, Button butName)
        {
            tbWord = textBox1.Text;                //GET THE TEXT THAT'S CURRENTLY IN THE WORD TEXTBOX

            int strLen = wordToGuess.Length;        //USE THE LENGTH PROPERTY OF THE wordToGuess STRING

            string guessesLeft = (textBox1.Text); //SEE HOW MANY GUESSES A PLAYER HAS LEFT

            int result = 0;
            int counter = 0;
            int foundLen = 0;
            string newChar = "";
            int guessedCorrectlyCounter = 0;
            for (int i = 0; i < strLen; i++)
            {
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
            int guess = guessesLeft.Length;
            if (guessedCorrectlyCounter == 0)
            {

                guess = guess - 1;
               

                p.TurnFlag = 1;
            }

            //=================================================
            //          CHECK HOW MANY GUESSESS LEFT. 
            //          IF ZERO - GAME OVER, AND RESET
            //=================================================
            //PLACE THE NEW VERSION OF THE WORD BACK INTO THE TEXTBOX
            textBox1.Text = tbWord;
            if (tbWord == wordToGuess) //winning condition
            {
                p.GuessedWord = textBox1.Text;
                p.EndGame = 1;
                p.btns.Clear();
                foreach (Control c in groupBox1.Controls)
                {
                    if (c is Button)
                    {
                        //MessageBox.Show(c.Enabled.ToString());
                        if (c.Enabled == false)
                        {
                            p.btns.Add(c.Name);
                        }
                    }
                }
                string _JsonString = JsonConvert.SerializeObject(p);
                byte[] _JSONString_byte = System.Text.Encoding.ASCII.GetBytes(_JsonString);
                //MessageBox.Show("json,p1: " + JsonString);

                nstream.Write(_JSONString_byte, 0, _JSONString_byte.Length);
                scoreFlag = 1; //won
                rematch();
                //MessageBox.Show("You Won - Well Done!");
                disableLetters();
                button2.Enabled = true;
            }

            //=================================================================================
            //      THIS IS WHERE WE USE THE BUTTON OBJECT THAT WE PASSED OVER TO THE METHOD.
            //      ALL WE'RE DOING IS SWITCHING THE LETTER BUTTON OFF
            //=================================================================================
            butName.Enabled = false;

            //-------------------sending to player 1
            p.TheWord = newWord;
            p.GuessedWord = textBox1.Text;
            p.btns.Clear();
            foreach (Control c in groupBox1.Controls)
            {
                if (c is Button)
                {
                    //MessageBox.Show(c.Enabled.ToString());
                    if (c.Enabled == false)
                    {
                        p.btns.Add(c.Name);
                    }
                }
            }

            string JsonString = JsonConvert.SerializeObject(p);
            byte[] JSONString_byte = System.Text.Encoding.ASCII.GetBytes(JsonString);
            //MessageBox.Show("json,p1: "+JsonString);

            nstream.Write(JSONString_byte, 0, JSONString_byte.Length);
            nstream.Flush();
            
            if (p.TurnFlag == 1)
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
        void enableLetters(Player p)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    enableLetters(p);
                });
            }
            else
            {
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

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
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

        private void PlayAgainBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Go Study!");
            this.Hide();
            //this.Refresh();
            //try
            //{
                TcpClient client2 = new TcpClient();

                client2.Connect("172.16.4.45", 50000);
                //client2.Connect("192.168.1.8", 50000);
            //client2.Connect((sender as Button).Text.Split(':')[0], 50000);

            //client2.Connect("172.16.4.45", 50000);
            NetworkStream nnstream = client2.GetStream();
                NetworkStream nnstream2 = client2.GetStream();
            ///////////////////to check connection///////////////////////////
            //bool blockingState = client2.Client.Blocking;
            try
                {
                    //byte[] tmp = new byte[1];

                    //client2.Client.Blocking = false;
                    //client2.Client.Send(tmp, 0, 0);
                    //MessageBox.Show("Connected!");

                    Byte[] data = new Byte[256];
                    Int32 bytes;

                    data = System.Text.Encoding.ASCII.GetBytes("Hi from player 2");
                    nnstream.Write(data, 0, data.Length);
                //MessageBox.Show("Sent");

                bytes = nnstream.Read(data, 0, data.Length);
                    String mess = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                //MessageBox.Show("Player 1: " + mess);

                /*foreach (Room r in roomsList)
                {
                    //This foreach loop is still not doing its purpose properly
                    if (r.RoomID == roomID)
                    //if(r.RoomID == int.Parse((sender as Button).Text.Split(':')[1]))
                    {
                        r.BusyFlag = 1; //2 players are in the room
                        r.Player2IP = myIP;
                        Connect(serverIP, "3", r.OwnerIP);
                    }
                }*/
                Form2_player2 frm = new Form2_player2(client2, nnstream, nnstream2, frm3, this.Text, frm1); //this.Text is the userName
                    frm.Text = this.Text;
                    frm.Show();
                }
                catch (SocketException ex)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (ex.NativeErrorCode.Equals(10035))
                        MessageBox.Show("Still Connected, but the Send would block");
                    else
                    {
                        MessageBox.Show("Disconnected: error code " + ex.NativeErrorCode);
                    }
                }
            //}   
        }

        private void ExitGameBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("No pressed.");
            MessageBox.Show("Bye Bye");
            //this.Close();
            frm3.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
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
            TurnFlag = 1;
            btns = new List<string>();
        }
        
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
}
