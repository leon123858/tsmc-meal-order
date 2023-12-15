from typing import List

from pydantic import BaseModel


class ResponseMenuItem(BaseModel):
    menuId: str
    Index: int


class ResponseMenuEmbedding(BaseModel):
    menuId: str
    index: int
    embedding: List[float]


class Response(BaseModel):
    data: object
    message: str
    result: bool


class RecommendMenuResponse(Response):
    data: List[ResponseMenuItem]


class MenuEmbeddingResponse(Response):
    data: List[ResponseMenuEmbedding]
