-- create game table
CREATE TABLE game (
  key VARCHAR(64)
);

-- create lines table
CREATE TABLE lines (
  data JSON,
  created_at TIMESTAMP DEFAULT NOW()
);

-- create singleton game record
INSERT INTO game VALUES (
  ''
);