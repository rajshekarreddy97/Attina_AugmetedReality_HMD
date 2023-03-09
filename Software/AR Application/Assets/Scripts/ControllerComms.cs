using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Events;

public class ControllerComms : MonoBehaviour
{
    [SerializeField]
	private int port = 1999; 
    [SerializeField]
    private string controllerIPAddress;
    private Thread receiveThread;
	private UdpClient client;

    private void Start()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
	{
		client = new UdpClient(port);
		IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse(controllerIPAddress), 0);

        Debug.Log("Listening to Data");
    
        
		while (true)
		{
			byte[] data = client.Receive(ref anyIP);
			string message = Encoding.UTF8.GetString(data);

            Debug.Log(message);

            ControllerManager.Instance.ReadIMU(message);
		}
	}

    private void OnApplicationQuit() 
    {
        Dispose();
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