FROM python:3-slim
COPY ./app.py /deploy/
COPY ./requirements.txt /deploy/
COPY ./planModel /deploy/
WORKDIR /deploy/
RUN pip install -r requirements.txt
EXPOSE 80
ENTRYPOINT ["python", "app.py"]