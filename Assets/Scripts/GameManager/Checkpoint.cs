using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;  // 몇 번째 체크포인트인지

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player passed Checkpoint " + checkpointIndex);
            CheckpointManager.instance.PlayerReachedCheckpoint(other.gameObject, checkpointIndex);
        }
    }
}
