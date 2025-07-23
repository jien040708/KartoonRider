using UnityEngine;
using Photon.Pun;

public class FinishLineTrigger : MonoBehaviour
{
    private bool hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 결승선을 통과했는지 확인
        if (other.CompareTag("Player") && !hasFinished)
        {
            hasFinished = true;
            
            // 포톤 RPC로 결승선 통과 메시지 전송
            if (PhotonRoomManager.Instance != null)
            {
                PhotonRoomManager.Instance.SendPlayerFinished();
            }
            else
            {
                Debug.LogWarning("PhotonRoomManager.Instance가 null입니다!");
            }
            Debug.Log("🏆 플레이어가 결승선을 통과했습니다!");
        }
    }
    

} 