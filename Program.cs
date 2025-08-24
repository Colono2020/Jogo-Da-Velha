//Alunos: Alison Civiero - 451219 e Jhonatan da Silva e Witor Daniel
using System;
using System.Linq;

class ExplosiveTicTacToe
{
    static char[] board = new char[9];
    static char current = 'X';
    static int scoreX = 0, scoreO = 0;

    // Explosões
    static int turnCount = 0;            // jogadas válidas já feitas
    static int lastMoveIndex = -1;       // índice da última jogada válida (0-8)
    static readonly Random rng = new Random();

    static void Main()
    {
        ResetBoard();
        while (true)
        {
            Console.Clear();
            PrintScore();
            PrintBoard();

            // fim da partida?
            if (HasWinner(out char w))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nPlayer {w} Ganhou!");
                Console.ResetColor();

                if (PlayAgain())
                {
                    if (w == 'X') scoreX++; else scoreO++;
                    ResetBoard();
                    continue;
                }
                break;
            }
            if (IsDraw())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\nDeu Empate.");
                Console.ResetColor();

                if (PlayAgain())
                {
                    ResetBoard();
                    continue;
                }
                break;
            }

            // input
            Console.Write($"\nPlayer {current}, choose a position (1-9): ");
            var input = Console.ReadLine();

            if (!int.TryParse(input, out int pos) || pos < 1 || pos > 9)
            {
                Warn("Invalid input. Use a number from 1 to 9.");
                continue;
            }
            if (board[pos - 1] != ' ')
            {
                Warn("That cell is already taken.");
                continue;
            }

            // aplica jogada
            board[pos - 1] = current;
            lastMoveIndex = pos - 1;
            turnCount++;

            // regra explosiva: a cada 3 jogadas válidas, explode 1 casa ocupada aleatória (exceto a jogada recém-feita)
            if (turnCount % 3 == 0)
            {
                TriggerExplosion();
            }

            // troca jogador
            current = (current == 'X') ? 'O' : 'X';
        }

        Console.WriteLine("\nJogo Acabou. Obrigado por jogar!");
    }

    // ===== Explosões =====
    static void TriggerExplosion()
    {
        var candidates = Enumerable.Range(0, 9)
            .Where(i => board[i] != ' ' && i != lastMoveIndex) // não explode a jogada recém-feita
            .ToList();

        if (candidates.Count == 0)
            return; // nada para explodir

        int idx = candidates[rng.Next(candidates.Count)];
        char removed = board[idx];
        board[idx] = ' ';

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"\n💥 Explosion! Cell {idx + 1} lost a '{removed}'.");
        Console.ResetColor();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    // ===== UI =====
    static void PrintScore()
    {
        Console.WriteLine("===== SCOREBOARD =====");
        Console.ForegroundColor = ConsoleColor.Red;  Console.Write("Player X: "); Console.ResetColor(); Console.WriteLine(scoreX);
        Console.ForegroundColor = ConsoleColor.Blue; Console.Write("Player O: "); Console.ResetColor(); Console.WriteLine(scoreO);
        Console.WriteLine("======================");
        Console.Write("Current: ");
        if (current == 'X') { Console.ForegroundColor = ConsoleColor.Red;  Console.WriteLine("X"); }
        else                { Console.ForegroundColor = ConsoleColor.Blue; Console.WriteLine("O"); }
        Console.ResetColor();
    }

    static void PrintBoard()
    {
        Console.WriteLine("\nUse 1-9 to place:\n");
        for (int r = 0; r < 3; r++)
        {
            int i = r * 3;
            Console.Write(" ");
            PrintCell(i);   Console.Write(" | ");
            PrintCell(i+1); Console.Write(" | ");
            PrintCell(i+2);
            Console.WriteLine();
            if (r < 2) Console.WriteLine("---+---+---");
        }
    }

    static void PrintCell(int index)
    {
        char c = board[index];
        if (c == 'X')
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("X");
        }
        else if (c == 'O')
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("O");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(index + 1);
        }
        Console.ResetColor();
    }

    static void Warn(string msg)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(msg);
        Console.ResetColor();
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }

    // ===== Lógica do jogo =====
    static void ResetBoard()
    {
        for (int i = 0; i < 9; i++) board[i] = ' ';
        current = 'X';
        turnCount = 0;
        lastMoveIndex = -1;
    }

    static bool IsDraw()
    {
        for (int i = 0; i < 9; i++)
            if (board[i] == ' ') return false;
        return !HasWinner(out _);
    }

    static bool HasWinner(out char winner)
    {
        int[,] lines = new int[,] {
            {0,1,2},{3,4,5},{6,7,8},
            {0,3,6},{1,4,7},{2,5,8},
            {0,4,8},{2,4,6}
        };
        for (int i = 0; i < 8; i++)
        {
            int a = lines[i,0], b = lines[i,1], c = lines[i,2];
            if (board[a] != ' ' && board[a] == board[b] && board[b] == board[c])
            {
                winner = board[a];
                return true;
            }
        }
        winner = '\0';
        return false;
    }

    static bool PlayAgain()
    {
        Console.Write("\nJogar Novamente? (y/n): ");
        var key = Console.ReadKey().KeyChar;
        Console.WriteLine();
        return char.ToLowerInvariant(key) == 'y';
    }
}
