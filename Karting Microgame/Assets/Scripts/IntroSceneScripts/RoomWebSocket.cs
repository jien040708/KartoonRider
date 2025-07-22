using NativeWebSocket;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RoomWebSocket : MonoBehaviour
{
    public static RoomWebSocket Instance;

    private WebSocket websocket;
    private bool gameStarted = false; // 게임 시작 상태 추적
    private string currentScene; // 현재 씬 추적
    private int myPlayerId = -1; // 내 플레이어 ID

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
                StartCoroutine(HandleRoomDeletedUI());
                _ = websocket.Close();
                return;
            }

            // 플레이어 ID 할당 메시지 처리
            if (msg.StartsWith("__PLAYER_ID__:"))
            {
                string[] parts = msg.Split(':');
                if (parts.Length == 2 && int.TryParse(parts[1], out int playerId))
                {
                    myPlayerId = playerId;
                    Debug.Log($"🎯 내 플레이어 ID 할당됨: {playerId}");
                    
                    // MultiplayerPlayerManager에 플레이어 ID 전달
                    var playerManager = FindObjectOfType<MultiplayerPlayerManager>();
                    if (playerManager != null)
                    {
                        playerManager.SetCurrentPlayerId(playerId);
                    }
                }
                return;
            }

            // 게임 시작 메시지 처리
            if (msg == "__GAME_START__")
            {
                Debug.Log("🎮 게임 시작 메시지 수신!");
                StartCoroutine(StartGame());
                return;
            }

            // MainScene에서의 멀티플레이어 메시지 처리
            currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "MainScene")
            {
                HandleMultiplayerMessage(msg);
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
                        
                        // 4명이 모이면 게임 시작
                        if (count >= 4 && !gameStarted)
                        {
                            Debug.Log("🎯 4명이 모였습니다! 게임을 시작합니다.");
                            gameStarted = true;
                            StartCoroutine(StartGameWithDelay());
                        }
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

    private void HandleMultiplayerMessage(string msg)
    {
        // 플레이어 위치 업데이트
        if (msg.StartsWith("__PLAYER_POSITION__:"))
        {
            HandlePlayerPosition(msg);
        }
        // 플레이어 결승선 통과
        else if (msg.StartsWith("__PLAYER_FINISH__:"))
        {
            string[] parts = msg.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1], out int playerId))
            {
                Debug.Log($"🏆 플레이어 {playerId}가 결승선을 통과했습니다!");
                // 순위 표시 등의 로직
            }
        }
        
        // 게임 종료
        else if (msg == "__GAME_END__")
        {
            Debug.Log("🏁 게임이 종료되었습니다.");
            // 로비로 돌아가기
            StartCoroutine(ReturnToLobbyCoroutine());
        }
    }
    
    private void HandlePlayerPosition(string msg)
    {
        // __PLAYER_POSITION__:playerId:posX,posY,posZ:rotX,rotY,rotZ,rotW
        string[] parts = msg.Split(':');
        if (parts.Length == 4)
        {
            if (int.TryParse(parts[1], out int playerId))
            {
                // 위치 파싱
                string[] posParts = parts[2].Split(',');
                if (posParts.Length == 3 && 
                    float.TryParse(posParts[0], out float x) &&
                    float.TryParse(posParts[1], out float y) &&
                    float.TryParse(posParts[2], out float z))
                {
                    Vector3 position = new Vector3(x, y, z);
                    
                    // 회전 파싱
                    string[] rotParts = parts[3].Split(',');
                    if (rotParts.Length == 4 &&
                        float.TryParse(rotParts[0], out float rotX) &&
                        float.TryParse(rotParts[1], out float rotY) &&
                        float.TryParse(rotParts[2], out float rotZ) &&
                        float.TryParse(rotParts[3], out float rotW))
                    {
                        Quaternion rotation = new Quaternion(rotX, rotY, rotZ, rotW);
                        
                        // MultiplayerPlayerManager에 위치 업데이트 요청
                        var playerManager = FindObjectOfType<MultiplayerPlayerManager>();
                        if (playerManager != null)
                        {
                            playerManager.UpdatePlayerPosition(playerId, position, rotation);
                        }
                    }
                }
            }
        }
    }

    public async void SendMessage(string msg)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(msg);
        }
    }
    
    public async void CloseConnection()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }

    private IEnumerator HandleRoomDeletedUI()
    {
        yield return null; // 다음 프레임까지 대기 (UI 접근 안전하게)

        RoomStatusUI ui = FindObjectOfType<RoomStatusUI>();
        if (ui != null)
        {
            ui.beforeJoinPanel.SetActive(true);
            ui.afterJoinPanel.SetActive(false);
        }
    }

    private IEnumerator StartGameWithDelay()
    {
        // 3초 대기 후 게임 시작 (모든 플레이어가 준비될 시간)
        yield return new WaitForSeconds(3f);
        
        // 모든 클라이언트에게 게임 시작 메시지 전송
        SendMessage("__GAME_START__");
        
        // 현재 클라이언트도 게임 시작
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        Debug.Log("🚀 MainScene으로 이동합니다...");
        
        // UI 업데이트 (게임 시작 중 메시지)
        RoomStatusUI ui = FindObjectOfType<RoomStatusUI>();
        if (ui != null && ui.statusText != null)
        {
            ui.statusText.text = "게임 시작 중...";
        }
        
        // 잠시 대기 후 씬 전환
        yield return new WaitForSeconds(1f);
        
        // MainScene으로 이동
        SceneManager.LoadScene("MainScene");
    }

    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
                websocket?.DispatchMessageQueue();
        #endif
    }

    private IEnumerator ReturnToLobbyCoroutine()
    {
        Debug.Log("🏠 로비로 돌아갑니다...");
        
        // 웹소켓 연결 종료
        if (websocket != null)
        {
            CloseConnection();
        }
        
        // IntroScene으로 이동
        SceneManager.LoadScene("IntroScene");
        yield return null;
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}


