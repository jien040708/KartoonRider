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

            if (msg == "__ROOM_DELETED__")
            {
                Debug.LogWarning("⚠️ 방이 삭제되어 연결을 종료합니다.");
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    // UI 초기화
                    RoomStatusUI ui = FindObjectOfType<RoomStatusUI>();
                    if (ui != null)
                    {
                        ui.beforeJoinPanel.SetActive(true);
                        ui.afterJoinPanel.SetActive(false);
                    }
                });
                _ = websocket.Close();  // 비동기로 웹소켓 연결 끊기
                return;
            }


            // "현재 인원: " 뒤의 숫자 추출
            string marker = "현재 인원: ";
            int index = msg.IndexOf(marker);
            if (index != -1)
            {
                string countStr = msg.Substring(index + marker.Length).Trim();
                if (int.TryParse(countStr, out int count))
                {
                    RoomStatusUI ui = FindObjectOfType<RoomStatusUI>();
                    if (ui != null)
                    {   
                        Debug.Log($"📊 UI 찾음. 현재 인원: {count}");
                        ui.SetCurrentPlayers(count);
                    }
                    else
                    {
                        Debug.LogWarning("❌ RoomStatusUI 못 찾음");
                    }
                }
            }
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
