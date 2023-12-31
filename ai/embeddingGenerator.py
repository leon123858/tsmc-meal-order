import asyncio
import os
import random
from typing import List

import numpy as np
import pandas as pd
from dotenv import load_dotenv
from openai import AsyncOpenAI
from pydantic import BaseModel

from model import MenuEmbedding, Menu


class MenuItem(BaseModel):
    MenuId: str
    Index: int
    Description: str
    Name: str
    Tags: List[str]


def convert_menu_to_menu_item(menu: Menu) -> List[MenuItem]:
    menu_items = []
    for food_index, food_item in enumerate(menu.FoodItems):
        menu_item = MenuItem(
            MenuId=menu.MenuId,
            Index=food_index,
            Description=food_item.Description,
            Name=food_item.Name,
            Tags=food_item.Tags
        )
        menu_items.append(menu_item)
    return menu_items


class EmbeddingGenerator:
    client: AsyncOpenAI = None
    isDev: bool = False

    def __init__(self):
        load_dotenv()
        openai_key = os.getenv("OPENAI_KEY")

        if openai_key is None:
            self.isDev = True
            return

        self.client = AsyncOpenAI(api_key=openai_key)

    async def get_menu_embedding(self, menu: Menu) -> List[MenuEmbedding]:
        # print(f"Generating embedding for menu")
        menu_items = convert_menu_to_menu_item(menu)
        data_frame = pd.DataFrame([item.dict() for item in menu_items])
        data_frame['combined'] = data_frame['Name'] + ' ' + data_frame['Description'] + ' ' + ' '.join(
            data_frame['Tags'].apply(lambda x: ' '.join(x)))
        data_frame['ada_embedding'] = await asyncio.gather(*(self.get_embedding(x) for x in data_frame['combined']))

        return [MenuEmbedding(menuId=row[0], index=row[1], embedding=row[2]) for row in
                data_frame[['MenuId', 'Index', 'ada_embedding']].values]

    async def get_embedding(self, text: str, engine="text-embedding-ada-002") -> List[float]:
        if self.isDev:
            await asyncio.sleep(random.uniform(0.5, 2.0))
            return [float(i) + 0.5 for i in range(1536)]

        # print(f"Generating embedding for text: {text}")
        text = text.replace("\n", " ")

        response = await self.client.embeddings.create(input=[text], model=engine)

        # print(f"Embedding: {response.data[0].embedding}")

        return response.data[0].embedding

    @staticmethod
    def cosine_similarity(a, b):
        return np.dot(a, b) / (np.linalg.norm(a) * np.linalg.norm(b))
