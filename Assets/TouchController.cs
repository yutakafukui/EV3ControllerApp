using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;               // add
using System.Text;
using UnityEngine;


public class TouchController : MonoBehaviour
{
    public string serverAddress = "192.168.0.101";
    public int port = 8001;
    public SpriteRenderer touchPoint = null;

    private TcpClient tcpClient;
    private NetworkStream networkStream;

    // Start is called before the first frame update
    void Start()
    {
        tcpClient = new TcpClient(serverAddress, port);
        networkStream = tcpClient?.GetStream();
        Debug.Log("Connected");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                var pos = touch.position;
                Debug.Log(pos);

                // スクリーン座標からワールド座標
                var screenPos = new Vector3(pos.x, pos.y, 5f);
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                touchPoint.transform.position = worldPos;

                // ロボットへ送信
                var buffer = Encoding.UTF8.GetBytes("move");
                networkStream?.Write(buffer, 0, buffer.Length);
                Debug.Log("move");

            }
        } else if (Input.touchCount == 2)
        {
            var buffer = Encoding.UTF8.GetBytes("exit");
            networkStream?.Write(buffer, 0, buffer.Length);
            Debug.Log("exit");
            tcpClient?.Close();
            networkStream?.Close();
            Debug.Log("Disconnected");
        }

    }

    private void OnDestroy()
    {
        tcpClient?.Close();
        networkStream?.Close();
        Debug.Log("Disconnected");
    }
}
