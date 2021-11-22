import socket
from threading import Thread
import json
import random

HOST = "loopback"  # "109.65.31.250"  # "79.179.71.212"
PORT = 49794  # 45000

BUFSIZ = 1024
ADDR = (HOST, PORT)


# create model, controller files (python), define for each what will be inside (game logic/communication with backend)
# create C# view file
# think about what happens with the view while the bot is playing, does it slow down?
# start writing the algorithm, save the random one and see if it starts beating and how much

def receive():
    while 1:
        try:
            msg_length = int(client_socket.recv(5))  # is BUFSIZ critical here?
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
            print(data["your turn"])
            print()
            if data["your turn"]:
                move()

        elif data["type"] == "Success":
            try:
                print("Game ID:", data["game_id"])
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
                print("yay!")
            else:
                print("lost :(")
        else:
            print(data)


def user_send():
    while 1:
        command = input("> ").lower()
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


def send(data):
    client_socket.send(data.encode())


def login():
    send(json.dumps({"type": "Login", "name": input("name --> ")}))


def move():
    send(
        json.dumps({
            "type": "Game Move",
            "index": random.randint(1, 6)
        })
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


# print_board_state([0, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4])
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

while 1:
    try:
        print("Waiting for connection...")
        client_socket.connect(ADDR)
        print("Connected to server successfully")
        break
    except ConnectionRefusedError:  # 10061
        print("ConnectionRefusedError")
        continue
    except TimeoutError:  # 10060
        print("TimeoutError")
        continue

rec = Thread(target=receive)
rec.start()

u_send = Thread(target=user_send)
u_send.start()
