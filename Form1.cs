using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using Newtonsoft.Json;
using tryPlayer2;
using Player1;

namespace createRooms2
{
    public partial class Form1 : Form
    {
        string serverIP = "192.168.1.10"; // main server IP
        //string serverIP = "172.16.4.45";

        public string imgsPath = @"C:\Users\Yomna\Desktop\Final\"; // images path 
        int addressIndex = 5; // address list index

        Button roombtn;
        String JSONString;

        int roomX;
        int roomY;

        int isConnFlag;
        List<Room> roomsList;

        TcpClient client;
        NetworkStream stream;
        NetworkStream nStream;
        NetworkStream nnstream;
        NetworkStream nnstream2;

        // new server (player1 server)
        IPAddress localAddr;
        TcpListener player1Server;
        //TcpClient connection;

        TcpClient client2;

        HomeForm frm3;
        string userName;
        private int player1ConnFlag;

        //int btnFlag;
        public Form1(int _isConnFlag,TcpClient _client, NetworkStream _stream, List<Room> _roomsList, HomeForm _frm3, string _userName)
        { //el gynli mn el constructor from homeform
            InitializeComponent();

            isConnFlag = _isConnFlag;
            client = _client;
            stream = _stream;
            roomsList = _roomsList;
            frm3 = _frm3;
            userName = _userName;

            this.Text = userName;

            roomX = 12; //x-position of "create new room" button
            roomY = 12;
            
        }

        private void AddRoom(string str)  // display newly created room
        {
            if (this.InvokeRequired) // solve cross-threading (accessing form controls from other thread)
            {
                this.Invoke((MethodInvoker)delegate {
                    AddRoom(str);
                });
                return;
            }
            else
            {
                roombtn = new Button();

                // adjusting roomBtn location on the form
                if (roomX > 142 * 7)
                {
                    roomY += 131;
                    roomX = 12;
                }
                roombtn.Location = new System.Drawing.Point(roomX + 158, roomY);
                roomX += 142;
                roombtn.Size = new System.Drawing.Size(142, 129);

                /*
                 * "str" parameter is in the form:
                 * "userName,busyFlag"
                 */

                // adjusting roomBtn Text
                //roombtn.Text = str.Split(',')[0];
                try
                {
                    roombtn.Text = str.Split(',')[2];
                }
                catch(Exception exx)
                {
                    roombtn.Text = str.Split(',')[0];
                }

                roombtn.Name = "roomBtn";
                this.Controls.Add(roombtn);
                roombtn.Click += new System.EventHandler(roomBtn_Click);

                // adjust background color of btn according to the state of the room
                if(str.Split(',')[1] == "0") //busy Flag = 0
                {
                    roombtn.BackgroundImage = Image.FromFile(imgsPath + @"CSharpProject_G4\tryPlayer2\btnnnd.png");
                }
                else if (str.Split(',')[1] == "1") //busy Flag = 1
                {
                    roombtn.BackgroundImage = Image.FromFile(imgsPath + @"CSharpProject_G4\tryPlayer2\busy4.jpg");
                }

                // styling room btn
                roombtn.BackgroundImageLayout = ImageLayout.Stretch;
                roombtn.FlatStyle = FlatStyle.Flat;
                roombtn.Font = new Font("impact", 16);
                roombtn.ForeColor = Color.White;
                roombtn.FlatAppearance.BorderSize = 0;
            }
        }

        private void roomBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show((sender as Button).Text);
            string hostName = Dns.GetHostName(); //(Domain name system) of my own device,
                                                   //Retrive the Name of HOST  
                                                 // Get the IP  
            string myIP = Dns.GetHostEntry(hostName).AddressList[addressIndex].ToString();
            //MessageBox.Show(myIP);

            string roomIP = null;
            int roomID = 0;
            string roomName = null;
            foreach (Room r in roomsList)
            {
                //MessageBox.Show(r.ownerName+"::"+ (sender as Button).Text);
                if(r.ownerName == (sender as Button).Text)
                {
                    roomIP = r.OwnerIP;
                    roomID = r.RoomID;
                    roomName = r.ownerName;
                }
            }
            //MessageBox.Show("myIP: " + myIP + "::"+ "roomIP: " + roomIP);

