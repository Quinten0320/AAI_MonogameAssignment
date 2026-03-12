using System;
using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Entities.BaseEntity;

namespace Project.Entities;

public class Player : BaseGameEntity
{
    private const float Speed = 150f;

    private readonly Texture2D _sprite;
    private readonly DungeonMap _map;
    private float _rotation;

    public Player(Vector2D pos, GameWorld world, Texture2D sprite, DungeonMap map)
        : base(pos, world)
    {
        _sprite = sprite;
        _map = map;
    }

    public override void Update(float delta)
    {
        var state = Keyboard.GetState();
        var direction = new Vector2D();

        if (state.IsKeyDown(Keys.W)) direction.Y -= 1;
        if (state.IsKeyDown(Keys.S)) direction.Y += 1;
        if (state.IsKeyDown(Keys.A)) direction.X -= 1;
        if (state.IsKeyDown(Keys.D)) direction.X += 1;

        if (direction.IsZero()) return;

        direction.Normalize();

        _rotation = MathF.Atan2(direction.Y, direction.X) + MathHelper.PiOver2;

        float moveX = direction.X * Speed * delta;
        float moveY = direction.Y * Speed * delta;

        if (!_map.CollidesWithWall(Pos.X + moveX, Pos.Y, Radius))
            Pos.X += moveX;

        if (!_map.CollidesWithWall(Pos.X, Pos.Y + moveY, Radius))
            Pos.Y += moveY;
    }

    public override void Render(SpriteBatch spriteBatch)
    {
        var origin = new Vector2(_sprite.Width / 2f, _sprite.Height / 2f);
        spriteBatch.Draw(_sprite, Pos.ToXna(), null, Color.White, _rotation, origin, 1f, SpriteEffects.None, 0f);
    }

    public override void RenderDebug(SpriteBatch spriteBatch) { }
}
