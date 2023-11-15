CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
SELECT * FROM pg_extension WHERE extname = 'uuid-ossp';

CREATE TABLE mail (
                      id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
                      user_id text NOT NULL,
                      status INTEGER NOT NULL
);

CREATE INDEX mail_user_id_index
    ON mail (user_id);