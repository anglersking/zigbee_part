using Aardvark.Base;
using Segment.Model;
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

namespace Zigbee_part
{
    public partial class Form1 : Form
    {
      

        public Form1()
        {
            InitializeComponent();
        }
        System.Threading.Thread threadWatch = null; //负责监听客户端的线程
        Socket socketWatch = null; //负责监听客户端的套接字
                                   //创建一个负责和客户端通信的套接字 
        List<Socket> socConnections = new List<Socket>();
        List<Thread> dictThread = new List<Thread>();
        Random r = new Random();
        int i = 0;
        double num = 0.0;
        int stop = 0;



        /// <summary>
        /// 监听客户端发来的请求
        /// </summary>
        private void WatchConnecting()
        {
            while (true)  //持续不断监听客户端发来的请求
            {
                Socket socConnection = socketWatch.Accept();
                //txtMsg.AppendText("客户端连接成功" + "\r\n");
                //创建一个通信线程 
                ParameterizedThreadStart pts = new ParameterizedThreadStart(ServerRecMsg);
                Thread thr = new Thread(pts);
                thr.IsBackground = true;
                socConnections.Add(socConnection);
                //启动线程
                thr.Start(socConnection);
                dictThread.Add(thr);
            }
        }
        private void ServerRecMsg(object socketClientPara)
        {
            Socket socketServer = socketClientPara as Socket; //类型转换 objec->Socket
            while (true)
            {
                //创建一个内存缓冲区 其大小为1024*1024字节  即1M
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                try
                {
                    //将接收到的信息存入到内存缓冲区,并返回其字节数组的长度
                    int length = socketServer.Receive(arrServerRecMsg);
                    //将机器接受到的字节数组转换为人可以读懂的字符串
                    string strSRecMsg = Encoding.UTF8.GetString(arrServerRecMsg, 0, length);
                    Console.WriteLine(length);
                    if (strSRecMsg.Length != 0)
                    {
                        //将发送的字符串信息附加到文本框txtMsg上   客户端IP 时间  消息

                        //  txtMsg.AppendText(socketServer.RemoteEndPoint.ToString() + "客户端 " + GetCurrentTime() + "\r\n" + strSRecMsg + "\r\n");

                        num = ((float)r.NextDouble());

                        num = r.NextDouble();
                        var dt = DateTime.Now;//ToString("yyyy/MM/dd HH:mm:ss.ffff");


                        dt = dt.AddMilliseconds(1.0);


                        chart1.Series[0].Points.AddXY(dt.ToString("yyyy/MM/dd HH:mm:ss:fff"), 25 + num);
                        i++;

                        chart1.Series[0].BorderWidth = 5;
                        chart1.Series[0].Color = Color.Blue;
                        chart1.ChartAreas[0].AxisX.Title = "Time/ms";
                        chart1.ChartAreas[0].AxisY.Title = "光强/%";

                        lb_light.Text = String.Format("{0:F}", (25 + strSRecMsg[8]+ strSRecMsg[9])) + "%";
                         if ((float)(strSRecMsg[1]+ strSRecMsg[2]) <= 5)
                         {
                             label1.Text = "有人";


                         }
                         else 
                         {
                             label1.Text = "无人";


                         }

                        this.dataGridView1.Rows.Insert(0, dt.ToString("yyyy/MM/dd HH:mm:ss:fff"), lb_light.Text, label1.Text, label2.Text);

                        System.Console.WriteLine(strSRecMsg);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("错误：" + ex.ToString());
                   this.label1.Text=socketServer.RemoteEndPoint.ToString() + "客户端已断开连接！" + "\r\n";

                    // 从 通信套接字 集合中删除被中断连接的通信套接字；
                  //  Dict.Remove(socketServer.RemoteEndPoint.ToString());
                    // 从通信线程集合中删除被中断连接的通信线程对象；
                //    dictThread.Remove(socketServer.RemoteEndPoint.ToString());
                    // 从列表中移除被中断的连接IP
          //lb_ipOnline.Items.Remove(socketServer.RemoteEndPoint.ToString());

                    break;
                }
            }
        }



        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            //调用 ServerSendMsg方法  发送信息到客户端
           // ServerSendMsg(txtSendMsg.Text.Trim());
        }

        /// <summary>
        /// 发送信息到客户端的方法
        /// </summary>
        /// <param name="sendMsg">发送的字符串信息</param>
        private void ServerSendMsg(string sendMsg)
        {
            //将输入的字符串转换成 机器可以识别的字节数组
            byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendMsg);
            //向客户端发送字节数组信息
            foreach (Socket socConnection in socConnections)
            {
                socConnection.Send(arrSendMsg);
            }

            //将发送的字符串信息附加到文本框txtMsg上
           // txtMsg.AppendText("So-flash:" + GetCurrentTime() + "\r\n" + sendMsg + "\r\n");
            //}

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
           int i=0





            if (i == 2000)
            {
                timer1.Stop();
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

            dataGridView1.Columns[0].HeaderText = "时间";
            dataGridView1.Columns[1].HeaderText = "光照强度";
            dataGridView1.Columns[2].HeaderText = "是否有人";
            dataGridView1.Columns[3].HeaderText = "灯是否打开";
            // this.dataGridView1.Columns.Add("0","时间", "光照强度", "是否有人", "灯组状态");
            // this.dataGridView1.Rows.Add("时间", "光照强度", "是否有人","灯组状态");
            //定义一个套接字用于监听客户端发来的信息  包含3个参数(IP4寻址协议,流式连接,TCP协议)
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //服务端发送信息 需要1个IP地址和端口号
            IPAddress ipaddress = IPAddress.Parse("127.0.0.1"); //获取文本框输入的IP地址
            //将IP地址和端口号绑定到网络节点endpoint上 
            IPEndPoint endpoint = new IPEndPoint(ipaddress, int.Parse("12345")); //获取文本框上输入的端口号
            //监听绑定的网络节点
            socketWatch.Bind(endpoint);
            //将套接字的监听队列长度限制为20
            socketWatch.Listen(20);
            //创建一个监听线程 
            threadWatch = new Thread(WatchConnecting);
            //将窗体线程设置为与后台同步
            threadWatch.IsBackground = true;
            //启动线程
            threadWatch.Start();
            //启动线程后 txtMsg文本框显示相应提示

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (label2.Text == "关闭")
            {
                label2.Text = "开启";
                ServerSendMsg("open");

            }
            else
            {
                label2.Text = "关闭";
                ServerSendMsg("off");
            }
        }
    }
}