            /*check the which player is clicking the room btn
             * if player1 (room creator): player1 starts to be a server and listens for requests
             * from other players
             * if player2: it sends a request to player1 to connect to his room
             */
            if (myIP == roomIP) // in case: player1
            {
                try
                {
                    clientAsServer(roomName); // methods that configures player1 as a server
                }
                catch { MessageBox.Show("cannot be server"); }
            }
            else // in case: player2 (or watcher)
            {
                new Thread(() =>
                {
                    //MessageBox.Show("Entered the Room");
                    try
                    {
                        client2 = new TcpClient();

                        client2.Connect(roomIP, 50000); // palyer2 connecting to player1

                        nnstream = client2.GetStream();
                        nnstream2 = client2.GetStream();
                        try
                        {
                            Byte[] data = new Byte[256];
                            Int32 bytes;

                            data = System.Text.Encoding.ASCII.GetBytes("Hi from player 2");
                            nnstream.Write(data, 0, data.Length);
                            //MessageBox.Show("Sent");

                            bytes = nnstream.Read(data, 0, data.Length);
                            String mess = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                            //MessageBox.Show("Player 1: " + mess);

                            foreach (Room r in roomsList)
                            {
                                //This foreach loop is still not doing its purpose properly
                                if (r.RoomID == roomID)
                                //if(r.RoomID == int.Parse((sender as Button).Text.Split(':')[1]))
                                {
                                    r.BusyFlag = 1; //2 players are in the room
                                    r.Player2IP = myIP;
                                    Connect(serverIP, "3", r.OwnerIP);
                                }
                            }
                            connectToPlayer2();
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

                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show("cannot connect, error text: " + ee);
                    }
                }).Start();
                
            }
        }

