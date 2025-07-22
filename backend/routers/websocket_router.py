from fastapi import APIRouter, WebSocket, WebSocketDisconnect
from typing import Dict

router = APIRouter()

# 방 별 유저 WebSocket 저장: {"ROOM123": {"user1": websocket1, "user2": websocket2}}
active_connections: Dict[str, Dict[str, WebSocket]] = {}

# 브로드캐스트 함수
async def broadcast_message(room_code: str, message: str, exclude: str = None):
    if room_code not in active_connections:
        return

    for uid, conn in active_connections[room_code].items():
        if uid != exclude:
            await conn.send_text(message)

# 웹소켓 엔드포인트
@router.websocket("/ws/{room_code}/{user_id}")
async def websocket_endpoint(websocket: WebSocket, room_code: str, user_id: str):
    await websocket.accept()

    # 방이 없으면 새로 생성
    if room_code not in active_connections:
        active_connections[room_code] = {}

    # 유저 등록
    active_connections[room_code][user_id] = websocket

    try:
        # 입장 메시지 전송
        await broadcast_message(
            room_code,
            f"✅ {user_id} joined. 현재 인원: {len(active_connections[room_code])}"
        )

        # 메시지 반복 수신 및 브로드캐스트
        while True:
            data = await websocket.receive_text()
            print(f"[{room_code}] {user_id}: {data}")
            await broadcast_message(
                room_code,
                f"{user_id}: {data}",
                exclude=user_id
            )

    except WebSocketDisconnect:
        # 연결 종료 시 정리
        print(f"❌ {user_id} disconnected")
        del active_connections[room_code][user_id]

        if not active_connections[room_code]:
            # 아무도 없으면 방 삭제
            del active_connections[room_code]
        else:
            # 퇴장 알림
            await broadcast_message(
                room_code,
                f"❌ {user_id} left. 현재 인원: {len(active_connections[room_code])}"
            )

@router.get("/room/{room_code}/count")
async def get_room_player_count(room_code: str):
    if room_code not in active_connections:
        raise HTTPException(status_code=404, detail="Room not found")

    return {
        "room_code": room_code,
        "current_players": len(active_connections[room_code])
    }