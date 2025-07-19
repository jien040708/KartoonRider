# main.py
from fastapi import FastAPI
from dependencies.db import create_db_and_table
from routers import auth_routers
app = FastAPI()

@app.on_event("startup")
def on_startup():
    create_db_and_table()

app.include_router(auth_routers.router)