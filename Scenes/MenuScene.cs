using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project.Scenes
{
    public class MenuScene : IScene
    {
        private Game1 _game;
        private SpriteFont _font;
        private Texture2D _pixel;
        private MouseState _previousMouse;

        private Rectangle _playButton;
        private int _screenWidth;
        private int _screenHeight;

        public void LoadContent(Game1 game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _game = game;
            _font = game.DebugFont;

            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _screenWidth = graphicsDevice.Viewport.Width;
            _screenHeight = graphicsDevice.Viewport.Height;

            int btnW = 160;
            int btnH = 48;
            _playButton = new Rectangle(
                (_screenWidth - btnW) / 2,
                _screenHeight / 2 + 20,
                btnW, btnH
            );
        }

        public void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            {
                if (_playButton.Contains(mouse.X, mouse.Y))
                    _game.ChangeScene(new GameScene());
            }

            _previousMouse = mouse;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            string title = "Dungeon Crawl";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(_font, title,
                new Vector2((_screenWidth - titleSize.X * 2.5f) / 2, _screenHeight / 2 - 60),
                Color.White, 0f, Vector2.Zero, 2.5f, SpriteEffects.None, 0f);


            bool hover = _playButton.Contains(Mouse.GetState().X, Mouse.GetState().Y);
            Color btnColor = hover ? new Color(80, 60, 140) : new Color(60, 40, 110);
            spriteBatch.Draw(_pixel, _playButton, btnColor);


            DrawBorder(spriteBatch, _playButton, Color.White, 2);


            string btnText = "Play";
            Vector2 btnTextSize = _font.MeasureString(btnText);
            spriteBatch.DrawString(_font, btnText,
                new Vector2(
                    _playButton.X + (_playButton.Width - btnTextSize.X * 1.5f) / 2,
                    _playButton.Y + (_playButton.Height - btnTextSize.Y * 1.5f) / 2),
                Color.White, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
        }

        private void DrawBorder(SpriteBatch sb, Rectangle rect, Color color, int thickness)
        {
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color);
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            sb.Draw(_pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color);
        }
    }
}
