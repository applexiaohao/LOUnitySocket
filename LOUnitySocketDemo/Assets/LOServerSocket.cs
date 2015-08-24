using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace LOSocket
{
	public class LOServerSocket
	{
		//创建服务器Socket对象
		private Socket socket = null;
		private string ip;
		private int backlog;

		public string IP{get{ return ip;}}
		/// <summary>
		/// 构造器..创建服务器Socket对象
		/// </summary>
		public LOServerSocket (string ip,int port,int backlog)
		{
			//1、使用什么网络
			AddressFamily v4 = AddressFamily.InterNetwork;
			//2、使用什么方式
			SocketType type = SocketType.Stream;
			//3、使用什么协议
			ProtocolType pt = ProtocolType.Tcp;

			//创建服务器Socket对象
			this.socket = new Socket(v4,type,pt);

			//创建服务器节点
			IPAddress ia = IPAddress.Parse("127.0.0.1");
			//自定义的端口10566
			IPEndPoint ep = new IPEndPoint (ia, port);

			//绑定服务器节点
			this.socket.Bind(ep);

			this.ip = ip;
			this.backlog = backlog;
		}


		private List<LOClientSocket> client_list;

		public void StartServer(ServerAcceptDelegate block)
		{
			this.accept_block = block;
			//开始侦听链接请求
			this.socket.Listen(this.backlog);

			//接受客户端请求...
			//创建委托类型的回调函数变量
			System.AsyncCallback acb = new System.AsyncCallback(ServerAccept);

			//开始等待接受客户端请求
			this.socket.BeginAccept(acb,this.socket);

			this.client_list = new List<LOClientSocket> ();
		}
		/// <summary>
		/// 接受一个客户端的链接请求，只要有一个客户端请求,就会回调该函数.
		/// </summary>
		void ServerAccept(System.IAsyncResult ar)
		{
			//将服务器Socket对象转换出来.
			Socket server = ar.AsyncState as Socket;

			//请求结束后在服务器端产生的客户端对象
			//每一个客户端请求,服务器上会产生一个对应的客户端对象
			Socket client = server.EndAccept (ar);

			//创建
			LOClientSocket cs = new LOClientSocket (client);


			this.client_list.Add (cs);

			this.accept_block (cs);

			cs.BeginReceive ((string content) => {
				this.ReceiveBlock(cs,content);
			});

			//开始等待接受客户端请求
			server.BeginAccept(new System.AsyncCallback(ServerAccept),server);
		}

		#region 委托类型声明区域
		public delegate void ServerAcceptDelegate(LOClientSocket newClient);
		public delegate void ServerReceiveDelegate(LOClientSocket sender,string message);
		public delegate void ServerSendDelegate(LOClientSocket sender,bool isSended);

		private ServerAcceptDelegate accept_block;
		public ServerReceiveDelegate ReceiveBlock{set;get;}
		public ServerSendDelegate SendBlock{set;get;}
		#endregion


		#region 发送消息区域

		public void SendMessage(string ip,string message)
		{
			LOClientSocket client = this.client_list.Find(((LOClientSocket obj) => {
				return obj.IP.Equals(ip);
			}));

			client.SendMessage (message, (bool isSended) => {
				this.SendBlock(client,isSended);
			});
		}

		#endregion
	}
}

