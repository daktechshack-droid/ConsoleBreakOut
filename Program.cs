using ConsoleBreakOut;

int screenWidth = Console.WindowWidth;
int screenHeight = Console.WindowHeight;

var myBuffer = new MyBuffer(screenWidth, screenHeight);

MyUIHelper.WriteCentered("My Utility Software", 3, screenWidth, myBuffer);

// Draw a border around the console window
MyUIHelper.DrawBox(0, 0, screenWidth, screenHeight, myBuffer);
await ShowMainMenu(screenWidth, screenHeight, myBuffer);

static async Task ShowMainMenu(int screenWidth, int screenHeight, MyBuffer myBuffer)
{
    string[] menuItems = new string[]
    {
        "Main Menu",
        "1. Enter your person",
        "2. Play Breakout",
        "Q/X. Exit"
    };

    while (true)
    {
        MyUIHelper.BuildMenu(5, 5, 40, 6, menuItems, myBuffer);
        myBuffer.Render();
        var getSelection = Console.ReadKey(); // Wait for user input before closing the console
        if (getSelection.Key == ConsoleKey.D2)
        {
            BreakOutGameBuffer.Start(screenWidth, screenHeight);
            continue;
        }
        if (getSelection.Key == ConsoleKey.X || getSelection.Key == ConsoleKey.Q)
        {
            break;
        }        
    }
}