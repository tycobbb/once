from flask import Flask

# create flask app
app = Flask(__name__)

# define root route
@app.route('/')
def index():
  return "<h1>hello</h1>"
