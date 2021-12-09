"""
Backend side of bot for mancala game (bot logic, communication with game server, communication with frontend)
Nov-Dec 2021
Yegor Stolyarsky, Ben Solomovitch
"""

import socket
from threading import Thread
import json  # format of messages defined by game server
import random
import time  # for testing of bot performance
import pickle  # for saving a tree object in .pkl format

CLINET_HOST = "192.168.1.15"
CLIENT_PORT = 58880
ADDR = (CLINET_HOST, CLIENT_PORT)

BACKEND_HOST = "loopback"
BACKEND_PORT = 21257
BACKEND_ADDR = (BACKEND_HOST, BACKEND_PORT)

# random number bounds for value of number in tree
RANDOM_NUMBER_MIN = 0
RANDOM_NUMBER_MAX = 13  # not sure

BUFFER_SIZE = 1024


# create model, controller files (python), define for each what will be inside (game logic/communication with backend)

def send_to_frontend(data):
    """
    Sends data to frontend socket, with its length preceding it
    :param data: str with the data to send
    :return: None
    """
    frontend_socket.send(str(len(data)).zfill(5).encode())
    frontend_socket.send(data.encode())


class Node:  # use _ before variables!
    """
    Used to represent a program.
    """

    def __init__(self, data):
        self.nodes = []
        self.data = data

    def insert(self, data):
        """
        Adds a child to the node.
        :param data: The value the new node will get.
        :return: None
        """
        self.nodes.append(Node(data))

    def print_tree(self):
        """
        Prints a program.
        :return: None
        """
        print(self.data + " [" + str(len(self.nodes)) + "]")
        for node in self.nodes:
            node.print_tree()

    def node_amount(self):
        """
        Counts the amount of nodes in the tree.
        :return: int of the amount
        """
        return 1 + sum([node.node_amount() for node in self.nodes])

    def depth(self):  # not including itself as a layer
        """
        The length of the longest child.
        :return: int
        """
        if not self.nodes:
            return 0
        return 1 + max([node.depth() for node in self.nodes])

    def random_node(self):  # might result in too small trees??? maybe do it with weights to weight more smaller ones
        """
        Chooses a "random" sub-tree in a tree.
        :return: The sub-tree (Node type)
        """
        r = random.randint(0, self.depth())
        tree = self
        while r > 0:
            if tree.nodes:
                k = len(tree.nodes)
                tree = tree.nodes[random.randint(0, k - 1)]
                r -= k
            else:
                # print("Chosen Random Tree:")
                # tree.print_tree()
                return tree
        # tree.print_tree()
        return tree

    def replace_random_node(self, replacing_tree=""):  # default is replacing with random tree
        """
        Chooses a "random" sub-tree and replaces it with another tree.
        :param replacing_tree: a Node that will replace a random tree, default is random.
        :return: None
        """
        node_to_replace = self.random_node()
        if replacing_tree == "":
            replacing_tree = random_tree(node_to_replace.depth())

        node_to_replace.data = node_to_replace.data
        node_to_replace.nodes = node_to_replace.nodes


def random_function(function_set):
    function = random.choice(function_set)

    if function == "value_at":
        pass
    elif function == "random_number":
        function = str(random.randint(RANDOM_NUMBER_MIN, RANDOM_NUMBER_MAX + 1))  # what is the range?

    return function


def random_tree(depth):  # random type and then a random number, not vice versa (?) / make option for blank node?
    function_set = ["+", "-", "value_at", "random_number"]
    tree = Node(random_function(function_set))
    depth -= 1

    return random_tree_fill(tree, depth)


def random_tree_fill(tree, depth):
    """
    Fills a tree with functions up to a given depth.
    :param tree: the tree to fill
    :param depth: the depth of the final tree
    :return: Node that holds the tree
    """
    d = depth
    if d > 0 and not tree.data.isnumeric():
        if depth == 1:
            function_set = ["random_number"]
            tree.insert(random_function(function_set))
            if tree.data in ["+", "-"]:
                tree.insert(random_function(function_set))

        elif tree.data in ["+", "-"]:
            function_set = ["+", "-", "value_at", "random_number"]
            tree.insert(random_function(function_set))
            tree.insert(random_function(function_set))
        else:
            function_set = ["+", "-", "value_at", "random_number"]
            tree.insert(random_function(function_set))

        for node in tree.nodes:
            # print(d)
            random_tree_fill(node, d - 1)

    return tree


def random_population(size, depth):
    return [random_tree(depth) for _ in range(size)]


def parse_program(program, board):
    """
    Translates a program into the hole it represents
    :param program: The program to parse (Node)
    :param board: The current board state (list of int)
    :return: int with the choice of the program
    """
    if program.data.isnumeric():
        return int(program.data)
    elif program.data == "+":
        return parse_program(program.nodes[0], board) + parse_program(program.nodes[1], board)
    elif program.data == "-":
        return parse_program(program.nodes[0], board) - parse_program(program.nodes[1], board)
    elif program.data == "value_at":
        return value_at(parse_program(program.nodes[0], board), board)


