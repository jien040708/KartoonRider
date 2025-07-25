from typing import Optional
from sqlmodel import Field, SQLModel, Relationship
from typing import List, ForwardRef
from datetime import datetime

Room = ForwardRef("Room")

# 사용자 테이블
class User(SQLModel, table=True):
    __tablename__ = "users"
    id: Optional[int] = Field(default=None, primary_key=True)
    login_id: str = Field(index=True, unique=True)  # 로그인용 ID
    nickname: str
    password: str  # 해시된 비밀번호
    created_at: datetime = Field(default_factory=datetime.utcnow)
    rating: int = Field(default=1000)
    coin: int = Field(default=100)
    hosted_rooms: List["Room"] = Relationship(back_populates="host_user")

User.update_forward_refs()
