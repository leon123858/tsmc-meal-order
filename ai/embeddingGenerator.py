import os
from typing import List

import numpy as np
import pandas as pd
from dotenv import load_dotenv
from openai import OpenAI

from model import MenuItem, MenuEmbedding


class EmbeddingGenerator:
    client: OpenAI = None

    def __init__(self):
        load_dotenv()
        openai_key = os.getenv("OPENAI_KEY")
        self.client = OpenAI(api_key=openai_key)

    def get_menu_embedding(self, menu: List[MenuItem]) -> List[MenuEmbedding]:
        print(f"Generating embedding for menu")
        data_frame = pd.DataFrame([item.dict() for item in menu])
        data_frame['combined'] = data_frame['Name'] + ' ' + data_frame['Description'] + ' ' + ' '.join(data_frame['Tags'].apply(lambda x: ' '.join(x)))
        data_frame['ada_embedding'] = data_frame['combined'].apply(lambda x: self.get_embedding(x))

        return [MenuEmbedding(menuId=row[0], index=row[1], embedding=row[2]) for row in data_frame[['MenuId', 'Index', 'ada_embedding']].values]

    def get_embedding(self, text: str, engine="text-embedding-ada-002") -> List[float]:
        print(f"Generating embedding for text: {text}")
        text = text.replace("\n", " ")

        #response = self.client.embeddings.create(input=[text], model=engine)
        return [float(i) + 0.0 for i in range(1536)]

        #print(f"Embedding: {response.data[0].embedding}")

        #return response.data[0].embedding

    @staticmethod
    def cosine_similarity(a, b):
        return np.dot(a, b) / (np.linalg.norm(a) * np.linalg.norm(b))
