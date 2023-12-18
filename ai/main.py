from typing import List

from fastapi import FastAPI, HTTPException

from embeddingGenerator import EmbeddingGenerator
from model import Menu
from repository import EmbeddingRepository
from response import MenuEmbeddingResponse, ResponseMenuEmbedding, RecommendMenuResponse, Response

app: FastAPI = FastAPI()
print("run doc on http://127.0.0.1:8000/docs")

generator = EmbeddingGenerator()
repository = EmbeddingRepository()


@app.get("/api/ai/recommend/{user_input}")
async def recommend(user_input: str) -> RecommendMenuResponse:
    try:
        input_embedding = await generator.get_embedding(user_input)
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
async def add_menu(menu_list: List[Menu]) -> Response:
    try:
        for menu in menu_list:
            menu_embeddings = await generator.get_menu_embedding(menu)
            repository.add_menu_embedding(menu_embeddings)

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

        response = [ResponseMenuEmbedding(menuId=menu_embedding.menuId, index=menu_embedding.index,
                                          embedding=menu_embedding.embedding[:10]) for menu_embedding in
                    menu_embeddings]

        return MenuEmbeddingResponse(
            data=response,
            message="Success",
            result=True
        )

    except Exception as e:
        raise HTTPException(status_code=500, detail={"data": None, "message": f"Error: {e}", "result": False})
