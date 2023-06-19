using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;               // add
using System.Text;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    public string serverAddress = "192.168.0.101";  // EV3��IP�A�h���X
    public int port = 8001;
    public SpriteRenderer touchPoint = null;

    private TcpClient tcpClient;
    private NetworkStream networkStream;

    public Vector2 startPos;

    void Start()
    {
        //tcpClient = new TcpClient(serverAddress, port);
        //networkStream = tcpClient?.GetStream();
        //Debug.Log("Connected");
    }

    void Update()
    {
        if (Input.touchCount == 1)  // �V���O���^�b�v1�_
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    break;
                case TouchPhase.Moved:
                    var pos = touch.position;
                    // �X�N���[�����W���烏�[���h���W
                    var screenPos = new Vector3(pos.x, pos.y, 5f);
                    var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                    touchPoint.transform.position = worldPos;

                    // ���{�b�g�֑��M
                    //var buffer = Encoding.UTF8.GetBytes("MOVE");
                    //networkStream?.Write(buffer, 0, buffer.Length);
                    //Debug.Log("MOVE");

                    break;
                case TouchPhase.Ended:
                    break;
            }

            //if (touch.phase == TouchPhase.Began)
            //{
            //    var pos = touch.position;
            //    Debug.Log(pos);

            //    // �X�N���[�����W���烏�[���h���W
            //    var screenPos = new Vector3(pos.x, pos.y, 5f);
            //    var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            //    touchPoint.transform.position = worldPos;

            //    // ���{�b�g�֑��M
            //    var buffer = Encoding.UTF8.GetBytes("MOVE");
            //    networkStream?.Write(buffer, 0, buffer.Length);
            //    Debug.Log("MOVE");

            //}
        }
        else if (Input.touchCount == 2) // �}���`�^�b�v2�_
        {
            // �\�P�b�g�ʐM���J�n����
            tcpClient = new TcpClient(serverAddress, port);
            networkStream = tcpClient?.GetStream();
            Debug.Log("Connected");
        }
        else if (Input.touchCount == 3) // �}���`�^�b�v3�_
        {
            // �I������
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
