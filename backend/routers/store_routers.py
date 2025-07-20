from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session, select
from dependencies.db import get_db_session
from models.user import User
from models.item import StoreItem, UserItem
from pydantic import BaseModel
from typing import List, Optional

router = APIRouter(prefix="/store", tags=["store"])


# ----------------------------
# 1. 아이템 목록 조회
# ----------------------------
@router.get("/items", response_model=List[StoreItem])
async def get_items(session: Session = Depends(get_db_session)):
    return session.exec(select(StoreItem)).all()


# ----------------------------
# 2. 아이템 구매
# ----------------------------
class BuyItemRequest(BaseModel):
    user_id: int
    item_id: int

@router.post("/buy")
async def buy_item(data: BuyItemRequest, session: Session = Depends(get_db_session)):
    user = session.get(User, data.user_id)
    item = session.get(StoreItem, data.item_id)

    if not user or not item:
        raise HTTPException(status_code=404, detail="User or Item not found")

    # 중복 구매 방지
    duplicate = session.exec(
        select(UserItem).where(UserItem.user_id == user.id, UserItem.item_id == item.id)
    ).first()
    if duplicate:
        raise HTTPException(status_code=400, detail="이미 구매한 아이템입니다")

    if user.coin < item.price:
        raise HTTPException(status_code=400, detail="코인이 부족합니다")

    user.coin -= item.price
    session.add_all([user, UserItem(user_id=user.id, item_id=item.id)])
    session.commit()
    return {"message": f"{item.name} 구매 완료", "remaining_coin": user.coin}


# ----------------------------
# 3. 내 인벤토리 조회
# ----------------------------
class InventoryItemResponse(BaseModel):
    item_id: int
    name: str
    description: Optional[str]
    type: str
    is_equipped: bool

@router.get("/inventory", response_model=List[InventoryItemResponse])
async def get_inventory(user_id: int, session: Session = Depends(get_db_session)):
    stmt = (
        select(UserItem, StoreItem)
        .join(StoreItem, UserItem.item_id == StoreItem.id)
        .where(UserItem.user_id == user_id)
    )
    results = session.exec(stmt).all()
    return [
        InventoryItemResponse(
            item_id=store_item.id,
            name=store_item.name,
            description=store_item.description,
            type=store_item.type,
            is_equipped=user_item.is_equipped
        )
        for user_item, store_item in results
    ]


# ----------------------------
# 4. 장착 변경
# ----------------------------
class EquipItemRequest(BaseModel):
    user_id: int
    item_id: int

@router.patch("/equip")
async def equip_item(data: EquipItemRequest, session: Session = Depends(get_db_session)):
    # 1. 해당 유저의 모든 아이템 장착 해제
    all_items = session.exec(
        select(UserItem).where(UserItem.user_id == data.user_id)
    ).all()
    for item in all_items:
        item.is_equipped = False
        session.add(item)

    # 2. 요청한 아이템 장착
    target_item = session.exec(
        select(UserItem)
        .where(UserItem.user_id == data.user_id, UserItem.item_id == data.item_id)
    ).first()
    if not target_item:
        raise HTTPException(status_code=404, detail="해당 아이템은 인벤토리에 없습니다")

    target_item.is_equipped = True
    session.add(target_item)
    session.commit()

    return {"message": f"{data.item_id}번 아이템 장착 완료"}
