namespace ConsoleBreakOut
{
    public class MyBrick
    {
        const int MaxState = 10;
        public MyPoint Position { get; set; }
        public int RowIndex { get; set; }
        public int Width { get; }
        public int Height { get; }

        public int State { get; set; }

        public int FallIndex { get; set; }

        public MyBrick(int width, int height, MyPoint myPoint, int rowIndex = 1)
        {
            Width = width;
            Height = height;
            Position = myPoint;
            RowIndex = rowIndex;
            State = 0;
            FallIndex = 0;
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
        char topLeft = '\u250C';   // ┌
        char topRight = '\u2510';  // ┐
        char bottomLeft = '\u2514';// └
        char bottomRight = '\u2518';// ┘
        char horizontal = '\u2500'; // ─
        char vertical = '\u2502';   // │

        public void DrawToBuffer(MyBuffer myBuffer)
        {
            if (State == 0)
            {
                myBuffer.SetString((int)Position.X, (int)Position.Y, topLeft + new string(horizontal, Width - 2) + topRight, colors[RowIndex]);
                myBuffer.SetString((int)Position.X, (int)Position.Y + 1, bottomLeft + new string(horizontal, Width - 2) + bottomRight, colors[RowIndex]);
            }
            else
            {
                if (State == 1)
                {
                    myBuffer.SetString((int)Position.X, (int)Position.Y, @"\\\  ///", colors[0]);
                    myBuffer.SetString((int)Position.X, (int)Position.Y + 1, @"///  \\\", colors[2]);
                }
                //else if (State == 2)
                //{
                //    myBuffer.SetString((int)Position.X, (int)Position.Y, new string('*', Width), colors[1]);
                //    myBuffer.SetString((int)Position.X, (int)Position.Y + 1, "\\" + new string(' ', Width - 2) + "/", colors[0]);
                //}
                else
                {
                    ClearToBuffer(myBuffer);
                                
                    FallIndex++;
                    var spread = FallIndex * 2 + Width;
                    var startOffset = spread / 2;
                    var j = 0;
                    for (int i = (int)Position.X; i <= (int)Position.X + Width; i++)
                    {
                        myBuffer.SetChar((int)Position.X - startOffset + j * FallIndex, (int)Position.Y - FallIndex, '.', colors[1]);
                        myBuffer.SetChar((int)Position.X - startOffset + j * FallIndex, (int)Position.Y + FallIndex, '.', colors[1]);
                        j++;
                    }
                }
            }
        }

        public void ClearToBuffer(MyBuffer myBuffer)
        {
            for (int y = 0; y < Height; y++)
            {
                myBuffer.SetString((int)Position.X, (int)Position.Y + y, new string(' ', Width));
            }
            if (State > 1)
            {
                var spread = FallIndex * 2 + Width;
                var startOffset = spread / 2;
                var j = 0;
                for (int i = (int)Position.X; i <= (int)Position.X + Width; i++)
                {
                    myBuffer.SetChar((int)Position.X - startOffset + j * FallIndex, (int)Position.Y - FallIndex, ' ');
                    myBuffer.SetChar((int)Position.X - startOffset + j * FallIndex, (int)Position.Y + FallIndex, ' ');
                    j++;
                }
            }
        }

        public bool CheckCollusion(MyPoint ballPos)
        {
            if (ballPos.X >= Position.X && ballPos.X < Position.X + Width)
            {
                if (ballPos.Y >= Position.Y && ballPos.Y < Position.Y + Height)
                {
                    RowIndex = 0;
                    return true;
                }
            }

            return false;
        }

        public void SetHitState()
        {
            State = 1;
        }

        public bool CheckReadyToRemove()
        {
            if (State == 0) return false;
            if (State >= MaxState) return true;
            State++;
            return false;
        }
    }
}
