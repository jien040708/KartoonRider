from fastapi import APIRouter, Depends, HTTPException, status, Request
from sqlmodel import Session
from dependencies.db import get_db_session
from dependencies.redis_client import redis  
from models.user import User
from pydantic import BaseModel
from services.room_service import *

router = APIRouter(prefix="/rooms", tags=["Rooms"])

class RoomCreateRequest(BaseModel):
    name: str

class JoinRequest(BaseModel):
    user_id: int

@router.post("/create/{host_id}")
async def create_room(host_id: int, data: RoomCreateRequest):
    room_code = await create_room_in_redis(host_id, data.name)
    return {"room_code": room_code}

@router.post("/join/{code}")
async def join_room(code: str, req: JoinRequest):
    if not await room_exists(code):
        raise HTTPException(404, "방이 존재하지 않습니다.")
    await add_player_to_room(code, req.user_id)
    return {"joined": True}

@router.post("/leave/{code}")
async def leave_room(code: str, req: JoinRequest):
    await remove_player_from_room(code, req.user_id)
    return {"left": True}

@router.get("/info/{code}")
async def room_info(code: str):
    data = await get_room_info(code)
    if not data:
        raise HTTPException(status_code=404, detail="방을 찾을 수 없습니다.")
    return data