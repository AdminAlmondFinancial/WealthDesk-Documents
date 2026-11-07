CREATE TABLE formatted_documents (
    id           uuid         PRIMARY KEY,
    user_id      varchar(128) NOT NULL,
    name         varchar(260) NOT NULL,
    meta         varchar(200) NOT NULL,
    content      bytea        NOT NULL,
    created_utc  timestamptz  NOT NULL
);
