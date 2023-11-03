from fastapi import FastAPI
from pydantic import BaseModel


class Item(BaseModel):
    id: int
    name: str
    quantity: int


app: FastAPI = FastAPI()
print("run doc on http://127.0.0.1:8000/docs")


# fake db
db = [Item(id=1, name="apple", quantity=10), Item(id=2, name="banana", quantity=5),
      Item(id=3, name="orange", quantity=3), Item(id=4, name="mango", quantity=2), Item(id=5, name="grape", quantity=1)]


@app.get(path="/", operation_id="hello")
def hello():
    return "hello world"


@app.get(
    path='/items/{item_id}',
    operation_id='read_item',
    response_model=Item,
)
def read_item(item_id: int):
    """Read item by id."""
    item: Item = next((item for item in db if item.id == item_id), None)

    return item
