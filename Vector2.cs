using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Vector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector2(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) 
                return false;

            if (obj is Vector2 other)
                return this.X == other.X && this.Y == other.Y;
            
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator !=(Vector2 a, Vector2 b) => !a.Equals(b);
        public static bool operator ==(Vector2 a, Vector2 b) => a.Equals(b);
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
    }
}
