using NativeWebSocket;
using UnityEngine;

public class RoomWebSocket : MonoBehaviour
{
    public static RoomWebSocket Instance;

    private WebSocket websocket;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async void Connect(string roomCode, string userId)
    {
        string url = $"wss://kartoonrider-production-b878.up.railway.app/ws/{roomCode}/{userId}";
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("웹소켓 연결됨!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string msg = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("수신 메시지: " + msg);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("웹소켓 연결 종료됨");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("웹소켓 에러: " + e);
        };

        await websocket.Connect();
    }

    public async void SendMessage(string msg)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(msg);
        }
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}
