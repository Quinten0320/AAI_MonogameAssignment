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

        // Basis toevoegen en aftrekken!!! --------------------------------------------------------

        public Vector2D Add(Vector2D v)
        {
            this.X += v.X;
            this.Y += v.Y;
            return this;
        }

        public Vector2D Sub(Vector2D v)
        {
            this.X -= v.X;
            this.Y -= v.Y;
            return this;
        }

        public Vector2D Multiply(float value)
        {
            this.X *= value;
            this.Y *= value;
            return this;
        }

        public Vector2D Divide(float value)
        {
            this.X /= value;
            this.Y /= value;
            return this;
        }

        // Lengte ------------------------------------------------------------------------------------

        public float Length()
        {
            return MathF.Sqrt((X * X) + (Y * Y));
        }

        public float LengthSquared()
        {
            return (X * X) + (Y * Y);
        }
        public bool IsZero()
        {
            return X == 0 && Y == 0;
        }

        // Normalizeren en truncate ------------------------------------------------------------------------------------

        // diagonaal bewegen is niet sneller dan rechtdoor bijv.
        public Vector2D Normalize()
        {
            float length = this.Length();

            if (length != 0)
            {
                this.X /= length;
                this.Y /= length;
            }
            return this;
        }

        // maximale snelheid limiteren
        public Vector2D Truncate(float maX)
        {
            if (Length() > maX)
            {
                Normalize();
                Multiply(maX);
            }
            return this;
        }

        // Hulpfunctie voor steering ------------------------------------------------------------------------------------

        // Checkt of 2 vectoren dezlefde kan op wijzen, handig voor "obstacle avoidance"
        public float Dot(Vector2D other)
        {
            return X * other.X + Y * other.Y;
        }

        // geeft een vector terug die 90 graden gedraaid is, handig om te checken of iets links of rechts van de entity is
        public Vector2D Perpendicular()
        {
            return new Vector2D(-Y, X);
        }

        // nodig voor debug lijnen tekenen
        public float Angle()
        {
            return MathF.Atan2(Y, X);
        }

        public float DistanceTo(Vector2D other)
        {
            float dx = this.X - other.X;
            float dy = this.Y - other.Y;
            return MathF.Sqrt(dx * dx + dy * dy);
        }

        public float DistanceSquaredTo(Vector2D other)
        {
            float dx = this.X - other.X;
            float dy = this.Y - other.Y;
            return dx * dx + dy * dy;
        }

        // Monogame ----------------------------------------------------------------------------------------------------------------
        public Vector2 ToXna()
        {
            return new Vector2(X, Y);
        }

        public static Vector2D FromXna(Vector2 v)
        {
            return new Vector2D(v.X, v.Y);
        }

        // Clone en toString ------------------------------------------------------------------------------------
        public Vector2D Clone()
        {
            return new Vector2D(this.X, this.Y);
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
        }
    }
}