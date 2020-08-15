using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
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
    public partial class HomeForm : Form
    {
        Button roombtn;
        String JSONString;
        List<System.Windows.Forms.Button> rooms;
        int roomX;
        int roomY;

        int isConnFlag;
        //int numRooms;
        List<Room> roomsList;

        TcpClient client;
        NetworkStream stream;

        public HomeForm()
        {
            InitializeComponent();
            usernametextbox.TabIndex = 0;//curser  on the textbox
        }
        
        private TcpClient Connect(String server, String message) //message is the action that player would take(create or join)
        {
            try
            {
                Byte[] data = new Byte[3000];//3000 size of array byrg3 b2a kol 7aga 
                Int32 bytes;
                if (isConnFlag == 0)     //not connected
                {
                    Int32 port = 13000; 
                    client = new TcpClient(server, port);
                    stream = client.GetStream();
                    isConnFlag = 1;   //connected
                }

                Byte[] message_byte = System.Text.Encoding.ASCII.GetBytes(message);
                stream.Write(message_byte, 0, message_byte.Length);
                //Thread.Sleep(1000);

                if (message == "0")//server sends to me  all the created rooms  as json
                {
                    bytes = stream.Read(data, 0, data.Length);
                    JSONString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                   // Convert  json to object
                    roomsList = JsonConvert.DeserializeObject<List<Room>>(JSONString);
                    //gets all rooms already created from the server and send them to form1 constructor
                    connectToForm1();
                }

                return client;

            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception: {0}", e);
                //textBox1.Text = "Exception: " + e; //ERROR: textBox1 accessed from a thread other than the one it was created on
                MessageBox.Show("Exception: " + e);
                return null;
            }
        }

        private void connectToForm1()
        {
            if (this.InvokeRequired) //gets from another thread
            {
                this.Invoke((MethodInvoker)delegate {
                    connectToForm1();
                });
            }
            else
            {                                                                      //when close app tis form close  
                this.Hide();
                Form1 createRoom = new Form1(isConnFlag, client, stream, roomsList, this, usernametextbox.Text);
                createRoom.FormClosing += (sender2, argss) =>
                {    ///same as this  bs bt3l2
                    this.Close();
                };
                createRoom.Show();
            }
        }
        private void button1_Click(object sender, EventArgs e) //login
        {
            Thread t = new Thread(() =>
            {
                isConnFlag = 0; // sent to Form1 constructor to prevent re-connecting to the server
                Thread.CurrentThread.IsBackground = true;
                Connect("192.168.1.10", "0"); //"0": to NOT create new room, only connect to the server
                //Connect("172.16.4.45", "0"); //"0": to NOT create new room
                //connectToForm1();
            });
            t.Start();
            //t.Join();
            //connectToForm1();
        }

        private void Form3Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("in home/frm3 closing");
            
        }
        

        private void HomeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //MessageBox.Show("in home/frm3 closed");
        }
    }


}
