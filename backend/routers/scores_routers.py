from fastapi import APIRouter, Depends, HTTPException
from sqlmodel import Session, select
from dependencies.db import get_db_session
from models.user import User
from pydantic import BaseModel
from typing import List, Optional


router = APIRouter(prefix="/scores", tags=["Scores"])

class ScoreUpdateRequest(BaseModel):
    user_id: int
    result: str  # "win" or "lose"


@router.post("/update")
async def update_score(data: ScoreUpdateRequest, session: Session = Depends(get_db_session)):
    user = session.get(User, data.user_id)
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    
    # 결과에 따라 점수 증감
    if data.result == "win":
        user.rating += 10
    elif data.result == "lose":
        user.rating = max(0, user.rating - 10)  # 음수 방지
    else:
        raise HTTPException(status_code=400, detail="Invalid result. Must be 'win' or 'lose'.")

    session.add(user)
    session.commit()
    session.refresh(user)

    return {"message": "Score updated", "user_id": user.id, "new_score": user.rating}

@router.get("/myscore")
async def get_my_score(user_id: int, session: Session = Depends(get_db_session)):
    user = session.get(User, user_id)
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    
    return {"user_id": user.id, "nickname": user.nickname, "score": user.rating}

class ScoreRankingResponse(BaseModel):
    user_id: int
    nickname: str
    score: int

@router.get("/ranking", response_model=List[ScoreRankingResponse])
async def get_ranking(top: Optional[int] = 10, session: Session = Depends(get_db_session)):
    statement = select(User).order_by(User.rating.desc()).limit(top)
    results = session.exec(statement).all()
    return [{"user_id": u.id, "nickname": u.nickname, "score": u.rating} for u in results]



