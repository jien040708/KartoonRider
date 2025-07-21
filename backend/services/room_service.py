# services/room_manager.py
import uuid
import json
from datetime import datetime
from dependencies.redis_client import redis  # redis 클라이언트 import

async def create_room_in_redis(host_id: int, name: str) -> str:
    room_code = str(uuid.uuid4())[:6]
    room_data = {
        "host_id": str(host_id),
        "name": name,
        "players": json.dumps([str(host_id)]),
        "status": "waiting",
        "created_at": datetime.utcnow().isoformat()
    }
    for key, value in room_data.items():
        await redis.hset(f"room:{room_code}", key, value)
    return room_code

async def room_exists(room_code: str) -> bool:
    return await redis.exists(f"room:{room_code}")

async def add_player_to_room(room_code: str, user_id: int):
    key = f"room:{room_code}"
    players_json = await redis.hget(key, "players")
    players = json.loads(players_json)
    if str(user_id) not in players:
        players.append(str(user_id))
        await redis.hset(key, mapping={"players": json.dumps(players)})

async def remove_player_from_room(room_code: str, user_id: int):
    key = f"room:{room_code}"
    players_json = await redis.hget(key, "players")
    if players_json is None:
        return
    players = json.loads(players_json)
    players = [uid for uid in players if uid != str(user_id)]
    await redis.hset(key, mapping={"players": json.dumps(players)})

async def get_room_info(room_code: str):
    key = f"room:{room_code}"
    if not await redis.exists(key):
        return None
    data = await redis.hgetall(key)
    data["players"] = json.loads(data["players"])
    return data


async def delete_room_in_redis(room_code: str) -> bool:
    key = f"room:{room_code}"
    exists = await redis.exists(key)

    if not exists:
        return False
    
    await redis.delete(key)
    return True