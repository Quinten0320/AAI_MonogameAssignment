using AAI_MonogameAssignment;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Entities.BaseEntity
{
    public abstract class BaseGameEntity
    {
        private static int _nextId = 0;

        public int Id { get; private set; }
        public Vector2D Pos { get; set; }
        public float Radius { get; set; }
        public bool IsActive { get; set; } = true;
        public GameWorld MyWorld { get; set; }

        protected BaseGameEntity(Vector2D pos, GameWorld world, float radius = 16f)
        {
            Id = _nextId++;
            Pos = pos;
            MyWorld = world;
            Radius = radius;
        }

        public abstract void Update(float deltaTime);
        public abstract void Render(SpriteBatch spriteBatch);
        public abstract void RenderDebug(SpriteBatch spriteBatch);
        public bool Overlaps(BaseGameEntity otherEntity)
        {
            float minDist = Radius + otherEntity.Radius;
            return Pos.DistanceSquaredTo(otherEntity.Pos) < minDist * minDist;
        }
    }
}