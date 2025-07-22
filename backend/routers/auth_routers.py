from fastapi import APIRouter, Depends, HTTPException, status, Request
from sqlmodel import Session
from dependencies.db import get_db_session
from dependencies.jwt_utils import JWTUtil
from services.auth_services import AuthService
from models.user import User
from pydantic import BaseModel

router = APIRouter(prefix="/auth", tags=["Authentication"])

auth_service = AuthService()
jwt_util = JWTUtil()


# ✅ 요청 스키마 정의
class RegisterRequest(BaseModel):
    login_id: str
    nickname: str
    password: str

class LoginRequest(BaseModel):
    login_id: str
    password: str


# ✅ 회원가입
@router.post("/register")
def register(data: RegisterRequest, db: Session = Depends(get_db_session)):
    existing_user = auth_service.get_user_by_login_id(db, data.login_id)
    if existing_user:
        raise HTTPException(status_code=400, detail="이미 존재하는 로그인 ID입니다.")
    
    user = auth_service.signup(
        db=db,
        login_id=data.login_id,
        nickname=data.nickname,
        password=data.password
    )
    if not user:
        raise HTTPException(status_code=500, detail="회원가입 실패")
    
    return {"message": "회원가입 성공", "user_id": user.id}


# ✅ 로그인 (JWT 발급)
@router.post("/login")
def login(data: LoginRequest, db: Session = Depends(get_db_session)):
    user = auth_service.signin(db, login_id=data.login_id, password=data.password)
    if not user:
        raise HTTPException(status_code=401, detail="로그인 실패: 잘못된 정보")
    
    token = jwt_util.create_token({"sub": str(user.id)})
    return {"access_token": token, "token_type": "bearer", "user_id": user.id}


# ✅ 내 정보 조회 (토큰 필요)
@router.get("/me")
def get_me(request: Request, db: Session = Depends(get_db_session)):
    auth_header = request.headers.get("Authorization")
    if not auth_header or not auth_header.startswith("Bearer "):
        raise HTTPException(status_code=401, detail="인증 토큰이 없습니다.")
    
    token = auth_header.split(" ")[1]
    payload = jwt_util.decode_token(token)
    if not payload:
        raise HTTPException(status_code=401, detail="토큰이 유효하지 않습니다.")

    user_id = int(payload.get("sub"))
    user = db.get(User, user_id)
    if not user:
        raise HTTPException(status_code=404, detail="사용자를 찾을 수 없습니다.")
    
    return {
        "id": user.id,
        "login_id": user.login_id,
        "nickname": user.nickname,
        "rating": user.rating,
        "coin": user.coin,
        "created_at": user.created_at
    }
