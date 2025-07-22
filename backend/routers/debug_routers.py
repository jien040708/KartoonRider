# routers/debug_router.py
from fastapi import APIRouter
from fastapi.responses import FileResponse
import os

router = APIRouter()

@router.get("/download-db")
def download_db():
    db_path = "db.db"  # 실제 DB 경로로 바꿔주세요
    if os.path.exists(db_path):
        return FileResponse(path=db_path, filename="db.db", media_type='application/octet-stream')
    return {"error": "Database file not found"}
