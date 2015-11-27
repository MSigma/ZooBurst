using Microsoft.Xna.Framework;

namespace ZooBurst.Utils
{
    public static class PointExt
    {
         public static Point GetDirectionVector(this Point a, Point b) => new Point(a.X < b.X ? -1 : a.X > b.X ? 1 : 0, a.Y < b.Y ? -1 : a.Y > b.Y ? 1 : 0);
    }
}