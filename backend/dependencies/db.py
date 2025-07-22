# db.py
from sqlmodel import SQLModel, Session, create_engine
import os

DB_URL = os.getenv("DATABASE_URL")
DB_ENGINE = create_engine(DB_URL, echo=True)

def get_db_session():
    with Session(DB_ENGINE) as sess:
        yield sess

def create_db_and_table():
    SQLModel.metadata.create_all(DB_ENGINE)
