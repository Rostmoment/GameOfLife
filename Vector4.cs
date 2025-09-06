using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    class Vector4
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; } 
        public int W { get; set; }

        public Vector4(int x, int y, int z, int w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Vector4 other)
                return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;

            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public static bool operator !=(Vector4 a, Vector4 b) => !a.Equals(b);
        public static bool operator ==(Vector4 a, Vector4 b) => a.Equals(b);
        public static Vector4 operator +(Vector4 a, Vector4 b) => new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        public static Vector4 operator -(Vector4 a, Vector4 b) => new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    }
}
