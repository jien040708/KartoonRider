using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    public int totalCheckpoints = 3;
    public int totalLaps = 1; 

    private Dictionary<GameObject, int> playerCheckpointIndex = new();
    private Dictionary<GameObject, int> playerLapCount = new();

    void Awake()
    {
        instance = this;
    }

    public void PlayerReachedCheckpoint(GameObject player, int checkpointIndex)
    {
        if (!playerCheckpointIndex.ContainsKey(player))
        {
            playerCheckpointIndex[player] = 0;
            playerLapCount[player] = 0;
        }

        int currentIndex = playerCheckpointIndex[player];

        // 올바른 순서일 때만 통과 인정
        if (checkpointIndex == (currentIndex + 1) % totalCheckpoints)
        {
            playerCheckpointIndex[player] = checkpointIndex;

            // 마지막 체크포인트 → 랩 증가
            if (checkpointIndex == totalCheckpoints - 1)
            {
                playerCheckpointIndex[player] = -1;
                playerLapCount[player]++;
                Debug.Log($"{player.name} 랩 완료! 현재 {playerLapCount[player]}랩");

                if (playerLapCount[player] >= totalLaps)
                {
                    Debug.Log($"{player.name} 결승선 도착!");
                    // TODO: 순위 시스템 연결
                }
            }
        }
    }
}
