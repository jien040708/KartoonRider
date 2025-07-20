# routers/websocket_router.py
from fastapi import APIRouter, WebSocket, WebSocketDisconnect
from typing import Dict

router = APIRouter()

# 연결된 유저 저장용
active_connections: Dict[str, WebSocket] = {}

@router.websocket("/ws/{room_code}/{user_id}")
async def websocket_endpoint(websocket: WebSocket, room_code: str, user_id: str):
    await websocket.accept()
    conn_key = f"{room_code}_{user_id}"
    active_connections[conn_key] = websocket
    try:
        while True:
            data = await websocket.receive_text()
            print(f"[{room_code}] {user_id}: {data}")
            # 예: 같은 방에 broadcast 하기
            for key, conn in active_connections.items():
                if key.startswith(room_code) and conn != websocket:
                    await conn.send_text(f"{user_id}: {data}")
    except WebSocketDisconnect:
        print(f"❌ {user_id} disconnected")
        del active_connections[conn_key]
