using System.Drawing;

namespace GameOfLife.Core.Extensions
{
    public static class CalculationExtensions
    {
        public static Point WrapToBoard(this Point point, int width, int height)
        {
            var wrappedX = (point.X % width + width) % width;
            var wrappedY = (point.Y % height + height) % height;
            return new Point(wrappedX, wrappedY);
        }
    }
}
