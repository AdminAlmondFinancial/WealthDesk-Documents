CREATE TABLE chats (
    id           uuid         PRIMARY KEY,
    user_id      varchar(128) NOT NULL,
    title        varchar(200) NOT NULL,
    created_utc  timestamptz  NOT NULL
);
