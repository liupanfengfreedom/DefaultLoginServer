using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace DefaultLoginServer
{
    struct FRoom
    {
        public string map;

        public string mapip;
        public FRoom(string s)
        {
            map = s;
            mapip = s;
        }
    }
    struct FRoomlist
    {
        public string space;
        public List<FRoom> roomlist;
        public FRoomlist(string s)
        {
            space = s;
            roomlist = new List<FRoom>();
        }
    }
    class Program
    {
        public static Config config = new Config();
        // public static string httpserver = "http://172.16.5.188:7000/";
        public static FRoomlist roomlist = new FRoomlist("");
        static void Main(string[] args)
        {
            Thread HttpServerThread;
            HttpServerThread = new Thread(new ThreadStart(httpserverthread));
            HttpServerThread.IsBackground = true;
            HttpServerThread.Start();
            while (true)
            {
                Thread.Sleep(100);
            }


        }
        static void httpserverthread()
        {
            string[] prefixes = { config.configinfor.ipaddress };//host http serer
            //string[] prefixes = { "http://localhost:7000/" };//host http serer
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }
            // URI prefixes are required,
            // for example "http://contoso.com:8000/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // Create a listener.
            HttpListener listener = new HttpListener { AuthenticationSchemes = AuthenticationSchemes.Anonymous };
            // Add the prefixes. 
            //foreach (string s in prefixes)
            //{
            //    listener.Prefixes.Add(s);
            //}
            listener.Prefixes.Add(string.Format("http://+:{0}/", config.configinfor.tccipport));
            listener.Start();
            Console.WriteLine("Listening...");
            while (true)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    new HttpListenerContextClass(context);
                    //throw (new Exception());
                }
                catch
                {
                    listener.Stop();
                    Thread.Sleep(1000);
                    break;
                }

            }
        }
    }
}
