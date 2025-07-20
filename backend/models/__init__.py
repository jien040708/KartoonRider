from .user import User
from .room import Room

# ✅ Pydantic 2 이상에서는 이렇게만 써야 함
User.update_forward_refs()
