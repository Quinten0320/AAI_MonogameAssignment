using System;
using AAI_MonogameAssignment;
using Project.Entities.BaseEntity;

namespace Project.Steering
{
    public class ObstacleAvoidance
    {
        private readonly DungeonMap _map;
        private const float FeelerLength = 28f;
        private const float RepulsionStrength = 200f;

        public ObstacleAvoidance(DungeonMap map)
        {
            _map = map;
        }

        public Vector2D Calculate(MovingEntity entity)
        {
            if (entity.Velocity.IsZero())
                return new Vector2D();

            Vector2D force = new Vector2D();
            Vector2D dir = entity.Direction;
            Vector2D side = entity.Side;

            CheckFeeler(entity.Pos, dir, FeelerLength, force);
            CheckFeeler(entity.Pos, new Vector2D(dir.X + side.X * 0.5f, dir.Y + side.Y * 0.5f).Normalize(), FeelerLength * 0.8f, force);
            CheckFeeler(entity.Pos, new Vector2D(dir.X - side.X * 0.5f, dir.Y - side.Y * 0.5f).Normalize(), FeelerLength * 0.8f, force);

            return force;
        }

        private void CheckFeeler(Vector2D pos, Vector2D direction, float length, Vector2D force)
        {
            for (float t = 0.5f; t <= 1f; t += 0.25f)
            {
                float checkX = pos.X + direction.X * length * t;
                float checkY = pos.Y + direction.Y * length * t;

                if (_map.CollidesWithWall(checkX, checkY, 2f))
                {
                    float strength = RepulsionStrength * (1f - t + 0.2f);
                    force.X -= direction.X * strength;
                    force.Y -= direction.Y * strength;
                    return;
                }
            }
        }
    }
}
