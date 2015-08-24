using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Text;
using LOSocket;

public class TestScript : MonoBehaviour {

	//创建服务器Socket对象
	private LOServerSocket server = null;
	//创建客户端Socket对象
	private LOClientSocket client = null;


	void OnGUI()
	{
		if (GUILayout.Button("CreateServer")) 
		{
			//获取服务器socket对象
			this.server = new LOServerSocket("127.0.0.1",12566,40);

			//开始服务器..
			this.server.StartServer ((LOClientSocket newClient) => {
				Debug.Log(newClient.IP + " is Coming!!");
			});

			//收到消息
			this.server.ReceiveBlock = ((LOClientSocket sender, string message) => {
				Debug.Log(sender.IP + " : " + message);
			});

			//发送消息
			this.server.SendBlock = ((LOClientSocket sender, bool isSended) => {
				Debug.Log(this.server.IP + " : " + isSended.ToString());
			});
		}


		if (GUILayout.Button("ConnectServer")) 
		{
			//获取客户端socket对象
			this.client = new LOClientSocket("127.0.0.1",12566);

			this.client.BeginReceive(((string message) => {
				Debug.Log("received : " + message);	
			}));
		}

		if (GUILayout.Button("ServerSend")) {
			this.server.SendMessage ("127.0.0.1", Random.Range (1000, 9999).ToString());
		}

		if (GUILayout.Button("ClientSend")) {
			this.client.SendMessage(Random.Range(100,999).ToString(),((bool isSended) => {
				Debug.Log(this.client.IP + " : " + isSended.ToString());
			}));
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
