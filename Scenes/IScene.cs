using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Scenes
{
    public interface IScene
    {
        void LoadContent(Game1 game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch);
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