        public void clientAsServer(string roomName)
        {
            Thread t = new Thread(() =>
            {
                if(player1ConnFlag == 0)
                {
                    string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
                                                         // Get the IP  
                                                         //MessageBox.Show(hostName);
                    string myIP = Dns.GetHostEntry(hostName).AddressList[5].ToString();
                    localAddr = IPAddress.Parse(myIP);
                    //localAddr 7tb2a el ip el 3la el button wa2t el test ;
                    player1Server = new TcpListener(localAddr, 50000);
                    //MessageBox.Show("server waiting the player......");

                    //serverOn(roomName); // indicator that player1 is listening for requests from players

                    player1Server.Start();

                    player1ConnFlag = 1;
                }

                serverOn(roomName); // indicator that player1 is listening for requests from players

                while (true) //kda et7wlt server and waiting for other player to connect
                {
                    //connectToPlayer1();
                    TcpClient connection = player1Server.AcceptTcpClient();
                    Thread tt = new Thread(() =>
                    {
                        //Thread.CurrentThread.IsBackground = true;

                        nStream = connection.GetStream();
                        //  nStream = new NetworkStream(connection);
                        Int32 bytes;
                        Byte[] data = new Byte[256];
                        String res = String.Empty;  //3rfnaha anha fadya hena msh btshawr 3la 7aga
                        // Read the Tcp Server Response Bytes.
                        bytes = nStream.Read(data, 0, data.Length);
                        res = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                        //MessageBox.Show("Player 2: " + res);

                        data = System.Text.Encoding.ASCII.GetBytes("Hi from Player1");
                        nStream.Write(data, 0, data.Length);

                        connectToPlayer1(connection, 0);
                    });
                    //MessageBox.Show("thread name: " + t.Name + ", id: ", t.ManagedThreadId.ToString());
                    threadID = tt;//for rematch
                    tt.Start();  // for any client connect to serve
                }

            });
            t.Start();   //the server of room
        }

       
        Thread threadID;
        private void serverOn(string roomName) // indicator that player1 is listening for requests from players
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    serverOn(roomName);
                });
            }
            else
            {
                foreach(Button b in this.Controls)
                {
                    if(b is Button && b.Text == roomName)
                    {
                        //b.BackgroundImage = Image.FromFile(@"C:\Users\Yomna\Desktop\Final\CSharpProject_G4\tryPlayer2\serverOn.jpg");
                        b.BackColor = Color.Gray;
                    }
                }
            }
        }
        private void connectToPlayer1(TcpClient connection, int callingFlag)
        {
            if(callingFlag == 1)
            {
                threadID.Abort();
            }
            if (this.InvokeRequired)
            {    // makesure no one is using this thread
                this.Invoke((MethodInvoker)delegate {
                    connectToPlayer1(connection, callingFlag);
                });
            }
            else
            {
                tryPlayer2.Categories game = new tryPlayer2.Categories(connection, nStream, frm3, userName, this);
                this.Hide();
                game.Show();

                game.FormClosing += (sender2, argss) =>
                {
                    this.Close();
                };
            }
        }

        private void connectToPlayer2()
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    connectToPlayer2();
                });
            }
            else
            {
                Form2_player2 game = new Form2_player2(client2, nnstream, nnstream2, frm3, userName, this);
                this.Hide();
                game.Show();

                game.FormClosing += (sender2, argss) =>
                {
                    this.Close();
                };
            }
        }
        private TcpClient Connect(String server, String message, params string[] player2Info)
            //params 3shan mmkn ab3tha  w mmkn la2.
        {
            //MessageBox.Show("isConnFlag in form1: "+isConnFlag);
            try
            {
                Byte[] data = new Byte[3000];
                Int32 bytes;
                if (isConnFlag == 0)
                {
                    Int32 port = 13000;
                    client = new TcpClient(server, port);
                    stream = client.GetStream();
                    isConnFlag = 1;
                }

                Byte[] message_byte = null;
                /// 
                if (message == "0" || message == "3")
                {
                    message_byte = System.Text.Encoding.ASCII.GetBytes(message);
                }
                else if(message == "1") // if craeting new room send with the message the user name who is creating the room
                {// sent to server  user name  with colon  so the sever split it
                    message_byte = System.Text.Encoding.ASCII.GetBytes(message+ ":" +userName);
                }
                stream.Write(message_byte, 0, message_byte.Length);
                //Thread.Sleep(1000);

                if (message == "0")
                {
                    bytes = stream.Read(data, 0, data.Length);
                    JSONString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    //List<Room> roomsList = JsonConvert.DeserializeObject<List<Room>>(JSONString);
                    roomsList = JsonConvert.DeserializeObject<List<Room>>(JSONString);
                }
                else if (message == "1")
                {    //recive from server the created romm that the client request
                    //string hostName = Dns.GetHostName();
                    bytes = stream.Read(data, 0, data.Length);
                    JSONString = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                    Room r = JsonConvert.DeserializeObject<Room>(JSONString);
                    r.ownerName = userName;
                    AddRoom(r.OwnerIP + ":" + r.RoomID + "," + r.BusyFlag + "," + r.ownerName);
                    //MessageBox.Show("Object: "+JsonConvert.SerializeObject(r));
                    roomsList.Add(r);
                }
                else if (message == "3") //player2 sends "3" to the main server when connecting to player1
                {
                    // no action at the player, the server handles it
                    string roomInfo;
                    roomInfo = "ownerIP:"+player2Info[0];
                    Byte[] roomInfo_byte = System.Text.Encoding.ASCII.GetBytes(roomInfo);
                    stream.Write(roomInfo_byte, 0, roomInfo_byte.Length);
                }

                //Thread.Sleep(2000);
                //stream.Close();
                //client.Close();

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
        

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                //Connect("172.16.4.45", "1"); //"1": to create new room
                Connect(serverIP, "1"); //"1": to create new room

            }).Start();
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageBox.Show("in form1 closing");
            frm3.Close();
            //check if player1 is alone in the room then he left, so the room should be removed
            string hostName = Dns.GetHostName(); // Retreive the Name of HOST  
                                                 // Get the IP
            string myIP = Dns.GetHostEntry(hostName).AddressList[5].ToString();
            foreach (Room r in roomsList)
            {                // lw ana  el owner bta3 el room  w mshet ams7ha
                if (r.OwnerIP == myIP && r.BusyFlag == 0)
                {
                    try
                    {
                        Byte[] data = new Byte[256];
                        // Translate the Message into ASCII.
                        //data = System.Text.Encoding.ASCII.GetBytes("client: closed, IP: " + IPAddress.Parse(((IPEndPoint)client.Client.LocalEndPoint).Address.ToString()) + ", Thread: " + Thread.CurrentThread.ManagedThreadId);
                        data = System.Text.Encoding.ASCII.GetBytes("2");
                        // Send the message to the connected TcpServer. 
                        stream.Write(data, 0, data.Length);
                        MessageBox.Show("closing form");
                    }
                    catch (Exception ee)
                    {

                    }
                }
            }
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            //once the from is loaded, all created rooms (received from home form) are displayed/drawn
            foreach (Room r in roomsList)
            {
                //MessageBox.Show("rr: " + r.ownerName);
                AddRoom(r.OwnerIP + ":" + r.RoomID + "," + r.BusyFlag + "," + r.ownerName);
            }
        }
    }
    
}
