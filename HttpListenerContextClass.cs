using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;

namespace DefaultLoginServer
{
    class HttpListenerContextClass
    {
        HttpListenerContext mhttplistenercontext;
        Thread handleThread;
        public HttpListenerContextClass(HttpListenerContext p)
        {
            mhttplistenercontext = p;
            handleThread = new Thread(new ThreadStart(handlethreadfunc));
            handleThread.IsBackground = true;
            handleThread.Start();
        }
        ~HttpListenerContextClass()
        {
            Console.WriteLine("HttpListenerContextClass deconstruct");
        }
        void handlethreadfunc()
        {
            try
            {
                HttpListenerRequest request = mhttplistenercontext.Request;
                Stream stream = request.InputStream;
                
                System.Collections.Specialized.NameValueCollection header = request.Headers;
                string[] headerallkeys = header.AllKeys;
                string mid = "";
                string rarpath = "";
                foreach (var a in headerallkeys)
                {
                    if (a.Equals("Matchserver"))
                    {
                        System.IO.Stream input1 = request.InputStream;
                        byte[] array1 = new byte[request.ContentLength64];
                        input1.Read(array1, 0, (int)request.ContentLength64);//larg file may encounter error
                        string utfString1 = Encoding.UTF8.GetString(array1, 0, array1.Length);
                        Console.WriteLine(utfString1);
                        Program.roomlist = JsonConvert.DeserializeObject<FRoomlist>(utfString1);
                        input1.Close();
                    }
                    if (a.Equals("UserName"))
                    {
                        //Console.WriteLine("username");
                        string strPayload = JsonConvert.SerializeObject(Program.roomlist);
                        HttpListenerResponse response1 = mhttplistenercontext.Response;
                        // Construct a response.
                        string responseString1 = "success";
                        //responseString += utfString;
                        byte[] buffer1 = System.Text.Encoding.UTF8.GetBytes(strPayload);
                        // Get a response stream and write the response to it.
                        response1.ContentLength64 = buffer1.Length;
                        System.IO.Stream output1 = response1.OutputStream;
                        output1.Write(buffer1, 0, buffer1.Length);
                        Thread.Sleep(500);
                        output1.Close();
                    }
                    else if (a.Equals("rarPath"))//
                    {
                        string[] values = header.GetValues(a);
                        rarpath = values[0];
                    }
                    else if (a.Equals("pakCallbackUrl"))//
                    {
                        string[] values = header.GetValues(a);
                    }
                }

                //foreach (var a in headerallkeys)
                //{
                //    if (a.Equals("rarPath"))//
                //    {
                //        string[] values = header.GetValues(a);
                //        rarmessage mmessage;
                //        mmessage.rarpath = values[0];
                //        mmessage.mid = mid;
                //        window_file_log.Log(" mmessage.rarpath :" + mmessage.rarpath);
                //        window_file_log.Log(" mmessage.mid :" + mmessage.mid);
                //        Program.rarqueue.Enqueue(mmessage);      
                //    }
                //}

                //System.IO.Stream input = request.InputStream;
                //byte[] array = new byte[request.ContentLength64];
                //input.Read(array, 0, (int)request.ContentLength64);//larg file may encounter error
                //string utfString = Encoding.UTF8.GetString(array, 0, array.Length);

                //HttpListenerResponse response = mhttplistenercontext.Response;
                //// Construct a response.
                //string responseString = "success";
                ////responseString += utfString;
                //byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                //// Get a response stream and write the response to it.
                //response.ContentLength64 = buffer.Length;
                //System.IO.Stream output = response.OutputStream;
                //output.Write(buffer, 0, buffer.Length);
                //Thread.Sleep(30);
                //input.Close();
                //output.Close();
            }
            catch(Exception e)
            {
                //throw (e);
                Console.WriteLine(e);

            }

        }

    }
}
