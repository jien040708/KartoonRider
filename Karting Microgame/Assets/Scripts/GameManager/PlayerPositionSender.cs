using UnityEngine;

public class PlayerPositionSender : MonoBehaviour
{
    public int playerId = 1;
    public float sendInterval = 0.1f; // 0.1초마다 전송
    
    private float lastSendTime;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    
    void Update()
    {
        // 일정 시간마다 위치 전송
        if (Time.time - lastSendTime >= sendInterval)
        {
            Vector3 currentPosition = transform.position;
            Quaternion currentRotation = transform.rotation;
            
            // 위치가 변경되었을 때만 전송
            if (Vector3.Distance(currentPosition, lastPosition) > 0.1f || 
                Quaternion.Angle(currentRotation, lastRotation) > 1f)
            {
                SendPosition(currentPosition, currentRotation);
                lastPosition = currentPosition;
                lastRotation = currentRotation;
            }
            
            lastSendTime = Time.time;
        }
    }
    
    void SendPosition(Vector3 position, Quaternion rotation)
    {
        var roomWebSocketObj = GameObject.Find("RoomWebSocketManager");
        if (roomWebSocketObj != null)
        {
            string message = $"__PLAYER_POSITION__:{playerId}:{position.x:F2},{position.y:F2},{position.z:F2}:{rotation.x:F2},{rotation.y:F2},{rotation.z:F2},{rotation.w:F2}";
            roomWebSocketObj.SendMessage("SendMessage", message);
            Debug.Log($"위치 전송: {message}");
        }
    }
} 