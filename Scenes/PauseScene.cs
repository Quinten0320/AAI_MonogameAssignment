using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project.Scenes
{
    public class PauseScene : IScene
    {
        private Game1 _game;
        private SpriteFont _font;
        private Texture2D _pixel;
        private MouseState _previousMouse;

        private Rectangle _homeButton;
        private Rectangle _resumeButton;
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

            int btnW = 200;
            int btnH = 44;
            int spacing = 16;
            int totalH = btnH * 2 + spacing;
            int startY = (_screenHeight - totalH) / 2 + 20;

            _resumeButton = new Rectangle((_screenWidth - btnW) / 2, startY, btnW, btnH);
            _homeButton = new Rectangle((_screenWidth - btnW) / 2, startY + btnH + spacing, btnW, btnH);
        }

        public void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
            {
                if (_resumeButton.Contains(mouse.X, mouse.Y))
                    _game.PopScene();
                else if (_homeButton.Contains(mouse.X, mouse.Y))
                    _game.ChangeScene(new MenuScene());
            }

            _previousMouse = mouse;
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(_pixel, new Rectangle(0, 0, _screenWidth, _screenHeight), new Color(0, 0, 0, 180));


            string title = "Paused";
            Vector2 titleSize = _font.MeasureString(title);
            spriteBatch.DrawString(_font, title,
                new Vector2((_screenWidth - titleSize.X * 2f) / 2, _screenHeight / 2 - 60),
                Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);


            DrawButton(spriteBatch, _resumeButton, "Resume");


            DrawButton(spriteBatch, _homeButton, "Back to Home");
        }

        private void DrawButton(SpriteBatch sb, Rectangle rect, string text)
        {
            bool hover = rect.Contains(Mouse.GetState().X, Mouse.GetState().Y);
            Color btnColor = hover ? new Color(80, 60, 140) : new Color(60, 40, 110);
            sb.Draw(_pixel, rect, btnColor);


            int t = 2;
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, t), Color.White);
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Bottom - t, rect.Width, t), Color.White);
            sb.Draw(_pixel, new Rectangle(rect.X, rect.Y, t, rect.Height), Color.White);
            sb.Draw(_pixel, new Rectangle(rect.Right - t, rect.Y, t, rect.Height), Color.White);


            Vector2 textSize = _font.MeasureString(text);
            sb.DrawString(_font, text,
                new Vector2(
                    rect.X + (rect.Width - textSize.X * 1.3f) / 2,
                    rect.Y + (rect.Height - textSize.Y * 1.3f) / 2),
                Color.White, 0f, Vector2.Zero, 1.3f, SpriteEffects.None, 0f);
        }
    }
}
