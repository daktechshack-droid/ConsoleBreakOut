using ConsoleBreakOut;

public class MyObjectChar   
{
    public MyPoint Position { get; set; }
    public char Character { get; set; }

    public MyObjectChar(MyPoint position, char character)
    {
        Position = new MyPoint(position.X, position.Y);
        Character = character;
    }    
}