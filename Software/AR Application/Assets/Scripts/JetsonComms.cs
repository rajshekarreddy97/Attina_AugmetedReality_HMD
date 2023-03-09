using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Events;

public class JetsonComms : MonoBehaviour
{
    public static JetsonComms Instance;

    [SerializeField]
	private int port = 8000; 
    [SerializeField]
    private string jetsonIPAddress;
    private Thread receiveThread;
	private UdpClient client;

    private static int Send_localPort;

    IPEndPoint remoteEndPoint;
    UdpClient Send_client;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(jetsonIPAddress), port);
        Send_client = new UdpClient();

        receiveThread.Start();
    }

    private void ReceiveData()
	{
		client = new UdpClient(port);
		IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse(jetsonIPAddress), 0);

        Debug.Log("Listening to Data");
    
        
		while (true)
		{
			byte[] data = client.Receive(ref anyIP);
			string message = Encoding.UTF8.GetString(data);

            Debug.Log(message);
		}
	}

    private void OnApplicationQuit() 
    {
        Dispose();
    }

    public void SendMessage(string message)
    {
        StartCoroutine(SendMsg(message));
    }

    public IEnumerator SendMsg(string strMessage)
    {
        byte[] data = Encoding.UTF8.GetBytes(strMessage);

        var message = client.Send(data, data.Length, remoteEndPoint);
        yield return message;
    }

    public void Dispose()
    {
        if (receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }
        if (client != null)
        {
            client.Close();
            client = null;
        }
    }
}