using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Entities;

namespace Project;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;
    private DungeonMap _dungeonMap;
    private Player _player;

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

        var wizardTexture = Texture2D.FromFile(GraphicsDevice, "ModelSprites/Wizard.png");

        var startPos = new Vector2D(
            1.5f * DungeonMap.TileSize,
            1.5f * DungeonMap.TileSize
        );
        _player = new Player(startPos, new GameWorld(), wizardTexture, _dungeonMap);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _player.Update(delta);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        _dungeonMap.Draw(_spriteBatch);
        _player.Render(_spriteBatch);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
