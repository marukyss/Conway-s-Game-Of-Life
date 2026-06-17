using System;
using System.Collections.Generic;
using System.Text;

namespace ConwaysGameOfLife.model
{
    public interface ITopology
    {
        (int X, int Y) Wrap(int x, int y, int width, int height);
    }

    public class EucledeanPlane : ITopology
    {
        public (int X, int Y) Wrap(int x, int y, int width, int height)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return (-1, -1);
            }
            return (x, y);
        }
    }

    public class Torus : ITopology
    {
        public (int X, int Y) Wrap(int x, int y, int width, int height)
        {
            int X = (x % width + width) % width;
            int Y = (y % height + height) % height;
            return (X, Y);
        }
    }

    public class KleinBottle : ITopology
    {
        public (int X, int Y) Wrap(int x, int y, int width, int height)
        {
            if (x < 0 || x >= width)
            {
                y = height - 1 - y;
            }

            int X = (x % width + width) % width;
            int Y = (y % height + height) % height; ;
            return (X, Y);
        }
    }

    public class MobiusStrip : ITopology
    {
        public (int X, int Y) Wrap(int x, int y, int width, int height)
        {
            if (y < 0 || y >= height)
                return (-1, -1);

            if (x < 0 || x >= width)
                y = height - 1 - y;
         
            int X = (x % width + width) % width;
            return (X, y);
        }
    }
}
