"""
    Name : Ignas Rocas
    Purpose : 4th year project cloud prediction
"""
from flask import Flask,request
import joblib


model = None
app = Flask(__name__)

#load model
def load_model():
    global model
    model = joblib.load('planModel')

#test app
@app.route('/')
def hello_world():
    return 'Hello from Flask!'


#prediction
@app.route('/predict', methods=['POST'])
def get_prediction():
    data = request.get_json(force=True)  
    data = [list(data.values())]        
    prediction = model.predict(data)  
    return str(round(prediction[0],2))


if __name__ == '__main__':
    load_model()  
    app.run(host='0.0.0.0',port=80)