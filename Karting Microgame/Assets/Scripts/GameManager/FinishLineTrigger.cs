using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    private bool hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 결승선을 통과했는지 확인
        if (other.CompareTag("Player") && !hasFinished)
        {
            hasFinished = true;
            
            // 멀티플레이어 결승선 통과 메시지 전송
            PhotonRoomManager.Instance.SendPlayerFinished();
            Debug.Log("🏆 플레이어가 결승선을 통과했습니다!");
        }
    }
} 