using AAI_MonogameAssignment;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Entities.BaseEntity
{
    public abstract class BaseGameEntity
    {
        public Vector2D Pos { get; set; }
        public float Radius { get; set; }
        public bool IsActive { get; set; } = true;

        protected BaseGameEntity(Vector2D pos, float radius = 16f)
        {
            Pos = pos;
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