def value_at(choice, board):
    return board[abs(choice) % 14]


def move_is_valid(choice, board):
    # print(board[choice])
    return 1 <= choice <= 6 and board[choice] != 0


def valid_moves(board):
    return [i for i in range(1, 7) if board[i] != 0]


def simulation_move(board, choice):
    """
    Does one move in a simulation.
    :param board: Current board state
    :param choice: The hole chosen for the move
    :return: New board state (list of int), whether the same side should move again (bool)
    """
    same_move = False
    i = choice
    amount = board[choice]
    board[choice] = 0

    while amount != 0:
        i = (i - 1) % 14  # move to next hole

        # skip your mancala
        if 1 <= choice <= 6:
            if i == 7:
                i = 6
        elif 8 <= choice <= 13:
            if i == 0:
                i = 13
        else:
            print("Problem in simulation_move()")

        amount -= 1
        board[i] += 1

    if i == 0 or i == 7:  # if finished in your mancala you get another move
        same_move = True

    elif board[i] == 1:  # if there is only one stone in the last hole - it was empty and the player finished there
        if 1 <= choice <= 6:
            board[0] += board[14 - i]
        elif 8 <= choice <= 13:
            board[7] += board[14 - i]
        else:
            print("Problem in simulation_move()")
        board[14 - i] = 0

    # if no stones on one side
    if sum(board[1:7]) == 0:
        board[0] += sum(board[8:14])
        for i in range(8, 14):
            board[i] = 0
    elif sum(board[8:14]) == 0:
        board[7] += sum(board[1:7])
        for i in range(1, 7):
            board[i] = 0

    return board, same_move


def simulation(program_tree, board, program_moves):
    """
    Runs a simulation game of a program_tree against a random bot
    :param program_tree: Node, a program that is being tested
    :param board: list of int, starting position
    :param program_moves: bool, whether the program_tree moves first
    :return: The number of stones that the program_tree has at the end of the game
    """

    if valid_moves(board) and valid_moves(board[7:] + board[:7]):  # if game hasn't finished yet
        if program_moves:
            choice = parse_program(program_tree, board)
            if choice not in valid_moves(board):
                choice = abs(choice) % 7
                while choice not in valid_moves(board):
                    choice = (choice + 1) % 7  # question!!
        else:
            choice = 7 + bot_move(board[7:] + board[:7])
        next_move = simulation_move(board, choice)
        if next_move[1]:  # if same side moves again
            next_side = program_moves
        else:
            next_side = not program_moves
        return simulation(program_tree, next_move[0], next_side)  # recursive advancement of the simulation
    else:
        return board[0]


def fitness(board_state, population):
    """
    Calculates for each program in a population how fit it is.
    :param board_state: list of int, current state of the board
    :param population: list of Node, the population to evaluate
    :return: list of (program, score) tuples
    """
    fitness_list = []
    for program in population:
        score = 0

        # board = board_state.copy()
        # score += simulation(program, board, True)

        # board = board_state.copy()
        # score += simulation(program, board, False)

        # score = 100 * score - program.depth()
        results = benchmark(program, 101)
        score = results  # * 100 - program.node_amount()
        fitness_list.append((program, score))

    return fitness_list


def evolve(board_state, generations, population_size, program_depth):
    """
    Evolve for several generations.
    :param board_state: list of int, current board state
    :param generations: int, number of generations to be run
    :param population_size: int, amount of programs in each generation
    :param program_depth: int, depth of each program in a generation
    :return: sorted list of (program, fitness) tuples (from best fitness to worst)
    """
    population = random_population(population_size, program_depth)
    population_fitness = fitness(board_state, population)
    next_population = []
    for i in range(generations):
        print("Starting Gen" + str(i))
        next_population.append(max(population_fitness, key=lambda x: x[1])[0])  # elitism
        fill_population(next_population, population_fitness, population_size)  # might not work because of pointers???
        population_fitness = fitness(board_state, next_population)
        population = next_population
        next_population = []
    print("finished evolving")

    return sorted(population_fitness, key=lambda x: x[1], reverse=True)


def fill_population(next_population, population_fitness, population_size):
    population_programs = [i[0] for i in population_fitness]  # get these as separate params? maybe not
    population_weights = [i[1] for i in population_fitness]

    next_population.extend(
        random.choices(
            population=population_programs,
            weights=population_weights,
            # cum_weights=[],
            k=population_size - len(next_population)
        )
    )

    for program in next_population:
        if random.random() < 90 / 100:
            previous_generation_program = random.choices(population=population_programs,
                                                         weights=population_weights,
                                                         k=1)[0]
            crossover(program, previous_generation_program)  # might not work because of pointers???
        if random.random() < 0.5 / 100:
            mutation(program)


