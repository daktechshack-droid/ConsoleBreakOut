
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
            HandleWallCollusion(screenWidth, soundOn, ballPos, direction, ref speedX, ref speedY);
            HandleBrickCollusion(myBuffer, ballPos, direction, ref score, ref speedX, ref speedY, ref WinState, bricks, soundOn);
            bool caught = HandleBatCollusion(soundOn, myObject, ballPos, direction, batPos, ref score, batLength, ref speedX, speedY);

            if (!HandleGameWon(screenWidth, soundOn, myBuffer, ballPos, lastBallPos, WinState)) break;
            if (!HandleGameOVer(screenWidth, screenHeight, soundOn, myBuffer, ballPos, lastBallPos, caught)) break;

            DrawToScreen(screenWidth, myBuffer, myObject, batPos, batLast, score, batLength, batSpeed, bricks);

            lastBallPos = new MyPoint(ballPos);
            ballPos.X += direction.X;
            ballPos.Y += direction.Y;
            batLast = new MyPoint(batPos);
            if (!HandleKeyPressEvents(screenWidth, batPos, batSpeed)) break;
        }
        Console.CursorVisible = true;
        MyUIHelper.WriteCentered("Pong Game Ended - Press any key to exit", (int)(ballPos.Y + 1), screenWidth, myBuffer);
        myBuffer.Render();
        Console.ReadKey(true);

        MyUIHelper.ClearBox(0, 0, screenWidth, screenHeight, myBuffer);
    }

    private static void DrawToScreen(int screenWidth, MyBuffer myBuffer, MyObject myObject, MyPoint batPos, MyPoint batLast, int score, int batLength, int batSpeed, List<MyBrick> bricks)
    {
        myBuffer.WriteCentered(0, $"SCORE: {score}", screenWidth, string.Empty);
        foreach (var b in bricks)
        {
            b.DrawToBuffer(myBuffer);
        }
        myObject.DrawToBuffer(myBuffer);

        DrawBat(batPos, batLast, batLength, batSpeed, myBuffer, screenWidth);
        myBuffer.Render();
    }

    private static void HandleWallCollusion(int screenWidth, bool soundOn, MyPoint ballPos, MyPoint direction, ref float speedX, ref float speedY)
    {
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
    }

    private static bool HandleGameWon(int screenWidth, bool soundOn, MyBuffer myBuffer, MyPoint ballPos, MyPoint lastBallPos, bool WinState)
    {
        if (WinState)
        {
            if (soundOn) Task.Run(() => Console.Beep(240, 800));
            myBuffer.SetChar((int)lastBallPos.X, (int)lastBallPos.Y, ' ', string.Empty);
            myBuffer.WriteCentered((int)(ballPos.Y / 2), "You Won, YSY!!!", screenWidth, string.Empty);
            return false;
        }

        return true;
    }

    private static bool HandleGameOVer(int screenWidth, int screenHeight, bool soundOn, MyBuffer myBuffer, MyPoint ballPos, MyPoint lastBallPos, bool caught)
    {
        if (ballPos.Y >= screenHeight - 2 && !caught)
        {
            if (soundOn) Task.Run(() => Console.Beep(240, 800));
            myBuffer.SetChar((int)lastBallPos.X, (int)lastBallPos.Y, ' ', string.Empty);
            myBuffer.WriteCentered((int)(ballPos.Y / 2), "GAME OVER!!!", screenWidth, string.Empty);
            return false;
        }

        return true;
    }

    private static bool HandleKeyPressEvents(int screenWidth, MyPoint batPos, int batSpeed)
    {
        if (Console.KeyAvailable)
        {
            var charPressed = Console.ReadKey(true).Key;
            if (charPressed == ConsoleKey.D || charPressed == ConsoleKey.RightArrow)
            {
                if (batPos.X < screenWidth - 2)
                {
                    batPos.X += batSpeed;
                }
            }
            else if (charPressed == ConsoleKey.A || charPressed == ConsoleKey.LeftArrow)
            {
                if (batPos.X > 0)
                {
                    batPos.X += -batSpeed;
                }
            }
            else if (charPressed == ConsoleKey.X)
            {
                return false;
            }
        }

        return true;
    }

    private static bool HandleBatCollusion(bool soundOn, MyObject myObject, MyPoint ballPos, MyPoint direction, MyPoint batPos, ref int score, int batLength, ref float speedX, float speedY)
    {
        bool caught = false;
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

        return caught;
    }

    private static void HandleBrickCollusion(MyBuffer myBuffer, MyPoint ballPos, MyPoint direction, ref int score, ref float speedX, ref float speedY, ref bool WinState, List<MyBrick> bricks, bool soundOn)
    {
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
                if (soundOn) Task.Run(() => Console.Beep(2000, 500));
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