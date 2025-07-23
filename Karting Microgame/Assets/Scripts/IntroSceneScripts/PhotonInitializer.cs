using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonInitializer : MonoBehaviourPunCallbacks
{
    public static PhotonInitializer Instance { get; private set; }

    public bool IsReady { get; private set; } = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject); // 씬 전환에도 유지
    }

    void Start()
    {
        ConnectToPhotonServer();
    }

    public void ConnectToPhotonServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            Debug.Log("🔌 Photon 서버 연결 시도");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("✅ Photon 마스터 서버에 연결됨");
        PhotonNetwork.JoinLobby(); // 로비 입장
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("✅ Photon 로비 입장 완료");
        IsReady = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"❌ Photon 연결 끊김: {cause}");
        IsReady = false;
    }
}
