from sqlmodel import SQLModel, Field, Relationship
from sqlalchemy import ForeignKey
from typing import Optional
from datetime import datetime

import time
import random
import string

def generate_room_code():
    return ''.join(random.choices(string.ascii_uppercase + string.digits, k=6))

class Room(SQLModel, table=True):
    id: Optional[int] = Field(default=None, primary_key=True)
    code: str = Field(default_factory=generate_room_code, index=True)
    name: str
    host_user_id: int  = Field(foreign_key="users.id") # 누가 만들었는지
    host_user: Optional["User"] = Relationship(back_populates="hosted_rooms")

    created_at: datetime = Field(default_factory=datetime.utcnow)
