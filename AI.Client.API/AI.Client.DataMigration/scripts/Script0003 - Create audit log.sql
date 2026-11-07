CREATE TABLE audit_log (
    id           bigserial    PRIMARY KEY,
    user_id      varchar(128) NOT NULL,
    action       varchar(100) NOT NULL,
    status_code  integer      NULL,
    duration_ms  integer      NULL,
    created_utc  timestamptz  NOT NULL
);