def crossover(program, previous_generation_program):
    program.replace_random_node(previous_generation_program.random_node())


def mutation(program):
    program.replace_random_node()


def frontend_communication():
    """
    Handles communication with frontend, meant to be run in a thread.
    :return: None
    """
    print("FRONTEND COMMUNICATION STARTED")
    while 1:
        try:
            command = frontend_socket.recv(BUFFER_SIZE)
            print("command received is:", command)
        except ConnectionResetError:  # 10054
            break
        if not command:
            break

        print(command)
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
            print("logged in!!!")

        elif command in ["logout"]:
            client_socket.send(json.dumps({"type": "Logout"}).encode())

        elif command in ["list", "lobbies", "showall"]:
            client_socket.send(json.dumps({"type": "Lobbies List"}).encode())

        else:
            print("ELSE IN FRONTEND")
            # send_to_frontend(command)


def server_communication():
    """
    Handles communication with the game server, meant to be run in a thread.
    :return: None
    """
    while 1:
        try:
            msg_length = int(client_socket.recv(5))
            data = json.loads(client_socket.recv(msg_length))
            print(data)
        except ConnectionResetError:
            print("ConnectionResetError in receive()")
            break
        except ConnectionAbortedError:
            print("ConnectionAbortedError in receive()")
            break
        if not data:
            break

        t0 = time.time()
        if data["type"] == "Board Update":
            print_board_state(data["board"])
            send_to_frontend((str(data["board"]) + " " + str(data["your turn"])))
            print(data["board"], data["your turn"])
            print()
            if data["your turn"]:
                move(data["board"])
                print(time.time() - t0)

        elif data["type"] == "Success":
            try:
                print("Game ID:", data["game_id"])
                send_to_frontend(("ID " + str(data["game_id"])))
            except:
                pass

        elif data["type"] == "Error":
            if data["errtype"] == "Invalid Name":
                print(data["data"])
            if data["errtype"] == "Invalid Move":
                print(data)
                print("Got error of invalid move, unable to make move because no board state was delivered")
                # move()
                print(time.time() - t0)

        elif data["type"] == "Game Over":
            wewon = data["won"]
            print(data["log"])
            if wewon:
                send_to_frontend("WIN")
            else:
                send_to_frontend("LOSS")
        else:
            print(data)


def bot_move(board_state):
    """
    Bot that is used in simulation().
    :param board_state: list of int, current board state in which the bot plays
    :return: int, hole that the bot chooses
    """
    return random.choice(valid_moves(board_state))


def move(board_state):
    """
    Chooses a move and sends it to the game server.
    :param board_state: list of int, current board state
    :return: None
    """
    # for dynamic runs:
    # t_s = time.time()
    # gen = evolve(initial_board_state, 10, 64, 8)
    # print(time.time() - t_s)
    # best_tree = gen[0][0]

    # static tree load:
    choice = parse_program(load_tree(), initial_board_state)
    if choice not in valid_moves(board_state):
        choice = abs(choice) % 7
        while choice not in valid_moves(board_state):
            choice = (choice + 1) % 7  # question!! Maybe if choice not valid set fitness to 0?

    client_socket.send(
        json.dumps({
            "type": "Game Move",
            "index": choice
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


def benchmark(program, runs):
    score = 0
    for i in range(runs):
        score += simulation(program, [0, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4], True)
    for i in range(runs):
        score += simulation(program, [0, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4], False)
    return score


def evolve_tree_and_save():
    t_s = time.time()
    gen = evolve(initial_board_state, 10, 128, 8)
    print(time.time() - t_s)
    best_tree = gen[0][0]
    best_tree.print_tree()
    with open("b_t.pkl", "wb") as f:
        pickle.dump(best_tree, f, pickle.HIGHEST_PROTOCOL)


def load_tree():
    with open("b_t.pkl", "rb") as f:
        best_tree = pickle.load(f)
    # best_tree.print_tree()
    return best_tree


initial_board_state = [0, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4]

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

'''
# runs a simulation, counts and prints (wins/draws/losses) against random
best_tree = load_tree()
wins = 0
draws = 0
losses = 0
for i in range(100000):
    r = simulation(best_tree, [0, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4], True)
    if r > 24:
        wins += 1
    elif r == 24:
        draws += 1
    else:
        losses += 1
print(wins, draws, losses)
wins, draws, losses = (0, 0, 0)
for i in range(100000):
    r = simulation(best_tree, [0, 4, 4, 4, 4, 4, 4, 0, 4, 4, 4, 4, 4, 4], False)
    if r > 24:
        wins += 1
    elif r == 24:
        draws += 1
    else:
        losses += 1
print(wins, draws, losses)
'''

# keeps parent thread running
while 1:
    pass
