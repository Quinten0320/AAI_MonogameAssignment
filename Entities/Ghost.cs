using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Entities.BaseEntity;
using Project.PathFinding;
using Project.Steering;

namespace Project.Entities
{
    public class Ghost : MovingEntity
    {
        public bool Collected { get; private set; }
        private readonly Texture2D _sprite;
        private float _timer;
        private const float Lifetime = 10f;
        private readonly FleeBehaviour _fleeBehaviour;
        private readonly ObstacleAvoidance _obstacleAvoidance;

        private const float DebugLineLength = 50f;
        public Ghost(Vector2D pos, Texture2D sprite, Vector2D playerPos,
                     NavGraph navGraph, DungeonMap map)
            : base(pos, maxSpeed: 140f, radius: 10f)
        {
            _sprite = sprite;
            IsActive = true;
            _fleeBehaviour = new FleeBehaviour(playerPos, navGraph);
            _obstacleAvoidance = new ObstacleAvoidance(map);
        }

        public override void Update(float delta)
        {
            _timer += delta;
            if (_timer >= Lifetime)
            {
                IsActive = false;
                return;
            }

            Vector2D fleeForce = _fleeBehaviour.Calculate(this);
            Vector2D avoidForce = _obstacleAvoidance.Calculate(this);
            Vector2D total = fleeForce.Add(avoidForce);
            total.Truncate(MaxForce);

            Vector2D accel = total.Clone().Divide(Mass);
            Velocity.Add(accel.Multiply(delta));
            Velocity.Truncate(MaxSpeed);

            Pos.X += Velocity.X * delta;
            Pos.Y += Velocity.Y * delta;

            if (!Velocity.IsZero())
            {
                Direction = Velocity.Clone().Normalize();
                Side = Direction.GetSide();
            }
        }

        public void Collect()
        {
            Collected = true;
            IsActive = false;
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            float alpha = 1f - (_timer / Lifetime);

            float angle = (float)System.Math.Atan2(Direction.Y, Direction.X) + MathHelper.PiOver2;
            var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
            float scale = (Radius * 3f) / _sprite.Width;

            spriteBatch.Draw(_sprite, Pos.ToXna(), null,
                Color.White * alpha, angle, origin, scale, SpriteEffects.None, 0f);
        }

        public override void RenderDebug(SpriteBatch spriteBatch)
        {
            _obstacleAvoidance.DrawFeelers(spriteBatch, this);

            if (!Velocity.IsZero())
            {
                Vector2D end = Pos.Clone().Add(Velocity.Clone().Normalize().Multiply(DebugLineLength));
                DebugDrawer.DrawLine(spriteBatch, Pos, end, Color.LimeGreen);
            }

            var path = _fleeBehaviour.GetCurrentPath();
            int pathIndex = _fleeBehaviour.GetCurrentPathIndex();
            if (path != null && pathIndex < path.Count)
            {
                DebugDrawer.DrawLine(spriteBatch, Pos, path[pathIndex].WorldPos, Color.Red);

                for (int i = pathIndex; i < path.Count - 1; i++)
                    DebugDrawer.DrawLine(spriteBatch, path[i].WorldPos, path[i + 1].WorldPos, Color.Red);
            }
        }

        public void RenderDebugText(SpriteBatch spriteBatch, SpriteFont font)
        {
            float timeLeft = System.MathF.Max(0f, Lifetime - _timer);
            string info = $"Ghost: T:{timeLeft:F1}s";

            Vector2 textSize = font.MeasureString(info);
            Vector2 textPos = new Vector2(Pos.X - textSize.X / 2f, Pos.Y - Radius - 20);
            spriteBatch.DrawString(font, info, textPos, Color.Cyan);
        }
    }
}