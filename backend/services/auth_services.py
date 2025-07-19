import bcrypt
from sqlmodel import Session, select
from models.user import User
from datetime import datetime

class AuthService:
    def get_hashed_password(self, password: str) -> str:
        return bcrypt.hashpw(password.encode('utf-8'), bcrypt.gensalt()).decode('utf-8')

    def verify_password(self, plain_password: str, hashed_password: str) -> bool:
        return bcrypt.checkpw(plain_password.encode('utf-8'), hashed_password.encode('utf-8'))

    def signup(self, db: Session, login_id: str, nickname: str, password: str) -> User | None:
        try:
            hashed_password = self.get_hashed_password(password)
            user = User(
                login_id=login_id,
                nickname=nickname,
                password=hashed_password,
                created_at=datetime.utcnow(),
                rating=1000
            )
            db.add(user)
            db.commit()
            db.refresh(user)
            return user
        except Exception as e:
            print(f"[signup error] {e}")
            return None

    def get_user_by_login_id(self, db: Session, login_id: str) -> User | None:
        return db.exec(select(User).where(User.login_id == login_id)).first()

    def signin(self, db: Session, login_id: str, password: str) -> User | None:
        user = self.get_user_by_login_id(db, login_id)
        if not user:
            return None
        if not self.verify_password(password, user.password):
            return None
        return user
