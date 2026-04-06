using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Entities.BaseEntity;

namespace Project.Entities
{
    public class Projectile : BaseGameEntity
    {
        private const float Speed = 400f;
        private const int Size = 8;
        public const int Damage = 10;
        private readonly Vector2D _velocity;
        private readonly Texture2D _texture;
        private readonly DungeonMap _map;

        public Projectile(Vector2D pos, Vector2D direction, DungeonMap map, Texture2D texture)
            : base(pos, radius: Size / 2f)
        {
            _velocity = direction.Clone().Normalize().Multiply(Speed);
            _texture = texture;
            _map = map;
        }

        public override void Update(float deltaTime)
        {
            Pos.Add(_velocity.Clone().Multiply(deltaTime));

            if (_map.CollidesWithWall(Pos.X, Pos.Y, Radius))
                IsActive = false;
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            var dest = new Rectangle((int)(Pos.X - Size / 2f), (int)(Pos.Y - Size / 2f), Size, Size);
            spriteBatch.Draw(_texture, dest, Color.White);
        }

        public override void RenderDebug(SpriteBatch spriteBatch) { }
    }
}
