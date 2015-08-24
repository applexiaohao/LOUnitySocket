using System;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace LOSocket
{
	public class LOClientSocket
	{
		private Socket 	Client{ set; get;}
		private byte[]	ByteBuffer;
		public string	IP{ set; get;}

		#region 客户端的消息发送对象
		/// <summary>
		/// 客户端的消息收发对象
		/// </summary>
		public LOClientSocket (string ip,int port)
		{
			//创建客户端Socket
			AddressFamily af = AddressFamily.InterNetwork;
			SocketType st = SocketType.Stream;
			ProtocolType pt = ProtocolType.Tcp;
			this.Client = new Socket (af, st, pt);

			//创建服务器节点
			IPAddress ia = IPAddress.Parse (ip);
			IPEndPoint ep = new IPEndPoint (ia, port);

			//开始链接服务器节点
			//1、服务器节点
			//2、异步链接成功后的回调函数
			System.AsyncCallback acb = new System.AsyncCallback(ClientConnectedToServer);
			//3、传递的参数

			//开始链接服务器节点
			this.Client.BeginConnect(ep,acb,this.Client);
			this.ByteBuffer = new byte[1024];
			this.IP = ip;
		}

		void ClientConnectedToServer(IAsyncResult ar)
		{
			//获取能够代表对象链接的Socket
			Socket state = ar.AsyncState as Socket;

			if (this.ConnectBlock != null) 
			{
				this.ConnectBlock (state.Connected);
			}
			//判断state对象的链接状态
			if (state.Connected) 
			{
				this.BeginReceive (this.RecieveBlock);
			} 
		}
		#endregion

		#region 服务器的消息发送对象

		/// <summary>
		/// 服务器的消息收发对象
		/// </summary>
		/// <param name="sender">Sender.</param>
		public LOClientSocket (Socket sender)
		{
			this.Client = sender;
			this.IP = ((IPEndPoint)sender.RemoteEndPoint).Address.ToString();
			this.ByteBuffer = new byte[1024];
		}

		#endregion


		#region  收发消息区域
		public string Text{
			get{
				return Encoding.Default.GetString (this.ByteBuffer);
			}
		}

		/// <summary>
		/// 开始接收消息指令
		/// </summary>
		public void BeginReceive(ClientReceiveDelegate block)
		{
			this.RecieveBlock = block;
			this.Client.BeginReceive (this.ByteBuffer, 0, this.ByteBuffer.Length, SocketFlags.None, new System.AsyncCallback (ReceiveCompleted), this.Client);

		}

		/// <summary>
		/// 收到消息后的处理
		/// </summary>
		/// <param name="ar">Ar.</param>
		private void ReceiveCompleted(IAsyncResult ar)
		{
			//获取正在与真正的客户端链接着的服务器客户端Socket对象
			Socket client = ar.AsyncState as Socket;

			//声明一个整型变量,用来存储收到了多少信息
			int byteRead = 0;

			//收到这些数量的数据...
			byteRead = client.EndReceive(ar);

			//证明有数据...
			if (byteRead > 0) 
			{
				//获取程序默认编码格式
				Encoding en = Encoding.Default;

				//将byte[]数据转换成字符串数据
				string content = en.GetString(this.ByteBuffer);
	
				//返回数据
				this.RecieveBlock(content);
			}

			//并不能因为收到了一次数据,而就不在接受数据了.
			this.BeginReceive(this.RecieveBlock);
		}

		/// <summary>
		/// 发送消息
		/// </summary>
		/// <param name="message">Message.</param>
		public void SendMessage(string message,ClientSendDelegate block)
		{
			this.SendBlock = block;

			System.AsyncCallback iar = new System.AsyncCallback (SendCompleted);
			byte[] mb = Encoding.Default.GetBytes (message);

			this.Client.BeginSend (mb, 0, mb.Length, SocketFlags.None, iar,this.Client);
		}

		/// <summary>
		/// 发送消息完成后的处理
		/// </summary>
		void SendCompleted(IAsyncResult ar)
		{
			Socket client = ar.AsyncState as Socket;
			client.EndSend (ar);
			this.SendBlock (true);
		}
		#endregion


		#region 委托类型声明区域
		public delegate void ClientConnectDelegate(bool isConnected);
		public delegate void ClientReceiveDelegate(string message);
		public delegate void ClientSendDelegate(bool isSended);

		public ClientConnectDelegate ConnectBlock = null;
		private ClientReceiveDelegate RecieveBlock = null;
		private ClientSendDelegate    SendBlock = null;
		#endregion
	}
}

