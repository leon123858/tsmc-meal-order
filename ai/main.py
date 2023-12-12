from typing import List

from fastapi import FastAPI, HTTPException

from response import MenuEmbeddingResponse, ResponseMenuEmbedding, RecommendMenuResponse, Response
from embeddingGenerator import EmbeddingGenerator
from repository import EmbeddingRepository
from model import MenuItem

app: FastAPI = FastAPI()
print("run doc on http://127.0.0.1:8000/docs")

generator = EmbeddingGenerator()
repository = EmbeddingRepository()


@app.get("/api/ai/recommend/{user_input}")
async def recommend(user_input: str) -> RecommendMenuResponse:
    try:
        input_embedding = generator.get_embedding(user_input)
        results = repository.get_menu_recommend(input_embedding)

        print(len(results))
        for result in results:
            print(result)

        return RecommendMenuResponse(
            data=results,
            message="Success",
            result=True
        )

    except Exception as e:
        raise HTTPException(status_code=500, detail={"data": None, "message": f"Error: {e}", "result": False})


@app.post("/api/ai/add")
async def add_menu(menu: List[MenuItem]) -> Response:
    try:
        menu_embeddings = generator.get_menu_embedding(menu)

        for menu_embedding in menu_embeddings:
            print(menu_embedding)
            repository.add_menu_embedding(menu_embedding.menuId, menu_embedding.index, menu_embedding.embedding)

        return Response(
            data=None,
            message="Success",
            result=True
        )
    except Exception as e:
        raise HTTPException(status_code=500, detail={"data": None, "message": f"Error: {e}", "result": False})


@app.get("/api/ai/get")
async def get_menu() -> MenuEmbeddingResponse:
    try:
        menu_embeddings = repository.get_menu_embedding()

        response = [ResponseMenuEmbedding(menuId=menu_embedding.menuId, index=menu_embedding.index, embedding=menu_embedding.embedding[:10]) for menu_embedding in menu_embeddings]

        return MenuEmbeddingResponse(
            data=response,
            message="Success",
            result=True
        )

    except Exception as e:
        raise HTTPException(status_code=500, detail={"data": None, "message": f"Error: {e}", "result": False})
