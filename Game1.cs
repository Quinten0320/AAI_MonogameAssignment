using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;
    private DungeonMap _dungeonMap;

    public Game1()
    {
        _ = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = DungeonMap.Cols * DungeonMap.TileSize,   // 800
            PreferredBackBufferHeight = DungeonMap.Rows * DungeonMap.TileSize,  // 480
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _dungeonMap = new DungeonMap();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _dungeonMap.LoadContent(GraphicsDevice);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        _dungeonMap.Draw(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
