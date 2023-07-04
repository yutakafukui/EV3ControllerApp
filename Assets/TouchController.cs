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
        // �\�P�b�g�ʐM�J�n
        tcpClient = new TcpClient(serverAddress, port);
        networkStream = tcpClient?.GetStream();
        Debug.Log("Connected");
    }

    // �^�b�`�J�n�_�ƁA�^�b�`�I���_��ێ�����ϐ���錾
    private Vector2 startPos = new Vector2(0, 0);
    private Vector2 endPos = new Vector2(0, 0);

    // �X���C�v���̌o�ߎ���
    private float elapsedTime = 0;

    // �X���C�v�̋O�Ղ̍��W
    private List<Vector2> positions = new List<Vector2>();

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)    // �X���C�v�̍ŏ�
            {
                startPos = touch.position;
                Debug.Log("Began: " + startPos);

                // �X���C�v�ŏ��̍��W�̕ێ�
                positions.Clear();
                positions.Add(startPos);

                // �^�b�`�����ʒu�ɉ~��`��
                var screenPos = new Vector3(startPos.x, startPos.y, 5f);
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                touchPoint.transform.position = worldPos;

            } else if (touch.phase == TouchPhase.Moved) // �X���C�v�̓r��
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

                    // �X���C�v�r���̍��W�̕ێ��i��t�j
                    positions.Add(pos);
                    elapsedTime = 0;
                }

            }
            else if (touch.phase == TouchPhase.Ended)   // �X���C�v�̍Ō�
            {
                // �X���C�v�Ō�̍��W�̕ێ�
                endPos = touch.position;
                positions.Add(endPos);
                Debug.Log("Ended: " + endPos);
                Debug.Log("Vector: " + (endPos - startPos));

                var screenPos = new Vector3(endPos.x, endPos.y, 5f);
                var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
                touchPoint.transform.position = worldPos;

                // �X���C�v�̋O�Ղ̍��W���J���}�ŋ�؂�A������Ɋi�[
                var s = "";
                foreach(var pos in positions)
                {
                    s += string.Format("{0:f2},{1:f2},", pos.x, pos.y);
                }
                s = s.Trim(',');
                Debug.Log("possitions: " + s);

                // ���{�b�g�֑��M
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
