import base64
import hashlib
import flask as f
import json
import os
import psycopg2 as pg
import operator as op

# -- main --
# create flask app
app = f.Flask(__name__)

# connect to db
url = os.environ.get("DATABASE_URL")
con = pg.connect(url or "user=postgres dbname=upon")

# -- routes --
# unlock the game and get existing lines
@app.route("/unlock", methods = ["post"])
def unlock():
  # get req json
  params = f.request.get_json()
  if params == None:
    return fmt_err("no request params"), 400

  # verify and update the key
  err, code, cur = verify_key(params)
  if code != 200:
    return err, code
  elif cur == None:
    return fmt_err("missing db cursor"), 500

  # fetch all the lines
  cur.execute("SELECT * FROM lines")
  recs = cur.fetchall()
  recs = list(map(op.itemgetter(0), recs))

  # release cursor and commit
  con.commit()
  cur.close()

  # grab data dictionary
  resp = {
    "Lines": recs
  }

  return fmt_res(resp), 200

# add a line to the game
@app.route("/unlock/line", methods = ["post"])
def add_line():
  # get req json
  params = f.request.get_json()
  if params == None:
    return fmt_err("no request params"), 400

  # validate line params
  data = params
  data = data.get("Data")
  data = data.get("Line")

  if data == None:
    return fmt_err("missing line data"), 400

  # verify and update the key
  err, code, cur = verify_key(params)
  if code != 200:
    return err, code
  elif cur == None:
    return fmt_err("missing db cursor"), 500

  # add the line
  str = json.dumps(data)
  cur.execute("INSERT INTO lines VALUES (%s)", (str,))

  # release cursor and commit
  con.commit()
  cur.close()

  return fmt_res("good job"), 200

# -- commands --
# verifies and saves the new key
def verify_key(params):
  # get key
  pkey = params.get("Key")
  if pkey == None:
    return fmt_err("missing key"), 400, None

  # get req params
  nonce = pkey.get("Nonce")
  if nonce == None or nonce == "":
    return fmt_err("missing key.nonce"), 400, None

  next_key = pkey.get("Next")
  if next_key == None or next_key == "":
    return fmt_err("missing key.next"), 400, None

  # find game
  cur = con.cursor()
  cur.execute("SELECT * FROM game LIMIT 1")

  # grab record
  rec = cur.fetchone()

  # compute next key
  prev_key = rec[0] or ""
  next_hsh = hashlib.sha256(
    base64.b64decode(prev_key) +
    base64.b64decode(nonce)
  )

  computed = base64.b64encode(next_hsh.digest()).decode("utf-8")

  # if keys don't match, reject
  if next_key != computed:
    cur.close()
    return fmt_err("unauthorized"), 401, None

  # otherwise, save new key
  cur.execute("UPDATE game SET key = %s", (next_key,))

  # return success resp
  return None, 200, cur

# -- helpers --
# format error response string
def fmt_err(err):
  return json.dumps({ "Err": err })

# format success response string
def fmt_res(data):
  return json.dumps({ "Data": data })
