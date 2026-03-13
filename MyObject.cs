using ConsoleBreakOut;

public class MyObject
{
    public MyPoint Position { get; set; }

    public int Angle { get; set; }
    public List<MyObjectChar> Trail { get; set; }

    private readonly object _lock = new object();
    public int TrailLength { get; set; }

    public bool Drawing { get; set; }

    public MyObject(int x, int y, int trailLength = 10)
    {
        Drawing = false;
        Position = new MyPoint(x, y);
        Trail = new List<MyObjectChar>();
        TrailLength = trailLength;
        for (int i = 0; i < TrailLength; i++)
        {
            Trail.Add(new MyObjectChar(new MyPoint(x, y), '.'));
        }
    }

    public void AddTrailPoint(MyObjectChar point)
    {
        TrailLength++;
        Trail.Add(point);
        if (Trail.Count > TrailLength)
        {
            Trail.RemoveAt(0);
        }
    }
    
    static ConsoleColor[] greenFade3 = { 
            ConsoleColor.Green, 
            ConsoleColor.Green, 
            ConsoleColor.Green, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.DarkGray,
        };    

    static ConsoleColor[] greenFade2 = { 
            ConsoleColor.Red, 
            ConsoleColor.DarkMagenta,
            ConsoleColor.Yellow,
            ConsoleColor.DarkYellow, 
            ConsoleColor.Cyan,
            ConsoleColor.DarkCyan,
            ConsoleColor.Green, 
            ConsoleColor.DarkGreen, 
            ConsoleColor.Gray,
            ConsoleColor.DarkGray,
        };

    static string[] greenFade =
    {
        "\u001b[38;5;46m",
        "\u001b[38;5;46m",
        "\u001b[38;5;40m",
        "\u001b[38;5;40m",
        "\u001b[38;5;34m",
        "\u001b[38;5;34m",
        "\u001b[38;5;28m",
        "\u001b[38;5;28m",
        "\u001b[38;5;22m",
        "\u001b[38;5;22m",
        "\u001b[38;5;22m",
        "\u001b[38;5;0m"
    };

    static string[] greenFade4 =
    {
        "\u001b[38;5;66m",
        "\u001b[38;5;66m",
        "\u001b[38;5;60m",
        "\u001b[38;5;60m",
        "\u001b[38;5;54m",
        "\u001b[38;5;54m",
        "\u001b[38;5;48m",
        "\u001b[38;5;48m",
        "\u001b[38;5;42m",
        "\u001b[38;5;42m",
        "\u001b[38;5;42m",
        "\u001b[38;5;0m"
    };

    Random rnd = new Random();

    public void Move(MyPoint myPoint)
    {
        lock (_lock)
        {
            Position.X += myPoint.X;
            Position.Y += myPoint.Y;
        }
    }

    public void Draw()
    {
        lock (_lock)
        {
            Drawing = true;
            Console.SetCursorPosition((int)Trail[0].Position.X, (int)Trail[0].Position.Y);
            Console.Write(' ');

            Trail.RemoveAt(0);
            Trail.Add(new MyObjectChar(Position, (char)rnd.Next(32, 127)));
            int g = greenFade3.Length - 1;
            foreach (var point in Trail)
            {
                Console.SetCursorPosition((int)point.Position.X, (int)point.Position.Y);
                Console.ForegroundColor = greenFade3[g];
                Console.Write(point.Character);
                g--;
                if (g < 0) g = 0;
            }
            Console.ResetColor();
            Console.SetCursorPosition((int)Position.X, (int)Position.Y);
            Console.Write("O");
            //Thread.Sleep(50);
            Drawing = false;
        }
    }

    public void DrawToBuffer(MyBuffer myBuffer, bool dontClear = false)
    {
        Random random = new Random();
        lock (_lock)
        {            
            Drawing = true;
            if(!dontClear)
            myBuffer.SetChar((int)Trail[0].Position.X, (int)Trail[0].Position.Y, ' ', string.Empty);

            Trail.RemoveAt(0);
            Trail.Add(new MyObjectChar(Position, (char)rnd.Next(32, 127)));
            int g = greenFade.Length - 1;
            foreach (var point in Trail)
            {
                myBuffer.SetChar((int)point.Position.X, (int)point.Position.Y, point.Character, greenFade[g]);

                g--;
                if (g < 0) g = 0;
            }
            //myBuffer.SetChar((int)Position.X, (int)Position.Y, (char)960, "\u001b[97m");
            myBuffer.SetChar((int)Position.X, (int)Position.Y, 'O', "\u001b[97m");
            Drawing = false;
        }
    }

    public void DrawToBufferDiff(MyBuffer myBuffer, char character)
    {
        Random random = new Random();
        lock (_lock)
        {
            Drawing = true;

            Trail.RemoveAt(0);
            Trail.Add(new MyObjectChar(Position, character));
            int g = greenFade4.Length - 1;
            foreach (var point in Trail)
            {
                myBuffer.SetChar((int)point.Position.X, (int)point.Position.Y, 'o', greenFade[g]);

                g--;
                if (g < 0) g = 0;
            }
            myBuffer.SetChar((int)Position.X, (int)Position.Y, character, "\u001b[97m");
            Drawing = false;
        }
    }

}