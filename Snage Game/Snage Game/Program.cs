using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    class Program
    {
        static int width = 40; // Doubled width
        static int height = 20;
        static int score = 0;
        static int speedLevel = 3; // Default speed level
        static int wallHitsRemaining = 5; // Wall hit chances
        static bool gameOver;
        static bool paused;
        static ConsoleKey direction = ConsoleKey.RightArrow;

        static List<(int x, int y)> snake = new List<(int, int)>();
        static (int x, int y) food = (0, 0);
        static Random random = new Random();

        static void Main()
        {
            Console.CursorVisible = false;
            MainMenu();

            while (!gameOver)
            {
                if (!paused)
                {
                    Draw();
                    Input();
                    Logic();
                }
                Thread.Sleep(200 - (speedLevel * 30)); // Adjust speed based on speed level
            }

            EndGame();
        }

        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("=== SNAKE GAME ===");
            Console.WriteLine("1. New Game");
            Console.WriteLine("2. Speed Setting");
            Console.WriteLine("3. Exit");
            Console.WriteLine("Choose an option (1-3):");

            switch (Console.ReadKey(true).Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    StartGame();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    SpeedSetting();
                    MainMenu();
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Environment.Exit(0);
                    break;
                default:
                    MainMenu();
                    break;
            }
        }

        static void SpeedSetting()
        {
            Console.Clear();
            Console.WriteLine("Choose Speed Level (1-5):");

            var key = Console.ReadKey(true).Key;
            if (key >= ConsoleKey.D1 && key <= ConsoleKey.D5)
            {
                speedLevel = (int)key - (int)ConsoleKey.D0;
                Console.WriteLine($"Speed level set to {speedLevel}");
                Thread.Sleep(1000);
            }
        }

        static void StartGame()
        {
            gameOver = false;
            paused = false;
            score = 0;
            wallHitsRemaining = 5; // Reset wall hit chances
            direction = ConsoleKey.RightArrow;
            snake.Clear();
            snake.Add((width / 2, height / 2)); // Initial snake position
            GenerateFood();
        }

        static void Draw()
        {
            Console.Clear();

            for (int i = 0; i < width + 2; i++)
                Console.Write("#");
            Console.WriteLine();

            for (int y = 0; y < height; y++)
            {
                Console.Write("#");
                for (int x = 0; x < width; x++)
                {
                    if (snake[0].x == x && snake[0].y == y)
                        Console.Write("O"); // Head of snake
                    else if (snake.Any(s => s.x == x && s.y == y))
                        Console.Write("o"); // Body of snake
                    else if (food.x == x && food.y == y)
                        Console.Write("X"); // Food
                    else
                        Console.Write(" ");
                }
                Console.WriteLine("#");
            }

            for (int i = 0; i < width + 2; i++)
                Console.Write("#");
            Console.WriteLine();
            Console.WriteLine($"Score: {score}");
            Console.WriteLine($"Wall Hits Remaining: {wallHitsRemaining}");
            Console.WriteLine("Press 'P' to Pause/Resume");
        }

        static void Input()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.P)
                {
                    paused = !paused;
                }
                else if (!paused)
                {
                    if (key == ConsoleKey.UpArrow && direction != ConsoleKey.DownArrow)
                        direction = ConsoleKey.UpArrow;
                    else if (key == ConsoleKey.DownArrow && direction != ConsoleKey.UpArrow)
                        direction = ConsoleKey.DownArrow;
                    else if (key == ConsoleKey.LeftArrow && direction != ConsoleKey.RightArrow)
                        direction = ConsoleKey.LeftArrow;
                    else if (key == ConsoleKey.RightArrow && direction != ConsoleKey.LeftArrow)
                        direction = ConsoleKey.RightArrow;
                }
            }
        }

        static void Logic()
        {
            (int x, int y) newHead = snake[0];

            switch (direction)
            {
                case ConsoleKey.UpArrow: newHead.y--; break;
                case ConsoleKey.DownArrow: newHead.y++; break;
                case ConsoleKey.LeftArrow: newHead.x--; break;
                case ConsoleKey.RightArrow: newHead.x++; break;
            }

            // Check if snake hits the wall
            if (newHead.x < 0 || newHead.x >= width || newHead.y < 0 || newHead.y >= height)
            {
                wallHitsRemaining--;
                if (wallHitsRemaining >= 0)
                {
                    // Wrap around to the opposite side
                    if (newHead.x < 0) newHead.x = width - 1;
                    else if (newHead.x >= width) newHead.x = 0;
                    if (newHead.y < 0) newHead.y = height - 1;
                    else if (newHead.y >= height) newHead.y = 0;
                }
                else
                {
                    gameOver = true;
                    return;
                }
            }

            // Check if snake hits itself
            if (snake.Contains(newHead))
            {
                gameOver = true;
                return;
            }

            snake.Insert(0, newHead);

            if (newHead == food)
            {
                score += 10;
                GenerateFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1); // Remove last segment if not eating food
            }
        }

        static void GenerateFood()
        {
            do
            {
                food = (random.Next(0, width), random.Next(0, height));
            } while (snake.Contains(food));
        }

        static void EndGame()
        {
            Console.Clear();
            Console.WriteLine("Game Over!");
            Console.WriteLine($"Final Score: {score}");
            Console.WriteLine("Do you want to play again? (Y/N)");

            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Y)
                MainMenu();
            else
                Environment.Exit(0);
        }
    }
}
