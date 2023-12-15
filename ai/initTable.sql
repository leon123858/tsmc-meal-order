CREATE TABLE menu_embeddings
(
    id        bigserial PRIMARY KEY,
    menu_id   varchar(50)  NOT NULL,
    index     int          NOT NULL,
    embedding vector(1536) NOT NULL,
    CONSTRAINT unique_menu_index_pair UNIQUE (menu_id, index)
);

CREATE USER db_user WITH PASSWORD 'user_password_000';
GRANT SELECT, INSERT, DELETE ON TABLE menu_embeddings TO db_user;
GRANT SELECT, INSERT, DELETE ON TABLE self_menu_embeddings TO db_user;
GRANT USAGE ON ALL SEQUENCES IN SCHEMA public TO db_user;
