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

        protected MovingEntity(Vector2D pos, GameWorld world, float maxSpeed, float radius = 16, float mass = 1f) 
            : base(pos, world, radius)
        {
            Velocity = new Vector2D();
            Direction = new Vector2D(0, -1);
            Side = Direction.GetSide();
            MaxSpeed = maxSpeed;
            MaxForce = MaxSpeed * 2f;
            Mass = mass;
        }

        protected void ApplyForce(Vector2D steeringForce, float deltaTime) // steeringForce = de kracht die aangeeft welke kant de entity op moet bewegen
        {
            Vector2D acceleration = steeringForce.Clone().Divide(Mass);    // Hoeveel snelheid elke frame, hoe zwaarder -> hoe langzamer op gang komen
            Velocity.Add(acceleration.Multiply(deltaTime));                // Snelheid berekenen met gewicht
            Velocity.Truncate(MaxSpeed);                                   // Maximum snelheid toepassen
            Pos.Add(Velocity.Clone().Multiply(deltaTime));                 // Positie updaten

            // Nodig om de entity mee te draaien, anders kijkt hij permanent omhoog
            if (!Velocity.IsZero())
            {
                Direction = Velocity.Clone().Normalize();
                Side = Direction.GetSide();
            }
        }
    }
}