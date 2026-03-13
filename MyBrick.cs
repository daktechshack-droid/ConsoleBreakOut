namespace ConsoleBreakOut
{
    public class MyBrick
    {
        public MyPoint Position { get; set; }
        public int RowIndex { get; }
        public int Width { get; }
        public int Height { get; }

        public MyBrick(int width, int height, MyPoint myPoint, int rowIndex = 0)
        {
            Width = width;
            Height = height;
            Position = myPoint;
            RowIndex = rowIndex;
        }

        static string[] colors =
        {
            "\u001b[38;5;196m", // Bright Red
            "\u001b[38;5;208m", // Orange
            "\u001b[38;5;226m", // Yellow
            "\u001b[38;5;46m",  // Bright Green
            "\u001b[38;5;45m",  // Turquoise
            "\u001b[38;5;27m",  // Royal Blue
            "\u001b[38;5;129m", // Deep Purple
            "\u001b[38;5;201m", // Hot Pink
            "\u001b[38;5;51m",  // Cyan
            "\u001b[38;5;214m", // Gold
            "\u001b[38;5;118m", // Lime
            "\u001b[38;5;15m"   // Pure White
        };

        //public void Move(MyPoint myPoint)
        //{
        //    Position.X += myPoint.X;
        //    Position.Y += myPoint.Y;
        //}

        public void DrawToBuffer(MyBuffer myBuffer)
        {
            //Random random = new Random();
            //var colorId = random.Next(colors.Length);
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    myBuffer.SetChar((int)Position.X + x, (int)Position.Y + y, '#', colors[RowIndex]);
                }
            }
        }

        public bool CheckCollusion(MyPoint ballPos)
        {
            if (ballPos.X >= Position.X && ballPos.X < Position.X + Width)
            {
                if (ballPos.Y >= Position.Y && ballPos.Y < Position.Y + Height)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
