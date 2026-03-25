using System;
using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project
{
    public static class DebugDrawer
    {
        private static Texture2D _pixel;

        public static void LoadContent(GraphicsDevice graphicsDevice)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2D from, Vector2D to, Color color, float thickness = 2f)
        {
            float dx = to.X - from.X;
            float dy = to.Y - from.Y;
            float length = MathF.Sqrt(dx * dx + dy * dy);
            float angle = MathF.Atan2(dy, dx);

            spriteBatch.Draw(
                _pixel,
                new Vector2(from.X, from.Y),
                null,
                color,
                angle,
                Vector2.Zero,
                new Vector2(length, thickness),
                SpriteEffects.None,
                0f
            );
        }
    }
}
