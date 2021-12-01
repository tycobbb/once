import flask as f
import json
import psycopg2 as pg
import random

# create flask app
app = f.Flask(__name__)
con = pg.connect("user=postgres dbname=upon")

# define root route
@app.route("/", methods = ["post"])
def index():
  # get req json
  req = f.request.get_json()
  if req == None:
    return json.dumps({ "err": "no request params" }), 400

  # get req params
  pwrd = req.get("Password")
  if pwrd == None or pwrd == "":
    return json.dumps({ "err": "missing encrypted password" }), 400

  pkey = req.get("PublicKey")
  if pkey == None or pkey == "":
    return json.dumps({ "err": "missing public key" }), 400

  # find game
  cur = con.cursor()
  cur.execute("SELECT * FROM game LIMIT 1")

  # grab data dictionary
  recs = cur.fetchall()
  data = recs[0][0]

  # add stuff to it
  data["key"] = random.randrange(1, 100)

  # update the record
  str = json.dumps(data)
  cur.execute("UPDATE game SET data = %s", (str,))

  return str, 200
