using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;  // 몇 번째 체크포인트인지

    private void OnTriggerEnter(Collider other)
    {
        // Player 태그가 있는지 확인
        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player '{other.name}' passed Checkpoint {checkpointIndex}");
            CheckpointManager.instance.PlayerReachedCheckpoint(other.gameObject, checkpointIndex);
        }
        else
        {
            // 디버그용: Player 태그가 없는 객체도 로그로 확인
            Debug.Log($"Non-player object '{other.name}' entered checkpoint {checkpointIndex}");
        }
    }
}
