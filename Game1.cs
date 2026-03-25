using System.Collections.Generic;
using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Entities;
using Project.PathFinding;

namespace Project;

public class Game1 : Game
{
    private SpriteBatch _spriteBatch;
    private DungeonMap _dungeonMap;
    private NavGraph _navGraph;
    private GameWorld _world;
    private Player _player;
    private readonly List<Skeleton> _skeletons = new List<Skeleton>();
    private readonly List<Projectile> _projectiles = new List<Projectile>();

    private Texture2D _projectileTexture;
    private Texture2D _pixelTexture;
    private int _currentRound;
    private bool _debugMode;
    private KeyboardState _previousKeyState;
    private MouseState _previousMouseState;

    private static readonly Vector2D[] SpawnPoints = new[]
    {
        new Vector2D(20.5f * DungeonMap.TileSize, 1.5f * DungeonMap.TileSize),
        new Vector2D(15.5f * DungeonMap.TileSize, 2.5f * DungeonMap.TileSize),
        new Vector2D(20.5f * DungeonMap.TileSize, 5.5f * DungeonMap.TileSize),
        new Vector2D(5.5f * DungeonMap.TileSize, 9.5f * DungeonMap.TileSize),
        new Vector2D(9.5f * DungeonMap.TileSize, 12.5f * DungeonMap.TileSize),
        new Vector2D(5.5f * DungeonMap.TileSize, 12.5f * DungeonMap.TileSize),
        new Vector2D(15.5f * DungeonMap.TileSize, 9.5f * DungeonMap.TileSize),
        new Vector2D(20.5f * DungeonMap.TileSize, 12.5f * DungeonMap.TileSize),
    };

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
        _dungeonMap = new DungeonMap();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _dungeonMap.LoadContent(GraphicsDevice);
        DebugDrawer.LoadContent(GraphicsDevice);

        _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });

        _projectileTexture = new Texture2D(GraphicsDevice, 1, 1);
        _projectileTexture.SetData(new[] { new Color(100, 200, 255) });

        var wizardTexture = Texture2D.FromFile(GraphicsDevice, "ModelSprites/Wizard.png");
        _navGraph = new NavGraph(_dungeonMap);
        _world = new GameWorld();

        _player = new Player(
            new Vector2D(1.5f * DungeonMap.TileSize, 1.5f * DungeonMap.TileSize),
            _world, wizardTexture, _dungeonMap
        );

        StartRound(1);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        HandleInput();
        UpdateEntities(delta);
        HandleCollisions();
        CheckRoundState();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();

        _dungeonMap.Draw(_spriteBatch);
        _player.Render(_spriteBatch);

        foreach (var skeleton in _skeletons)
            if (skeleton.IsActive)
                skeleton.Render(_spriteBatch);

        foreach (var projectile in _projectiles)
            if (projectile.IsActive)
                projectile.Render(_spriteBatch);

        if (_debugMode)
        {
            _player.RenderDebug(_spriteBatch);
            foreach (var skeleton in _skeletons)
                if (skeleton.IsActive)
                    skeleton.RenderDebug(_spriteBatch);
        }

        DrawHUD();

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    private void HandleInput()
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.F1) && _previousKeyState.IsKeyUp(Keys.F1))
            _debugMode = !_debugMode;
        _previousKeyState = keyState;

        var mouseState = Mouse.GetState();
        if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
        {
            var direction = new Vector2D(mouseState.X, mouseState.Y).Sub(_player.Pos);
            if (!direction.IsZero())
                _projectiles.Add(new Projectile(_player.Pos.Clone(), direction, _world, _dungeonMap, _projectileTexture));
        }
        _previousMouseState = mouseState;
    }

    private void UpdateEntities(float delta)
    {
        _player.Update(delta);

        foreach (var skeleton in _skeletons)
            if (skeleton.IsActive)
                skeleton.Update(delta);

        foreach (var projectile in _projectiles)
            if (projectile.IsActive)
                projectile.Update(delta);
    }

    private void HandleCollisions()
    {
        //player contact damage
        foreach (var skeleton in _skeletons)
            if (skeleton.IsActive && skeleton.Overlaps(_player))
                _player.TryTakeDamage(Skeleton.ContactDamage);

        //projectile damge skeleotns
        foreach (var projectile in _projectiles)
        {
            if (!projectile.IsActive) continue;

            foreach (var skeleton in _skeletons)
            {
                if (!skeleton.IsActive) continue;
                if (projectile.Overlaps(skeleton))
                {
                    skeleton.TakeDamage(Projectile.Damage);
                    projectile.IsActive = false;
                    break;
                }
            }
        }

        _projectiles.RemoveAll(p => !p.IsActive);
    }


    private void StartRound(int round)
    {
        _currentRound = round;
        _skeletons.Clear();
        _projectiles.Clear();

        for (int i = 0; i < round; i++)
        {
            var spawnPos = SpawnPoints[i % SpawnPoints.Length].Clone();
            _skeletons.Add(new Skeleton(spawnPos, _world, _player, _dungeonMap, _navGraph, GraphicsDevice));
        }
    }

    private void CheckRoundState()
    {
        if (_player.HP <= 0)
        {
            _player.HP = 100;
            _player.Pos.X = 1.5f * DungeonMap.TileSize;
            _player.Pos.Y = 1.5f * DungeonMap.TileSize;
            StartRound(1);
        }
        else if (_skeletons.TrueForAll(s => !s.IsActive))
        {
            _player.HP = 100;
            StartRound(_currentRound + 1);
        }
    }

    private void DrawHUD()
    {
        int barWidth = 200;
        int barHeight = 16;
        int barX = (DungeonMap.Cols * DungeonMap.TileSize - barWidth) / 2;
        int barY = DungeonMap.Rows * DungeonMap.TileSize - barHeight - 8;
        float hpRatio = _player.HP / 100f;

        _spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, barWidth, barHeight), new Color(40, 40, 40));
        _spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, (int)(barWidth * hpRatio), barHeight), Color.Red);
    }
}
