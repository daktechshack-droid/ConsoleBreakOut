
using ConsoleBreakOut;

public static class BreakOutGameBuffer
{
    public static void Start(int screenWidth = 80, int screenHeight = 25, bool soundOn = false)
    {
        var myBuffer = new MyBuffer(screenWidth, screenHeight);
        var myObject = new MyObject(screenWidth / 2, screenHeight * 3 / 5, 3);
        var ballPos = myObject.Position;
        var direction = new MyPoint(1, 1);
        var lastBallPos = new MyPoint(ballPos);
        var batPos = new MyPoint(screenWidth / 2, screenHeight - 2);
        var batLast = new MyPoint(batPos);
        var score = 0;
        var batLength = 12;
        var batSpeed = 8;
        float speedX = 1f;
        float speedY = 1f;
        bool WinState = false;

        var bricks = new List<MyBrick>();
        var numBrickPerWidth = screenWidth / 10 + 2;
        CreateBrickRow(bricks, 0, 2, numBrickPerWidth, 1);
        CreateBrickRow(bricks, 4, 4, numBrickPerWidth, 2);
        CreateBrickRow(bricks, 0, 6, numBrickPerWidth, 3);
        CreateBrickRow(bricks, 4, 8, numBrickPerWidth, 4);
        CreateBrickRow(bricks, 0, 10, numBrickPerWidth, 5);
        CreateBrickRow(bricks, 4, 12, numBrickPerWidth, 6);

        myBuffer.Clear();
        Console.CursorVisible = false;
        myBuffer.WriteCentered(screenHeight - 1, "Pong Game - Press X to exit", screenWidth, string.Empty);
        while (true)
        {
            myBuffer.SetChar((int)lastBallPos.X, (int)lastBallPos.Y, ' ');

            Thread.Sleep(50);
            if (ballPos.X > screenWidth - 2)
            {
                speedX = 1f;
                direction.X = -speedX;
                if (soundOn) Task.Run(() => Console.Beep(440, 500));
            }
            //if (ballPos.Y > screenHeight - 2) direction.Y = -speed;

            if (ballPos.X < 1)
            {
                speedX = 1f;
                direction.X = speedX;
                if (soundOn) Task.Run(() => Console.Beep(440, 500));
            }
            if (ballPos.Y < 1)
            {
                speedY = 1f;
                direction.Y = speedY;
                if (soundOn) Task.Run(() => Console.Beep(440, 500));
            }

            lastBallPos = new MyPoint(ballPos);

            foreach (var b in bricks)
            {
                if (b.CheckReadyToRemove())
                {
                    b.ClearToBuffer(myBuffer);
                    bricks.Remove(b);
                    break;
                }
            }

            if (bricks.Count == 0)
            {
                WinState = true;
            }

            foreach (var b in bricks)
            {
                if (b.CheckCollusion(ballPos) && b.State == 0)
                {
                    direction.Y = -direction.Y;
                    speedY += 0.1f;
                    speedX += 0.1f;
                    speedY = Math.Min(speedY, 1.8f);
                    speedX = Math.Min(speedX, 1.8f);
                    direction.X = Math.Sign(direction.X) * speedX;
                    direction.Y = Math.Sign(direction.Y) * speedX;

                    score += 10 * (5 - b.RowIndex);
                    b.ClearToBuffer(myBuffer);
                    b.SetHitState();
                    //bricks.Remove(b);                    
                    break;
                }
            }

            bool caught = false;
            ballPos.X += direction.X;
            ballPos.Y += direction.Y;
            if (ballPos.X >= batPos.X && ballPos.X <= batPos.X + batLength && ballPos.Y >= batPos.Y)
            {
                caught = true;
                var diff = (ballPos.X - batPos.X);
                if (diff < batLength / 3)
                {
                    if (Math.Sign(direction.X) > 0)
                        direction.X = -direction.X;
                    else
                        speedX = 1.2f;
                }
                else if (diff > batLength * 2 / 3)
                {
                    if (Math.Sign(direction.X) > 0)
                        speedX = 1.2f;
                    else
                        direction.X = -direction.X;
                }
                else
                {
                    speedX = 1f;
                }
                direction.X = Math.Sign(direction.X) * speedX;

                direction.Y = -speedY;
                ballPos.Y += direction.Y;
                score += 10;
                myObject.AddTrailPoint(new MyObjectChar(ballPos, '.'));
                if (soundOn) Task.Run(() => Console.Beep(540, 500));
            }

            myBuffer.WriteCentered(0, $"SCORE: {score}", screenWidth, string.Empty);

            foreach (var b in bricks)
            {
                b.DrawToBuffer(myBuffer);
            }
            myObject.DrawToBuffer(myBuffer);

            if (WinState)
            {
                if (soundOn) Task.Run(() => Console.Beep(240, 800));
                myBuffer.SetChar((int)lastBallPos.X, (int)lastBallPos.Y, ' ', string.Empty);
                myBuffer.WriteCentered((int)(ballPos.Y / 2), "You Won, YSY!!!", screenWidth, string.Empty);
                break;
            }

            if (ballPos.Y >= screenHeight - 2 && !caught)
            {
                if (soundOn) Task.Run(() => Console.Beep(240, 800));
                myBuffer.SetChar((int)lastBallPos.X, (int)lastBallPos.Y, ' ', string.Empty);
                myBuffer.WriteCentered((int)(ballPos.Y / 2), "GAME OVER!!!", screenWidth, string.Empty);
                break;
            }

            if (Console.KeyAvailable)
            {
                var charPressed = Console.ReadKey(true).Key;
                if (charPressed == ConsoleKey.D || charPressed == ConsoleKey.RightArrow)
                {
                    batLast = new MyPoint(batPos);
                    if (batPos.X < screenWidth - 2)
                    {
                        batPos.X += batSpeed;
                    }
                }
                else if (charPressed == ConsoleKey.A || charPressed == ConsoleKey.LeftArrow)
                {
                    batLast = new MyPoint(batPos);
                    if (batPos.X > 0)
                    {
                        batPos.X += -batSpeed;
                    }
                }
                else if (charPressed == ConsoleKey.X)
                {
                    break;
                }
            }
            DrawBat(batPos, batLast, batLength, batSpeed, myBuffer, screenWidth);
            myBuffer.Render();
        }
        Console.CursorVisible = true;
        MyUIHelper.WriteCentered("Pong Game Ended - Press any key to exit", (int)(ballPos.Y + 1), screenWidth, myBuffer);
        myBuffer.Render();
        Console.ReadKey(true);

        MyUIHelper.ClearBox(0, 0, screenWidth, screenHeight, myBuffer);
    }

    private static int CreateBrickRow(List<MyBrick> bricks, int offset, int row, int numBrickPerWidth, int color)
    {
        for (int i = 0; i < numBrickPerWidth; i++)
        {
            bricks.Add(new MyBrick(8, 2, new MyPoint(offset, row), color));
            offset += 8;
        }

        return offset;
    }

    static char topLeft = '\u250C';   // ┌
    static char topRight = '\u2510';  // ┐
    static char bottomLeft = '\u2514';// └
    static char bottomRight = '\u2518';// ┘
    static char horizontal = '\u2500'; // ─
    static char vertical = '\u2502';   // │
    public static void DrawBat(MyPoint p, MyPoint last, int length, int batSpeed, MyBuffer myBuffer, int screenWidth)
    {
        var bat = string.Empty;
        if (p.X != last.X)
        {
            bat = new String(' ', length);
            myBuffer.SetString((int)last.X, (int)last.Y, bat, string.Empty);
        }

        if(p.X < 0) p.X = 0;
        if(p.X > screenWidth - length) p.X = screenWidth - length;
        bat = topLeft + new String(horizontal, length - 2) + topRight;
        myBuffer.SetString((int)p.X, (int)p.Y, bat, string.Empty);
    }
}