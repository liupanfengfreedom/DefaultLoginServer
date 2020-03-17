#define UTF16
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
namespace DefaultLoginServer
{
    public delegate void OnReceivedCompleted(byte[] buffer);
    enum MessageTypev1
    {
        PLAYER,//
        MATCHSERVER,//
    }
    struct FMessagePackagev1
    {
        public MessageTypev1 MT;

        public string PayLoad;
        public FMessagePackagev1(string s)
        {
            MT = MessageTypev1.PLAYER;
            PayLoad = "";
        }
    }
    class TcpClient
    {
        static List<TcpClient> playerlist = new List<TcpClient>();
        const int BUFFER_SIZE = 65536;
        public byte[] receivebuffer = new byte[BUFFER_SIZE];
        Socket clientsocket;
        Thread ReceiveThread;
        public OnReceivedCompleted OnReceivedCompletePointer = null;

        public TcpClient(Socket msocket)
        {
            Console.WriteLine("TCPClient " + msocket.RemoteEndPoint);
            clientsocket = msocket;
            ReceiveThread = new Thread(new ThreadStart(ReceiveLoop));
            ReceiveThread.IsBackground = true;
            ReceiveThread.Start();
            OnReceivedCompletePointer += messagehandler;
        }
        public void Send(byte[] buffer)
        {
            if (clientsocket != null)
            {
                clientsocket.Send(buffer);
                //buffer.CopyTo(sendbuffer, 0);
                //clientsocket.Send(sendbuffer);
                //Array.Clear(sendbuffer, 0, SENDBUFFER_SIZE);
                Thread.Sleep(300);
            }
        }
        public void Send(String message)
        {
#if UTF16
            UnicodeEncoding asen = new UnicodeEncoding();
#else
            ASCIIEncoding asen = new ASCIIEncoding();
#endif
            if (clientsocket != null)
            {
                this.Send(asen.GetBytes(message));
            }
        }
        void ReceiveLoop()
        {
            while (true)
            {
                try
                {
                    Array.Clear(receivebuffer, 0, receivebuffer.Length);
                    clientsocket.Receive(receivebuffer);
                    OnReceivedCompletePointer += messagehandler;

                    Thread.Sleep(30);
                }
                catch (SocketException)
                {

                    ReceiveThread.Abort();
                }
            }

        }
        void messagehandler(byte[] buffer)
        {
            FMessagePackagev1 mp;
            try
            {
#if UTF16
                var str = System.Text.Encoding.Unicode.GetString(buffer);
#else
                var str = System.Text.Encoding.UTF8.GetString(buffer);
#endif
                bool bisjson = Utility.IsValidJson(str);
                if (!bisjson)
                {                   
                    return;
                }
                mp = JsonConvert.DeserializeObject<FMessagePackagev1>(str);
                switch (mp.MT)
                {
                    case MessageTypev1.PLAYER:
                        /////////////////////////////////////////////////////
                        ///
                        playerlist.Add(this);
                        break;
                    case MessageTypev1.MATCHSERVER:

                        break;
                }

            }
            catch (Newtonsoft.Json.JsonSerializationException)
            {//buffer all zero//occur when mobile client force kill the game client

            }
        }
    }
}
