import socket
from threading import Thread
import json
import random

CLINET_HOST = "loopback"
CLIENT_PORT = 56595
ADDR = (CLINET_HOST, CLIENT_PORT)

BACKEND_HOST = "loopback"
BACKEND_PORT = 0  # 21257
BACKEND_ADDR = (BACKEND_PORT, BACKEND_HOST)

BUFFER_SIZE = 1024


# create model, controller files (python), define for each what will be inside (game logic/communication with backend)

def frontend_communication():
    while 1:
        try:
            command = backend_socket.recv(BUFFER_SIZE)
        except ConnectionResetError:  # 10054
            break
        if not command:
            break

        command = command.decode()
        if command in ["start", "create"]:
            client_socket.send(json.dumps({"type": "Start Game", "slow_game": True}).encode())
        elif command in ["restart", "reset"]:
            client_socket.send(json.dumps({"type": "Restart Game"}).encode())
        elif command.startswith("join"):
            client_socket.send(json.dumps({"type": "Join Game", "game_id": int(command.split()[1])}).encode())
        elif command in ["quit", "leave"]:
            client_socket.send(json.dumps({"type": "Quit Game"}).encode())
        elif command.startswith("login"):
            client_socket.send(json.dumps({"type": "Login", "name": command.split()[1]}).encode())
        elif command in ["logout"]:
            client_socket.send(json.dumps({"type": "Logout"}).encode())
        elif command in ["list", "lobbies", "showall"]:
            client_socket.send(json.dumps({"type": "Lobbies List"}).encode())

        else:
            client_socket.send(command.encode())


def server_communication():
    while 1:
        try:
            msg_length = int(client_socket.recv(5))
            data = json.loads(client_socket.recv(msg_length))
        except ConnectionResetError:
            print("ConnectionResetError in receive()")
            break
        except ConnectionAbortedError:
            print("ConnectionAbortedError in receive()")
            break
        if not data:
            break

        if data["type"] == "Board Update":
            print_board_state(data["board"])
            frontend_socket.send((data["board"] + " " + data["your turn"]).encode())
            print(data["board"] + " " + data["your turn"])
            print()
            if data["your turn"]:
                move()

        elif data["type"] == "Success":
            try:
                print("Game ID:", data["game_id"])
                frontend_socket.send(("ID " + data["game_id"]).encode())
            except:
                pass

        elif data["type"] == "Error":
            if data["errtype"] == "Invalid Name":
                print(data["data"])
            if data["errtype"] == "Invalid Move":
                move()

        elif data["type"] == "Game Over":
            wewon = data["won"]
            print(data["log"])
            if wewon:
                frontend_socket.send("WIN".encode())
            else:
                frontend_socket.send("LOSS".encode())
        else:
            print(data)


def move():
    client_socket.send(
        json.dumps({
            "type": "Game Move",
            "index": random.randint(1, 6)
        }).encode()
    )


def print_board_state(board):
    print("   ||| ", end="")
    for i in range(1, 7):
        print(board[i], end=" ||| ")
    print()

    print(" " + str(board[0]) + (7 + 6 * 6 - 2) * " " + str(board[7]))

    print("   ||| ", end="")
    for i in range(13, 7, -1):
        print(board[i], end=" ||| ")
    print()


backend_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
backend_socket.bind(BACKEND_ADDR)
backend_socket.listen(2)

print("Waiting for frontend connection...")
frontend_socket, addr = backend_socket.accept()
print("Frontend connected from: ", addr)

client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
while 1:
    try:
        print("Waiting for server connection...")
        client_socket.connect(ADDR)
        print("Connected to server successfully")
        break
    except ConnectionRefusedError:  # 10061
        print("ConnectionRefusedError")
        continue
    except TimeoutError:  # 10060
        print("TimeoutError")
        continue

handle_server_communication = Thread(target=server_communication)
handle_server_communication.start()

handle_frontend_communication = Thread(target=frontend_communication)
handle_frontend_communication.start()
