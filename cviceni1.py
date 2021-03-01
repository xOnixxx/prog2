from enum import Enum

class Position(Enum):
    LEFT = 0
    RIGHT = 1

def valid_bank(bank):
    mis, can = bank
    return can >= 0 and (mis == 0 or mis >= can)

def valid_state(state):
    left, _, right = state
    return valid_bank(left) and valid_bank(right)

def move(mis, can, start, end):
    s_mis, s_can = start
    e_mis, e_can = end
    return ((s_mis - mis, s_can - can), (e_mis + mis, e_can + can))

def next(state):
    left, boat, right = state
    new_states = []

    for mis, can in [(2,0), (1,0), (0,2), (0,1), (1,1)]:
        if boat == Position.LEFT:
            n_left, n_right = move(mis, can, left, right)
            new_state = (n_left, Position.RIGHT, n_right)
        else:
            n_right, n_left = move(mis, can, right, left)
            new_state = (n_left, Position.LEFT, n_right)

        if valid_state(new_state):
            new_states.append(new_state)

    return new_states

#############################
## State search algorithms ##
#############################

def bfs(start, end):
    queue = [[start]]

    while len(queue) > 0:
        current_path = queue.pop(0)
        state = current_path[-1]

        if state == end:
            return current_path

        for next_state in next(state):
            queue.append(current_path + [next_state])

def dfs(start, end):
    stack = [[start]]

    while len(stack) > 0:
        current_path = stack.pop()
        state = current_path[-1]

        if state == end:
            return current_path

        for next_state in next(state):
            stack.append(current_path + [next_state])

################################
## Iterative deepening search ##
################################

def ids_step(start, end, limit):
    '''
    A variant of depth first search that only searches paths
    which are shorther than the given limit.
    '''
    stack = [[start]]

    while len(stack) > 0:
        current_path = stack.pop()
        if len(current_path) > limit:
            continue

        state = current_path[-1]

        if state == end:
            return current_path

        for next_state in next(state):
            stack.append(current_path + [next_state])

def ids(start, end):
    '''
    Iterative deepening search performs DFS with an increasing limit
    until a solution is found.
    '''
    limit = 1
    while True:
        result = ids_step(start, end, limit)
        if result != None:
            return result

        limit += 1

################
## Heuristics ##
################

def dfs_visited(start, end):
    stack = [[start]]
    visited = set()
    searched = 0

    while len(stack) > 0:
        current_path = stack.pop()
        state = current_path[-1]
        searched += 1

        if state == end:
            print('States:', searched)
            return current_path

        if state in visited:
            continue
        else:
            visited.add(state)

        for next_state in next(state):
            stack.append(current_path + [next_state])

def value(state):
    '''
    Estimates how close to the solution the given state is.
    '''
    (l_mis, l_can), _, (r_mis, r_can) = state
    return r_mis + r_can - l_mis - l_can

def dfs_visited_heur(start, end):
    stack = [[start]]
    visited = set()
    searched = 0

    while len(stack) > 0:
        current_path = stack.pop()
        state = current_path[-1]
        searched += 1

        if state == end:
            print('States:', searched)
            return current_path

        if state in visited:
            continue
        else:
            visited.add(state)

        # Order the states by their value and put the state with the
        # highest value at the end. Because a stack is LIFO, it will be the
        # first state considered in the next step.
        for next_state in sorted(next(state), key=value):
            stack.append(current_path + [next_state])


start = ((3,3), Position.LEFT, (0,0))
end = ((0,0), Position.RIGHT, (3,3))

for step in dfs_visited_heur(start, end):
    print(step)
