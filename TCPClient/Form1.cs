using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;

namespace TCPClient
{

    public delegate void WEI(string str);
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Thread t;
        string message = "";
        public string IP = "127.0.0.1";
        public int Point = 49211;
        public bool state;
        private  string receiveStr="";
        public byte[] receiveByte = new byte[1024 * 1024];

        /// <summary>
        /// 客户端连接服务器 和 重连功能，和接收数据功能。
        /// 打印是用console
        /// </summary>
        public void ConnectClient()
        {
            int count = 0;
            while(true)
            {
                Socket socketClinet = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                receiveStr = "";
                while (true)
                {
                    try
                    {
                        socketClinet.Connect(IPAddress.Parse(IP), Point);//连接服务器
                        if (socketClinet.Connected)
                        {
                            if (socketClinet.Poll(10, SelectMode.SelectRead))//是否断开。
                            {
                                try 
                                {
                                    socketClinet.Connect(IPAddress.Parse(IP), Point);//如果断开再次连接
                                    if(socketClinet.Connected)
                                    {
                                        break;//再次连接成功
                                    }
                                }
                                catch(SocketException ex)
                                {
                                    Console.WriteLine("第二次连接Fail"+ex.ErrorCode.ToString());
                                }
                            }
                           else
                                Console.WriteLine("connect success");
                                break;//connect success jump while!
                        }
                    }
                    catch (SocketException ex)
                    {
                        message = "0";
                        Console.WriteLine("连接失败" + ex.ErrorCode.ToString());
                    }
                }

                while (true)
                {
                    //在接收数据时判断是否socket连接着，不 责为true；
                    if (socketClinet.Poll(10, SelectMode.SelectRead))
                    {
                       socketClinet.Close();//如果断开就跳出此循坏从新new继续连接
                        break;
                    }
                    Console.WriteLine("zai1");
                  
                    int length = socketClinet.Receive(receiveByte);
                    receiveStr= Encoding.ASCII.GetString(receiveByte, 0, length);
                   
                  Console.WriteLine(receiveStr);
                   BeginInvoke(new WEI(receiveStrSetListBox),receiveStr);//new个委托了执行主线程控件，并将接收结果以参数模式带过去。
                  
                }
                count++;
                Console.WriteLine("从新new socket connect 第"+count);
            }
          
           

        }

        private void button1_Click(object sender, EventArgs e)
        {

            button1.Enabled = false;
            t = new Thread(ConnectClient);
            t.IsBackground = true;
            t.Start();
            string kk = ConfigurationManager.AppSettings["path"];
            listBox2.Items.Add(kk);
           // Task.Run(() => { receiveStrSetListBox(); });

        }
        
        private void receiveStrSetListBox(string s)//建个函数来访问主线程控件，因为线程之间不能直接通讯只能用委托。
        {
                if (s!=null && s!="")
                {
                Console.WriteLine(s+"调用了receiveset函数");
                listBox2.Items.Add(s);
                 s= "";
                }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {//发送
            if (message != "")
            {
                listBox2.Text = message;
            }
           // string message =listBox1.Text;
           //byte[] send= Encoding.ASCII.GetBytes(message);
           // socketClinet.Send(send);
        }
    }
}
