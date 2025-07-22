# main.py
from fastapi import FastAPI
from dependencies.db import create_db_and_table
from routers import auth_routers, rooms_router, websocket_router, scores_routers, store_routers, debug_routers
from fastapi.middleware.cors import CORSMiddleware

app = FastAPI()


app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # 또는 Unity 실행 주소
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

@app.on_event("startup")
def on_startup():
    create_db_and_table()

app.include_router(auth_routers.router)
app.include_router(rooms_router.router)
app.include_router(websocket_router.router)
app.include_router(scores_routers.router)
app.include_router(store_routers.router)
app.include_router(debug_routers.router)