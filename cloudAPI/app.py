
# A very simple Flask Hello World app for you to get started with...

from flask import Flask, request
import joblib
import smtplib
from email.mime.text import MIMEText

app = Flask(__name__)
model = joblib.load('planModel')

codeList = ["client","client2","client3"]
codeTaken = [False,False,False]

@app.route('/')
def home():
    return "nobody is here"


@app.route('/confirmationEmail', methods=['POST'])
def confirmation_email():
    try:
        data = request.get_json(force=True)  # Get data posted as a json

        if(data != None):
            #Create your secure SMTP session
            smtp = smtplib.SMTP('smtp.gmail.com', 587)
            smtp.starttls()
            smtp.login("dinamicinsuranceapp@gmail.com","3b3f134bec5872745c2ee67e245329777b361ac6fc4c1ee5a738613f8af72d52")

            # create email string
            text =f"\nDear Customer\nPlease find confirmation code below.\n\n\nEmail Confirmation Code : {data['code']}\n\n\nKind regards.\nDynamic Personal Insurance™\n{data['date']}\n\nP.S. This is an automated email.\nPlease do not respond"
            msg = MIMEText(text)
            msg['Subject'] = 'Email confirmation '
            msg['From'] = 'Dynamic Insurance App'
            msg['To'] =  data['email']

            #send email
            smtp.sendmail("", data['email'],msg.as_string())
            smtp.quit()
            return "sent"
        return "error"

    except Exception as ex:
        return "error"

@app.route('/notifyCustomer', methods=['POST'])
def send_email_notification():
    try:
        data = request.get_json(force=True)  # Get data posted as a json

        if(data != None):
            #Create your secure SMTP session
            smtp = smtplib.SMTP('smtp.gmail.com', 587)
            smtp.starttls()
            smtp.login("dinamicinsuranceapp@gmail.com","3b3f134bec5872745c2ee67e245329777b361ac6fc4c1ee5a738613f8af72d52")

            # create email string
            text =f"\nDear {data['name']}\n\nThe Policy request has been {data['action']}\n\nPlease contact support if any questions arises\nKind regards.\nDynamic Personal Insurance\n{data['date']}"
            msg = MIMEText(text)
            msg['Subject'] = 'Policy update'
            msg['From'] = 'Dynamic Insurance App'
            msg['To'] =  data['email']

            #send email
            smtp.sendmail("dinamicinsuranceapp@gmail.com", data['email'],msg.as_string())
            smtp.quit()
            return "sent"
        return "error"

    except Exception as ex:
        return "error"

@app.route('/ClaimNotifyCustomer', methods=['POST'])
def send_email_notification2():
    try:
        data = request.get_json(force=True)  # Get data posted as a json

        if(data != None):
            #Create your secure SMTP session
            smtp = smtplib.SMTP('smtp.gmail.com', 587)
            smtp.starttls()
            smtp.login("dinamicinsuranceapp@gmail.com","3b3f134bec5872745c2ee67e245329777b361ac6fc4c1ee5a738613f8af72d52")
            action = data['action']
            text = ""
            if(action == "Accepted"):
                text =f"\nDear {data['name']}\n\nThe Claim has been Accepted.\nFurther details will be available soon.\n\nKind regards.\nDynamic Personal Insurance\n{data['date']}"
            elif(action == "Denied"):
                text =f"\nDear {data['name']}\n\nThe Claim has been Denied.\nReason: {data['reason']}\nFurther details will be available soon.\n\nKind regards.\nDynamic Personal Insurance\n{data['date']}"

            # create email string
            msg = MIMEText(text)
            msg['Subject'] = 'Claim'
            msg['From'] = 'Dynamic Insurance App'
            msg['To'] =  data['email']

            #send email
            smtp.sendmail("dinamicinsuranceapp@gmail.com", data['email'],msg.as_string())
            smtp.quit()
            return "sent"
        return "error"

    except Exception as ex:
        return "error"



@app.route('/resetPass', methods=['POST'])
def send_email_pass():
    try:
        data = request.get_json(force=True)  # Get data posted as a json

        if(data != None):
            #Create your secure SMTP session
            smtp = smtplib.SMTP('smtp.gmail.com', 587)
            smtp.starttls()
            smtp.login("dinamicinsuranceapp@gmail.com","3b3f134bec5872745c2ee67e245329777b361ac6fc4c1ee5a738613f8af72d52")

            # create email string
            text =f"\nDear {data['name']}\n\nThe temporary password has been reset as requested to : {data['pass']}\n\nPlease contact support if any questions arises\nKind regards.\nDynamic Personal Insurance\n{data['date']}"
            msg = MIMEText(text)
            msg['Subject'] = 'Password Reset'
            msg['From'] = 'Dynamic Insurance App'
            msg['To'] =  data['email']

            #send email
            smtp.sendmail("dinamicinsuranceapp@gmail.com", data['email'],msg.as_string())
            smtp.quit()
            return "sent"
        return "error"

    except Exception as ex:
        return "error"


@app.route('/CompanyCode', methods=['POST'])
def check_Company_Code():

    try:
        data = request.get_json(force=True)  # Get data posted as a json
        if(data==None):
            return "error"
        c = 0
        for i in codeList:
            if(data["code"]==i and codeTaken[c]==False):
                return "ok"
            c+=1
        return "error"
    except Exception as ex:
        return "error"


@app.route('/predict', methods=['POST'])
def get_prediction():
    columns = ['Hospitals','Age','Cover','Hospital_Excess','Plan','Smoker']
    data = request.get_json(force=True)  # Get data posted as a json

    temp =[]
    for i in columns: temp.append(data[i])

    prediction = model.predict([temp])  # runs globally loaded model on the data
    return str(round(prediction[0],2))





