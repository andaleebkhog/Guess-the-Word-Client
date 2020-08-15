using Player1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using tryPlayer2;
using createRooms2;

namespace tryPlayer2
{
    public partial class Categories : Form
    {
        int wLen;
        string randomWord;
        char[] newWord;
        string chosenCategory;

        TcpClient client;
        NetworkStream nStream;

        HomeForm frm3;
        Form1 frm1;

        string userName;
        public Categories(TcpClient _client, NetworkStream _nStream, HomeForm _frm3, string _userName, Form1 _frm1)
        {
            InitializeComponent();
            customizeDesign();
            //hideMenue();
            //panelCategory.Visible = true;

            client = _client;
            nStream = _nStream;
            frm3 = _frm3;
            userName = _userName;
            this.Text = userName;
            frm1 = _frm1;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void customizeDesign()
        {
            panelCategory.Visible = false;
        }
        //private void hideMenue()
        //{
        //    if(panel2.Visible == true)
        //    {
        //        panelCategory.Visible = true;
        //    }
        //}

        private void btnCategory_Click(object sender, EventArgs e)
        {
            panelCategory.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e) //furniture
        {
            string[] wordList = File.ReadAllLines("furniture.txt"); // read from chosen category file
            Random ranNumberGenerator = new Random();
            randomWord = wordList[ranNumberGenerator.Next(wordList.Length)]; // choose random word
            wLen = randomWord.Length;
            newWord = new char[30];
            //MessageBox.Show(randomWord);

            chosenCategory = "furniture";
            connectToPlayer1();
        }

        private void button3_Click(object sender, EventArgs e) //animals
        {
            string[] wordList = File.ReadAllLines("animals.txt");
            Random ranNumberGenerator = new Random();
            randomWord = wordList[ranNumberGenerator.Next(wordList.Length)];
            wLen = randomWord.Length;
            newWord = new char[30];
            //MessageBox.Show(randomWord);

            chosenCategory = "animals";
            connectToPlayer1();
        }

        private void button4_Click(object sender, EventArgs e) //food
        {
            string[] wordList = File.ReadAllLines("food.txt");
            Random ranNumberGenerator = new Random();
            randomWord = wordList[ranNumberGenerator.Next(wordList.Length)];
            wLen = randomWord.Length;
            newWord = new char[30];
            //MessageBox.Show(randomWord);

            chosenCategory = "food";
            connectToPlayer1();
        }

        private void button5_Click(object sender, EventArgs e) //sports
        {
            string[] wordList = File.ReadAllLines("sports.txt");
            Random ranNumberGenerator = new Random();
            randomWord = wordList[ranNumberGenerator.Next(wordList.Length)];
            wLen = randomWord.Length;
            newWord = new char[30];
            //MessageBox.Show(randomWord);

            chosenCategory = "sports";
            connectToPlayer1();
        }
        private void button6_Click(object sender, EventArgs e) //clothing
        {
            string[] wordList = File.ReadAllLines("clothing.txt");
            Random ranNumberGenerator = new Random();
            randomWord = wordList[ranNumberGenerator.Next(wordList.Length)];
            wLen = randomWord.Length;
            newWord = new char[30];
            //MessageBox.Show(randomWord);

            chosenCategory = "clothing";
            connectToPlayer1();
        }

        private void connectToPlayer1() // starts the game
        {
            Form2_player1 game = new Form2_player1(client, nStream, randomWord, chosenCategory, frm3, userName, frm1);
            //"chosenCategory" variable is to be sent to player2 to know which category player1 has chosen
            this.Hide();
            game.Show();

            game.FormClosing += (sender2, argss) =>
            {
                this.Close();
            };
        }

        private void Categories_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("in catg closing");
            frm1.Close();
            //frm3.Close();
        }
    }
}
