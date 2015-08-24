++#UnitySocket开源框架

*一款非常小巧的适用于Unity开发中使用Socket技术时的框架*

**框架要求:**
*.Net 2.0以上*

**Unity版本要求**
*Unity 4.6.3以上*

**开发语言**
*C#*

**使用到的系统库**
```
using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
```

**类结构**
- `LOServerSocket`
	- 创建服务器端消息收发对象
- `LOClientSocket`
	- 创建客户端消息收发对象

**委托类型**

- `LOServerSocket.cs`
```
//服务器收到新的链接请求后的回调函数类型		
public delegate void ServerAcceptDelegate(LOClientSocket newClient);
//服务器收到新的消息后的回调函数类型
public delegate void ServerReceiveDelegate(LOClientSocket sender,string message);
//服务器发送消息后的回调函数类型
public delegate void ServerSendDelegate(LOClientSocket sender,bool isSended);
```

- `LOClientSocket.cs`
```
//客户端链接服务器后的回调函数类型
public delegate void ClientConnectDelegate(bool isConnected);
//客户端收到消息后的回调函数类型
public delegate void ClientReceiveDelegate(string message);
//客户端发送消息后的回调函数类型
public delegate void ClientSendDelegate(bool isSended);
```

##**使用举例**

###服务器端设置举例
- 创建服务器
```
//获取服务器socket对象
LOServerSocket server = new LOServerSocket("127.0.0.1",12566,40);
```
- 开启服务器
```
//开启服务器
server.StartServer ((LOClientSocket newClient) => {
	Debug.Log(newClient.IP + " is Coming!!");
});
```
- 设置收到消息功能
```
//收到消息
server.ReceiveBlock = ((LOClientSocket sender, string message) => {
	Debug.Log(sender.IP + " : " + message);
});
```
- 设置发送消息的功能
```
//设置发送消息的功能
server.SendBlock = ((LOClientSocket sender, bool isSended) => {
	Debug.Log(this.server.IP + " : " + isSended.ToString());
});
```
- 发送消息给一个客户端
```
//发送消息给一个客户端  例:127.0.0.1
server.SendMessage ("127.0.0.1", Random.Range (1000, 9999).ToString());
```
* * *
客户端设置举例
- 创建客户端
```
//获取客户端socket对象
LOClientSocket client = new LOClientSocket("127.0.0.1",12566);
```
- 设置接收消息的功能
```
//设置接收消息的功能,并且开始接收消息
client.BeginReceive(((string message) => {
	Debug.Log("received : " + message);	
}));
```
- 设置发送消息的功能
```
//设置发送消息的功能,并且发送消息
client.SendMessage(Random.Range(100,999).ToString(),((bool isSended) => {
	Debug.Log(this.client.IP + " : " + isSended.ToString());
}));
```

**最后，真诚的希望能够提出宝贵意见给我~**
**联系方式:**
*E-mail:applexiaohao@gmail.com*
*QQ:632138357*
