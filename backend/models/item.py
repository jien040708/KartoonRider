from typing import Optional
from sqlmodel import SQLModel, Field
from datetime import datetime

# 상점에 등록된 아이템
class StoreItem(SQLModel, table=True):
    id: Optional[int] = Field(default=None, primary_key=True)
    name: str
    description: Optional[str] = None
    type: str  # 예: "카트", "캐릭터"
    price: int
    created_at: datetime = Field(default_factory=datetime.utcnow)


# 사용자가 보유한 아이템
class UserItem(SQLModel, table=True):
    id: Optional[int] = Field(default=None, primary_key=True)
    user_id: int = Field(foreign_key="user.id")
    item_id: int = Field(foreign_key="storeitem.id")
    is_equipped: bool = Field(default=False)
    acquired_at: datetime = Field(default_factory=datetime.utcnow)
