import base64
import hashlib
import flask as f
import json
import psycopg2 as pg

# -- main --
# create flask app
app = f.Flask(__name__)
con = pg.connect("user=postgres dbname=upon")

# -- routes --
@app.route("/unlock", methods = ["post"])
def index():
  # get req json
  req = f.request.get_json()
  if req == None:
    return fmt_err("no request params"), 400

  # get req params
  nonce = req.get("Nonce")
  if nonce == None or nonce == "":
    return fmt_err("missing nonce"), 400

  next_key = req.get("NextKey")
  if next_key == None or next_key == "":
    return fmt_err("missing next key"), 400

  # find game
  cur = con.cursor()
  cur.execute("SELECT * FROM game LIMIT 1")

  # grab data dictionary
  recs = cur.fetchall()
  data = recs[0][0]

  # compute next key
  prev_key = data["key"]
  next_hsh = hashlib.sha256(
    base64.b64decode(prev_key) +
    base64.b64decode(nonce)
  )

  computed = base64.b64encode(next_hsh.digest()).decode("utf-8")

  # if keys don't match, reject
  if next_key != computed:
    return fmt_err("unauthorized"), 401

  # otherwise, save new key
  data["key"] = next_key
  str = json.dumps(data)
  cur.execute("UPDATE game SET data = %s", (str,))

  # return success resp
  return fmt_res("have fun"), 200

# -- helpers --
# format error response string
def fmt_err(err):
  return json.dumps({ "err": err })

# format success response string
def fmt_res(body):
  return json.dumps({ "body": body })
