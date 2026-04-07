using AAI_MonogameAssignment;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Entities.BaseEntity
{
    public abstract class MovingEntity : BaseGameEntity
    {
        public Vector2D Velocity { get; set; }
        public Vector2D Direction { get; set; }
        public Vector2D Side { get; set; }

        public float MaxSpeed { get; set; }
        public float MaxForce { get; set; } 
        public float Mass { get; set; }

        protected MovingEntity(Vector2D pos, float maxSpeed, float radius = 16, float mass = 1f)
            : base(pos, radius)
        {
            Velocity = new Vector2D();
            Direction = new Vector2D(0, -1);
            Side = Direction.GetSide();
            MaxSpeed = maxSpeed;
            MaxForce = MaxSpeed * 2f;
            Mass = mass;
        }

    }
}