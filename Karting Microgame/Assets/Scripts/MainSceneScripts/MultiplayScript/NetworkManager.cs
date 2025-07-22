using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;

public class NetworkManager : MonoBehaviour
{
    public GameObject kartPrefab;    // KartClassic_MLAgent 프리팹
    public GameObject myKart;        // 내가 조작하는 카트 오브젝트
    public string myUserId; // 로그인 되어있는 유저 login_id

    private WebSocket ws;
    private Dictionary<string, GameObject> players = new();
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject); // ✅ 씬 전환 시 살아남게
    }

    void Start()
    {
        ws = new WebSocket("ws://your-server/ws/ROOM123/" + myUserId);  // 주소는 실제 서버로 교체
        ws.OnMessage += OnMessageReceived;
        ws.Connect();
    }

    void Update()
    {
        if (ws == null || myKart == null) return;

        if (Time.frameCount % 5 == 0) // 12fps 주기로 전송
        {
            Vector3 pos = myKart.transform.position;
            float rotY = myKart.transform.eulerAngles.y;

            PlayerState myState = new PlayerState
            {
                user_id = myUserId,
                x = pos.x,
                y = pos.y,
                z = pos.z,
                rotationY = rotY
            };

            string json = JsonUtility.ToJson(myState);
            ws.Send(json);
        }
    }

    void OnMessageReceived(object sender, MessageEventArgs e)
    {
        PlayerState state = JsonUtility.FromJson<PlayerState>(e.Data);
        if (state.user_id == myUserId) return;

        if (!players.ContainsKey(state.user_id))
        {
            GameObject newKart = Instantiate(kartPrefab);
            players[state.user_id] = newKart;
        }

        GameObject player = players[state.user_id];
        RemotePlayerController controller = player.GetComponent<RemotePlayerController>();
        if (controller != null)
        {
            Vector3 pos = new Vector3(state.x, state.y, state.z);
            Quaternion rot = Quaternion.Euler(0, state.rotationY, 0);
            controller.SetTarget(pos, rot);
        }
    }
}

[System.Serializable]
public class PlayerState
{
    public string user_id;
    public float x, y, z;
    public float rotationY;
}
