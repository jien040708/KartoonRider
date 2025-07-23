using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class PhotonTransformSetup : MonoBehaviour
{
    [Header("플레이어 카트들")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    
    void Start()
    {
        // Inspector에서 연결되지 않은 경우 자동으로 찾기
        if (player1 == null) player1 = GameObject.Find("Player_1");
        if (player2 == null) player2 = GameObject.Find("Player_2");
        if (player3 == null) player3 = GameObject.Find("Player_3");
        if (player4 == null) player4 = GameObject.Find("Player_4");
        
        // 각 플레이어 카트에 PhotonView와 PhotonTransformView 추가
        SetupPhotonComponents(player1, "Player_1");
        SetupPhotonComponents(player2, "Player_2");
        SetupPhotonComponents(player3, "Player_3");
        SetupPhotonComponents(player4, "Player_4");
    }
    
    void SetupPhotonComponents(GameObject playerKart, string playerName)
    {
        if (playerKart == null)
        {
            Debug.LogError($"{playerName}을 찾을 수 없습니다!");
            return;
        }
        
        // PhotonView 추가
        PhotonView photonView = playerKart.GetComponent<PhotonView>();
        if (photonView == null)
        {
            photonView = playerKart.AddComponent<PhotonView>();
            Debug.Log($"{playerName}에 PhotonView 추가됨");
        }
        
        // PhotonTransformView 추가
        PhotonTransformView transformView = playerKart.GetComponent<PhotonTransformView>();
        if (transformView == null)
        {
            transformView = playerKart.AddComponent<PhotonTransformView>();
            transformView.m_SynchronizePosition = true;
            transformView.m_SynchronizeRotation = true;
            transformView.m_SynchronizeScale = false;
            Debug.Log($"{playerName}에 PhotonTransformView 추가됨");
        }
        
        // PhotonView 설정
        photonView.ObservedComponents.Add(transformView);
        
        Debug.Log($"{playerName} 포톤 컴포넌트 설정 완료");
    }
} 