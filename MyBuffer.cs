using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ConsoleBreakOut
{
    public class MyBuffer
    {
        private int Width, Height;
        private char[,] buffer;
        private string[,] colorbuffer;

        public MyBuffer(int width, int height)
        {
            Width = width;
            Height = height;
            buffer = new char[width, height];
            colorbuffer = new string[width, height];
            Clear();
        }

        public void Clear()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    buffer[x, y] = ' ';
                    colorbuffer[x, y] = "\u001b[0m";
                }
            }
        }

        public void SetChar(int x, int y, char character, string color = "")
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                buffer[x, y] = character;
                if(!string.IsNullOrWhiteSpace(color))
                    colorbuffer[x, y] = color;
            }
        }

        public void SetString(int x, int y, string text, string color = "")
        {
            foreach (char c in text)
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                {
                    buffer[x, y] = c;
                    if (!string.IsNullOrWhiteSpace(color))
                        colorbuffer[x, y] = color;
                }
                x++;
            }
        }

        public void WriteCentered(int y, string text, int screenWidth, string color = "")
        {
            int x = (screenWidth - text.Length) / 2;
            SetString(x, y, text, color);            
        }

        public void Render()
        {
            Console.SetCursorPosition(0, 0);
            StringBuilder screen = new StringBuilder();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (!string.IsNullOrWhiteSpace(colorbuffer[x, y]))
                        screen.Append(colorbuffer[x, y]);
                    screen.Append(buffer[x, y]);
                }
                if (y < Height - 1)
                    screen.AppendLine();
            }
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Write(screen.ToString());
        }
    }

}
