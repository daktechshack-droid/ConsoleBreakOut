
using ConsoleBreakOut;

public static class BreakOutGameBuffer
{
    public static void Start(int screenWidth = 80, int screenHeight = 25, bool soundOn = false)
    {
        var myBuffer = new MyBuffer(screenWidth, screenHeight);
        var myObject = new MyObject(10, 5);
        var ballPos = myObject.Position;
        var direction = new MyPoint(1, 1);
        var lastBallPos = new MyPoint(ballPos);
        var batPos = new MyPoint(screenWidth / 2, screenHeight - 2);
        var batLast = new MyPoint(batPos);
        var score = 0;
        var batLength = 12;
        var batSpeed = 8;
        float speed  = 1f;

        var bricks = new List<MyBrick>();
        var offset = 0;
        var numBrickPerWidth = screenWidth / 10;
        for (int i = 0; i < numBrickPerWidth; i++)
        {
            bricks.Add(new MyBrick(8, 2, new MyPoint(offset, 2)));
            offset += 9;
        }
        offset = 0;
        for (int i = 0; i < numBrickPerWidth; i++)
        {
            bricks.Add(new MyBrick(8, 2, new MyPoint(offset, 5), 1));
            offset += 9;
        }

        myBuffer.Clear();
        Console.CursorVisible = false;
        myBuffer.WriteCentered(screenHeight - 1, "Pong Game - Press X to exit", screenWidth, string.Empty);
        while (true)
        {
            myBuffer.SetChar((int)lastBallPos.X, (int)lastBallPos.Y, ' ');

            Thread.Sleep(50);
            if (ballPos.X > screenWidth - 2)
            {
                direction.X = -speed;
                if(soundOn) Task.Run(() => Console.Beep(440, 500));
            }
            //if (ballPos.Y > screenHeight - 2) direction.Y = -speed;

            if (ballPos.X < 1)
            {
                direction.X = speed;
                if (soundOn) Task.Run(() => Console.Beep(440, 500));
            }
            if (ballPos.Y < 1)
            {
                direction.Y = speed;
                if (soundOn) Task.Run(() => Console.Beep(440, 500));
            }
            
            lastBallPos = new MyPoint(ballPos);

            foreach (var b in bricks)
            {
                if(b.CheckCollusion(ballPos))
                {
                    direction.Y = -speed;
                    break;
                }
            }

            ballPos.X += direction.X;
            ballPos.Y += direction.Y;
            if (ballPos.X >= batPos.X && ballPos.X <= batPos.X + batLength && ballPos.Y == batPos.Y)
            {
                direction.Y = -speed;
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
            if (ballPos.Y == screenHeight - 2)
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
                    if(batPos.X > 0)
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
        bat = new String('^', length);
        myBuffer.SetString((int)p.X, (int)p.Y, bat, string.Empty);
    }
}