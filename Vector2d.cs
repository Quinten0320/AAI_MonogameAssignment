using System;
using Microsoft.Xna.Framework;

namespace AAI_MonogameAssignment
{
    public class Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D() : this(0, 0) { }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2D Add(Vector2D v)
        {
            X += v.X;
            Y += v.Y;
            return this;
        }

        public Vector2D Sub(Vector2D v)
        {
            X -= v.X;
            Y -= v.Y;
            return this;
        }

        public Vector2D Multiply(float value)
        {
            X *= value;
            Y *= value;
            return this;
        }

        public Vector2D Divide(float value)
        {
            X /= value;
            Y /= value;
            return this;
        }

        public float Length()
        {
            return MathF.Sqrt(X * X + Y * Y);
        }

        public float LengthSquared()
        {
            return X * X + Y * Y;
        }

        public bool IsZero()
        {
            return X == 0 && Y == 0;
        }

        public Vector2D Normalize()
        {
            float length = Length();
            if (length != 0)
            {
                X /= length;
                Y /= length;
            }
            return this;
        }

        public Vector2D Truncate(float max)
        {
            if (Length() > max)
            {
                Normalize();
                Multiply(max);
            }
            return this;
        }

        public float Dot(Vector2D other)
        {
            return X * other.X + Y * other.Y;
        }

        public Vector2D GetSide()
        {
            return new Vector2D(-Y, X);
        }

        public float Angle()
        {
            return MathF.Atan2(Y, X);
        }

        public float DistanceTo(Vector2D other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public float DistanceSquaredTo(Vector2D other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            return dx * dx + dy * dy;
        }

        public Vector2 ToXna()
        {
            return new Vector2(X, Y);
        }

        public static Vector2D FromXna(Vector2 v)
        {
            return new Vector2D(v.X, v.Y);
        }

        public Vector2D Clone()
        {
            return new Vector2D(X, Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
