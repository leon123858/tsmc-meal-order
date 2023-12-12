import json
from ast import literal_eval
from typing import List

import psycopg2
from psycopg2 import sql
from dotenv import load_dotenv
import os

from response import ResponseMenuItem
from model import MenuEmbedding


class EmbeddingRepository:
    conn_str = ""

    def __init__(self):
        load_dotenv()
        db_server = "menu.postgres.database.azure.com"
        db_user = "db_user"
        db_password = os.getenv("AZURE_PASSWORD")
        self.conn_str = f"host={db_server} dbname=postgres user={db_user} password={db_password} sslmode=require"

    def add_menu_embedding(self, menu_id: str, index: int, embedding: List[float]) -> None:
        connection, cursor = None, None

        # print(f"menu_id: {menu_id}, index: {index}, embedding: {embedding}")

        try:
            connection = psycopg2.connect(self.conn_str)

            cursor = connection.cursor()

            cursor.execute(
                sql.SQL("SELECT * FROM menu_embeddings WHERE menu_id = %s AND index = %s"),
                (menu_id, index),
            )

            existing_record = cursor.fetchone()

            if existing_record:
                cursor.execute(
                    sql.SQL("UPDATE menu_embeddings SET embedding = %s WHERE menu_id = %s AND index = %s"),
                    (embedding, menu_id, index),
                )
            else:
                cursor.execute(
                    sql.SQL("INSERT INTO menu_embeddings (menu_id, index, embedding) VALUES (%s, %s, %s)"),
                    (menu_id, index, embedding),
                )

            connection.commit()

        except Exception as e:
            print(f"Error: {e}")
            raise e

        finally:
            if cursor:
                cursor.close()
            if connection:
                connection.close()

    def get_menu_recommend(self, embedding: List[float]) -> List[ResponseMenuItem]:
        connection, cursor = None, None

        try:
            connection = psycopg2.connect(self.conn_str)

            cursor = connection.cursor()

            embeddingStr = json.dumps(embedding)
            cursor.execute(sql.SQL("select * from menu_embeddings order by cosine_distance(embedding, %s)"), (embeddingStr,))

            rows = cursor.fetchall()

            connection.commit()

            return [ResponseMenuItem(menuId=row[1], Index=row[2]) for row in rows]

        except Exception as e:
            print(f"Error: {e}")
            raise e

        finally:
            if cursor:
                cursor.close()
            if connection:
                connection.close()

    def get_menu_embedding(self) -> List[MenuEmbedding]:
        connection, cursor = None, None

        try:
            connection = psycopg2.connect(self.conn_str)

            cursor = connection.cursor()

            cursor.execute(sql.SQL("select * from menu_embeddings"))

            rows = cursor.fetchall()

            connection.commit()

            return [MenuEmbedding(menuId=row[1], index=row[2], embedding=list(map(float, literal_eval(row[3])))) for row in rows]

        except Exception as e:
            print(f"Error: {e}")
            raise e

        finally:
            if cursor:
                cursor.close()
            if connection:
                connection.close()