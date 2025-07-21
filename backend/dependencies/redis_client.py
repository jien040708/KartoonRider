from redis import asyncio as aioredis
import os

REDIS_URL = os.getenv("REDIS_URL", "redis://localhost:6379")  # ← 레일웨이용

redis = aioredis.from_url(REDIS_URL, decode_responses=True)