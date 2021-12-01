-- create game table
CREATE
  TABLE game
  (data json);

-- create singleton game rec
INSERT
  INTO game
  VALUES ('{}');
