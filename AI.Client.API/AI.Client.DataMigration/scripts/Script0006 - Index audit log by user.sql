CREATE INDEX ix_audit_log_user_created ON audit_log (user_id, created_utc DESC);
