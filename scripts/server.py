import socket
from threading import Thread
import json

server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.bind(("127.0.0.1", 7878))
server.listen(4)
clients = []
print("listening for connection")

def process_client_messages(clnt):
    clients.append(clnt)
    while True:
        try:
            data = clnt.recv(2048).decode()
            if data.startswith("play"):
                for client in clients:
                    if client != clnt:
                        client.sendall(data.encode())
        except:
            break

    print(f"{clnt} disconnected")
    clnt.close()

while True:
    try:
        clnt, addr = server.accept()
        ip = addr[0]
        print(f"Connected to {ip}")
        Thread(target=process_client_messages, args=(clnt,)).start()
    except ConnectionAbortedError:
        break
    
print("Couldn't establish connection")

