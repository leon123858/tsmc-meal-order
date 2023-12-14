from typing import List

from pydantic import BaseModel


class FoodItem(BaseModel):
    Name: str
    Description: str
    Tags: List[str]


class Menu(BaseModel):
    MenuId: str
    FoodItems: List[FoodItem]


class MenuEmbedding(BaseModel):
    menuId: str
    index: int
    embedding: List[float]
