using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Scenes;

namespace Project;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;
    private readonly Stack<IScene> _sceneStack = new Stack<IScene>();
    private IScene _pendingScene;
    private bool _pendingPush;
    private bool _pendingPop;

    public SpriteFont DebugFont { get; private set; }

    public Game1()
    {
        _ = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = DungeonMap.Cols * DungeonMap.TileSize,
            PreferredBackBufferHeight = DungeonMap.Rows * DungeonMap.TileSize,
        };
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        DebugDrawer.LoadContent(GraphicsDevice);
        DebugFont = Content.Load<SpriteFont>("DebugFont");

        ChangeScene(new MenuScene());
    }

    public void ChangeScene(IScene scene)
    {
        _pendingScene = scene;
        _pendingPush = false;
        _pendingPop = false;
    }

    public void PushScene(IScene scene)
    {
        _pendingScene = scene;
        _pendingPush = true;
        _pendingPop = false;
    }

    public void PopScene()
    {
        _pendingPop = true;
        _pendingScene = null;
        _pendingPush = false;
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (_pendingPop)
        {
            if (_sceneStack.Count > 1)
                _sceneStack.Pop();
            _pendingPop = false;
        }
        else if (_pendingScene != null)
        {
            if (_pendingPush)
            {
                _pendingScene.LoadContent(this, GraphicsDevice, _spriteBatch);
                _sceneStack.Push(_pendingScene);
            }
            else
            {
                _sceneStack.Clear();
                _pendingScene.LoadContent(this, GraphicsDevice, _spriteBatch);
                _sceneStack.Push(_pendingScene);
            }
            _pendingScene = null;
            _pendingPush = false;
        }

        if (_sceneStack.Count > 0)
            _sceneStack.Peek().Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        var scenes = _sceneStack.ToArray();
        for (int i = scenes.Length - 1; i >= 0; i--)
            scenes[i].Draw(_spriteBatch);

        _spriteBatch.End();
        base.Draw(gameTime);
    }
}
