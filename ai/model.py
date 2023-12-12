from typing import List

from pydantic import BaseModel


class MenuItem(BaseModel):
    MenuId: str
    Index: str
    Description: str
    Name: str
    Price: int
    Tags: List[str]


class MenuEmbedding(BaseModel):
    menuId: str
    index: int
    embedding: List[float]
