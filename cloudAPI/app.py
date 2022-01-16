
# A very simple Flask Hello World app for you to get started with...

from flask import Flask, request
import joblib
import smtplib
from email.mime.text import MIMEText
#from flask_restful import Api, Resource, reqparse
#import numpy as np

app = Flask(__name__)
model = joblib.load('planModel')

@app.route('/')
def home():
    return "nobody is here"

def send_email_confirm():
    try:
    #Create your secure SMTP session
        smtp = smtplib.SMTP('smtp.gmail.com', 587)
        smtp.starttls()
        smtp.login("dinamicinsuranceapp@gmail.com","6714d944a286291a12784df0302e5a0910fd2fe75a09023daceaeb6f54ab6eb1")

        # ------------------- remove
        price = 1.5
        name = "Ignas"
        application = "http://www.aplication.com/insurance app..."
        #--------------------

        # create email string
        text =f"\n Hi {name},\n\n Your insurance Quote is : {price} \n\n Please follow link below back to your application \n {application}"
        msg = MIMEText(text)
        msg['Subject'] = 'Dinamic Insurance Quote'
        msg['From'] = 'dinamicinsuranceapp@gmail.com'
        msg['To'] = 'ignasandholly@gmail.com'           #Change this

        #send email
        smtp.sendmail("dinamicinsuranceapp@gmail.com", "ignasandholly@gmail.com",msg.as_string()) # ignas@gmail.com  to inputed
        smtp.quit()
        print ("Email sent successfully!")

    except Exception as ex:
        print("Something went wrong....",ex)


@app.route('/predict', methods=['POST'])
def get_prediction():
    columns = ['Hospitals','Age','Cover','Hospital_Excess','Plan','Smoker']
    data = request.get_json(force=True)  # Get data posted as a json

    temp =[]
    for i in columns: temp.append(data[i])

    prediction = model.predict([temp])  # runs globally loaded model on the data
    return str(round(prediction[0],2))

