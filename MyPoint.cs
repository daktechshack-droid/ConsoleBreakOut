namespace ConsoleBreakOut
{
    public class MyPoint
    {
        public MyPoint(float x, float y)
        {   
            X = x;
            Y = y;
        }

        public MyPoint(MyPoint point)
        {
            X = point.X;
            Y = point.Y;
        }

        public float X { get; set; }
        
        public float Y { get; set; }
    }
}
