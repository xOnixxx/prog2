using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        enum Tile
        {
            Empty,
            Cross,
            Circle
        }

        enum State
        {
            InProgress,
            Draw,
            CrossWins,
            CircleWins
        }

        class Board
        {
            private List<List<Tile>> tiles;

            public Board()
            {
                tiles = new List<List<Tile>>();
                for (int i = 0; i < 3; i++)
                {
                    tiles.Add(new List<Tile>() { Tile.Empty, Tile.Empty, Tile.Empty });
                }
            }

            public void Reset()
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        tiles[i][j] = Tile.Empty;
                    }
                }
            }

            public Tile Get(int x, int y)
            {
                return tiles[x][y];
            }

            public void Set(int x, int y, Tile t)
            {
                tiles[x][y] = t;
            }

            private bool CheckLine(Tile player, int x, int y, int dx, int dy)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (tiles[x + i * dx][y + i * dy] != player)
                    {
                        return false;
                    }
                }

                return true;
            }

            private bool CheckPlayer(Tile player)
            {
                for (int i = 0; i < 3; i++)
                {
                    // Rows
                    if (CheckLine(player, i, 0, 0, 1))
                    {
                        return true;
                    }

                    // Columns
                    if (CheckLine(player, 0, i, 1, 0))
                    {
                        return true;
                    }
                }

                // Diagonals
                return CheckLine(player, 0, 0, 1, 1) || CheckLine(player, 2, 0, -1, 1);
            }

            public State GameState()
            {
                if (CheckPlayer(Tile.Cross))
                {
                    return State.CrossWins;
                }

                if (CheckPlayer(Tile.Circle))
                {
                    return State.CircleWins;
                }

                foreach (var row in tiles)
                {
                    foreach (var tile in row)
                    {
                        if (tile == Tile.Empty)
                        {
                            return State.InProgress;
                        }
                    }
                }

                return State.Draw;
            }

            public void Draw()
            {
                var lines = new List<string>();
                foreach (var row in tiles)
                {
                    var line = new List<string>();
                    foreach (var tile in row)
                    {
                        string s;
                        switch (tile)
                        {
                            case Tile.Empty:
                                s = " ";
                                break;
                            case Tile.Cross:
                                s = "x";
                                break;
                            case Tile.Circle:
                                s = "o";
                                break;
                            default:
                                s = "";
                                break;
                        }
                        line.Add(s);
                    }
                    lines.Add(string.Join("|", line) + "\n");
                }
                Console.WriteLine(string.Join("-+-+-\n", lines));
            }
        }

        abstract class Player
        {
            public Tile tile { get; private set; }

            public Player(Tile t)
            {
                tile = t;
            }

            public abstract (int x, int y) NextMove(Board board);

            public virtual void Reset()
            { }
        }

        class MinMaxPlayer : Player
        {
            private (int x, int y) candidate;

            public MinMaxPlayer(Tile t) : base(t)
            { }

            public override (int x, int y) NextMove(Board board)
            {
                MinMax(board, tile, true);
                return candidate;
            }

            private int MinMax(Board board, Tile player, bool setCandidate = false)
            {
                switch (board.GameState())
                {
                    case State.Draw:
                        return 0;
                    case State.CrossWins:
                        return 1;
                    case State.CircleWins:
                        return -1;
                    default:
                        break;
                }

                int bestValue;
                int desiredValue;
                Tile otherPlayer;
                Func<int, int, int> op;

                if (player == Tile.Cross)
                {
                    bestValue = int.MinValue;
                    desiredValue = 1;
                    otherPlayer = Tile.Circle;
                    op = Math.Max;
                }
                else
                {
                    bestValue = int.MaxValue;
                    desiredValue = -1;
                    otherPlayer = Tile.Cross;
                    op = Math.Min;
                }

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board.Get(i, j) == Tile.Empty)
                        {
                            board.Set(i, j, player);
                            int newValue = op(bestValue, MinMax(board, otherPlayer));
                            board.Set(i, j, Tile.Empty);

                            if (newValue != bestValue)
                            {
                                if (setCandidate)
                                {
                                    candidate = (i, j);
                                }

                                if (newValue == desiredValue)
                                {
                                    return newValue;
                                }
                                else
                                {
                                    bestValue = newValue;
                                }
                            }
                        }
                    }
                }

                return bestValue;
            }

            public override void Reset()
            {
                base.Reset();
                candidate = (0, 0);
            }
        }

        class RandomPlayer : Player
        {
            protected Random rng;

            public RandomPlayer(Tile t) : base(t)
            {
                rng = new Random();
            }

            public override (int x, int y) NextMove(Board board)
            {
                int x;
                int y;

                do
                {
                    x = rng.Next(0, 3);
                    y = rng.Next(0, 3);
                }
                while (board.Get(x, y) != Tile.Empty);

                return (x, y);
            }
        }

        class BetterRandomPlayer : RandomPlayer
        {
            public BetterRandomPlayer(Tile t) : base(t)
            { }

            public override (int x, int y) NextMove(Board board)
            {
                var free = new List<(int, int)>();

                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (board.Get(i, j) == Tile.Empty)
                        {
                            free.Add((i, j));
                        }
                    }
                }

                return free[rng.Next(free.Count)];
            }
        }

        class HumanPlayer : Player
        {
            public HumanPlayer(Tile t) : base(t)
            { }

            public override (int x, int y) NextMove(Board board)
            {
                while (true)
                {
                    string[] input = Console.ReadLine().Split(null as char[], StringSplitOptions.RemoveEmptyEntries);
                    try
                    {
                        int x = int.Parse(input[0]);
                        int y = int.Parse(input[1]);

                        if (x < 0 || x > 3 || y < 0 || y > 3)
                        {
                            throw new Exception("Number is not in the specified range");
                        }

                        if (board.Get(x, y) != Tile.Empty)
                        {
                            throw new Exception("Tile is not empty");
                        }

                        return (x, y);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        class Game
        {
            private Player player1;
            private Player player2;
            private Board board;

            private int turn;

            public Game(Player p1, Player p2)
            {
                player1 = p1;
                player2 = p2;
                board = new Board();
                turn = 0;
            }

            public void Reset()
            {
                player1.Reset();
                player2.Reset();
                board.Reset();
                turn = 0;
            }

            public void Play()
            {
                Reset();

                while (true)
                {
                    board.Draw();
                    switch (board.GameState())
                    {
                        case State.Draw:
                            Console.WriteLine("Draw!");
                            return;
                        case State.CrossWins:
                            Console.WriteLine("Cross wins!");
                            return;
                        case State.CircleWins:
                            Console.WriteLine("Circle wins!");
                            return;
                        default:
                            break;
                    }

                    Player p = turn++ % 2 == 0 ? player1 : player2;
                    Console.WriteLine($"Turn {turn}: {(p.tile == Tile.Cross ? "x" : "o")}");

                    var (x, y) = p.NextMove(board);
                    board.Set(x, y, p.tile);
                }
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Please select an opponent (minmax/random/human).");
            Player opponent = null;
            while (opponent == null)
            {
                switch (Console.ReadLine())
                {
                    case "minmax":
                        opponent = new MinMaxPlayer(Tile.Circle);
                        break;
                    case "random":
                        opponent = new BetterRandomPlayer(Tile.Circle);
                        break;
                    case "human":
                        opponent = new HumanPlayer(Tile.Circle);
                        break;
                    default:
                        break;
                }
            }

            Game g = new Game(new HumanPlayer(Tile.Cross), opponent);
            string resp;
            do
            {
                g.Play();
                Console.WriteLine("Would you like to play again?");
                resp = Console.ReadLine();
            }
            while (resp == "y");
        }
    }
}
