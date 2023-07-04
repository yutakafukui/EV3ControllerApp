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
        // ソケット通信開始
        tcpClient = new TcpClient(serverAddress, port);
        networkStream = tcpClient?.GetStream();
        Debug.Log("Connected");
    }

    // タッチ開始点と、タッチ終了点を保持する変数を宣言
    private Vector2 startPos = new Vector2(0, 0);
    private Vector2 endPos = new Vector2(0, 0);

    // スワイプ中の経過時間
    private float elapsedTime = 0;

    // スワイプの軌跡の座標
    private List<Vector2> positions = new List<Vector2>();

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)    // スワイプの最初
            {
                startPos = touch.position;
                Debug.Log("Began: " + startPos);

                // スワイプ最初の座標の保持
                positions.Clear();
                positions.Add(startPos);

                // タッチした位置に円を描く
                var screenPos = new Vector3(startPos.x, startPos.y, 5f);
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                touchPoint.transform.position = worldPos;

            } else if (touch.phase == TouchPhase.Moved) // スワイプの途中
            {
                var pos = touch.position;
                // Debug.Log("Moved: " + pos);
                // Debug.Log("Delt: " + deltTime);

                if (elapsedTime > 0.2)
                {
                    var screenPos = new Vector3(pos.x, pos.y, 5f);
                    var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                    touchPoint.transform.position = worldPos;
                    Debug.Log("Moved:" + pos);

                    // スワイプ途中の座標の保持（Δt）
                    positions.Add(pos);
                    elapsedTime = 0;
                }

            }
            else if (touch.phase == TouchPhase.Ended)   // スワイプの最後
            {
                // スワイプ最後の座標の保持
                endPos = touch.position;
                positions.Add(endPos);
                Debug.Log("Ended: " + endPos);
                Debug.Log("Vector: " + (endPos - startPos));

                var screenPos = new Vector3(endPos.x, endPos.y, 5f);
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                touchPoint.transform.position = worldPos;

                // スワイプの軌跡の座標をカンマで区切り、文字列に格納
                var s = "";
                foreach(var pos in positions)
                {
                    s += string.Format("{0:f2},{1:f2},", pos.x, pos.y);
                }
                s = s.Trim(',');
                Debug.Log("possitions: " + s);

                // ロボットへ送信
                var buffer = Encoding.UTF8.GetBytes(s);
                networkStream?.Write(buffer, 0, buffer.Length);

            }

            elapsedTime += Time.deltaTime;

        }
        else if (Input.touchCount == 2)
        {
            //var buffer = Encoding.UTF8.GetBytes("exit");
            //networkStream?.Write(buffer, 0, buffer.Length);
            //Debug.Log("exit");
            //tcpClient?.Close();
            //networkStream?.Close();
            //Debug.Log("Disconnected");
        }

    }

    private void OnDestroy()
    {
        tcpClient?.Close();
        networkStream?.Close();
        Debug.Log("Disconnected");
    }
}
