using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tryPlayer2;

namespace ProjectTypes
{
    class ProjectTypes
    {
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
