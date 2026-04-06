using System.Collections.Generic;
using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Behaviour;
using Project.Entities;
using Project.FuzzyLogic;
using Project.PathFinding;

namespace Project.Scenes
{
    public class GameScene : IScene
    {
        private Game1 _game;
        private GraphicsDevice _graphicsDevice;
        private DungeonMap _dungeonMap;
        private NavGraph _navGraph;
        private Player _player;
        private readonly List<Skeleton> _skeletons = new List<Skeleton>();
        private readonly List<Projectile> _projectiles = new List<Projectile>();

        private Texture2D _projectileTexture;
        private Texture2D _pixelTexture;
        private Texture2D _skeletonTexture;
        private SpriteFont _font;
        private FuzzyEngine _fuzzyEngine;
        private int _currentRound;
        private bool _debugMode;
        private bool _showGraph;
        private KeyboardState _previousKeyState;
        private MouseState _previousMouseState;

        private Rectangle _pauseButton;

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

        public void LoadContent(Game1 game, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _game = game;
            _graphicsDevice = graphicsDevice;
            _font = game.DebugFont;

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _projectileTexture = new Texture2D(graphicsDevice, 1, 1);
            _projectileTexture.SetData(new[] { new Color(100, 200, 255) });

            _dungeonMap = new DungeonMap();
            _dungeonMap.LoadContent(graphicsDevice);

            var wizardTexture = Texture2D.FromFile(graphicsDevice, "ModelSprites/Wizard.png");
            _skeletonTexture = Texture2D.FromFile(graphicsDevice, "ModelSprites/Skeleton.png");
            _navGraph = new NavGraph(_dungeonMap);

            _fuzzyEngine = FuzzyEngine.LoadFromJson("Config/fuzzy_config.json");

            _player = new Player(
                new Vector2D(1.5f * DungeonMap.TileSize, 1.5f * DungeonMap.TileSize),
                wizardTexture, _dungeonMap
            );

            int btnSize = 28;
            int margin = 8;
            _pauseButton = new Rectangle(
                graphicsDevice.Viewport.Width - btnSize - margin,
                margin,
                btnSize, btnSize
            );

            StartRound(1);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            HandleInput();
            UpdateEntities(delta);
            HandleCollisions();
            CheckRoundState();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _dungeonMap.Draw(spriteBatch);

            if (_debugMode && _showGraph)
                DrawNavGraph(spriteBatch);

            _player.Render(spriteBatch);

            foreach (var skeleton in _skeletons)
                if (skeleton.IsActive)
                    skeleton.Render(spriteBatch);

            foreach (var projectile in _projectiles)
                if (projectile.IsActive)
                    projectile.Render(spriteBatch);

            if (_debugMode)
            {
                _player.RenderDebug(spriteBatch);
                foreach (var skeleton in _skeletons)
                {
                    if (!skeleton.IsActive) continue;
                    skeleton.RenderDebug(spriteBatch);
                    skeleton.RenderDebugText(spriteBatch, _font);
                }

                spriteBatch.DrawString(_font, "F1: Debug  F2: Graph", new Vector2(4, 4), Color.Gray);
            }

            DrawHUD(spriteBatch);
            DrawPauseButton(spriteBatch);
        }

        private void HandleInput()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.F1) && _previousKeyState.IsKeyUp(Keys.F1))
                _debugMode = !_debugMode;
            if (keyState.IsKeyDown(Keys.F2) && _previousKeyState.IsKeyUp(Keys.F2))
                _showGraph = !_showGraph;

            _previousKeyState = keyState;

            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
            {
                if (_pauseButton.Contains(mouseState.X, mouseState.Y))
                {
                    _game.PushScene(new PauseScene());
                    _previousMouseState = mouseState;
                    return;
                }

                var direction = new Vector2D(mouseState.X, mouseState.Y).Sub(_player.Pos);
                if (!direction.IsZero())
                    _projectiles.Add(new Projectile(_player.Pos.Clone(), direction, _dungeonMap, _projectileTexture));
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
            foreach (var skeleton in _skeletons)
                if (skeleton.IsActive && skeleton.Overlaps(_player))
                    _player.TryTakeDamage(Skeleton.ContactDamage);

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
                var sm = ScriptedStateMachine.LoadFromJson("Config/state_machine.json");
                _skeletons.Add(new Skeleton(spawnPos, _player, _dungeonMap, _navGraph,
                    _graphicsDevice, sm, _fuzzyEngine, _skeletonTexture));
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

        private void DrawNavGraph(SpriteBatch spriteBatch)
        {
            for (int row = 0; row < DungeonMap.Rows; row++)
            {
                for (int col = 0; col < DungeonMap.Cols; col++)
                {
                    var node = _navGraph.Grid[row, col];
                    if (!node.IsWalkable) continue;

                    var nodePos = new Vector2D(
                        (col + 0.5f) * DungeonMap.TileSize,
                        (row + 0.5f) * DungeonMap.TileSize
                    );

                    spriteBatch.Draw(_pixelTexture,
                        new Rectangle((int)nodePos.X - 1, (int)nodePos.Y - 1, 3, 3),
                        new Color(80, 80, 255, 80));

                    foreach (var neighbor in node.Neighbors)
                    {
                        if (!neighbor.IsWalkable) continue;
                        if (neighbor.Row < row || (neighbor.Row == row && neighbor.Col < col))
                            continue;

                        var neighborPos = new Vector2D(
                            (neighbor.Col + 0.5f) * DungeonMap.TileSize,
                            (neighbor.Row + 0.5f) * DungeonMap.TileSize
                        );
                        DebugDrawer.DrawLine(spriteBatch, nodePos, neighborPos, new Color(60, 60, 200, 40), 1f);
                    }
                }
            }
        }

        private void DrawPauseButton(SpriteBatch spriteBatch)
        {
            bool hover = _pauseButton.Contains(Mouse.GetState().X, Mouse.GetState().Y);
            Color bg = hover ? new Color(80, 80, 80, 200) : new Color(50, 50, 50, 160);
            spriteBatch.Draw(_pixelTexture, _pauseButton, bg);

            int barW = 4;
            int barH = _pauseButton.Height - 12;
            int y = _pauseButton.Y + 6;
            int gap = 4;
            int totalW = barW * 2 + gap;
            int startX = _pauseButton.X + (_pauseButton.Width - totalW) / 2;

            spriteBatch.Draw(_pixelTexture, new Rectangle(startX, y, barW, barH), Color.White);
            spriteBatch.Draw(_pixelTexture, new Rectangle(startX + barW + gap, y, barW, barH), Color.White);
        }

        private void DrawHUD(SpriteBatch spriteBatch)
        {
            int barWidth = 200;
            int barHeight = 16;
            int barX = (_graphicsDevice.Viewport.Width - barWidth) / 2;
            int barY = _graphicsDevice.Viewport.Height - barHeight - 8;
            float hpRatio = _player.HP / 100f;

            spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, barWidth, barHeight), new Color(40, 40, 40));
            spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, (int)(barWidth * hpRatio), barHeight), Color.Red);

            string roundText = $"Round {_currentRound}";
            Vector2 roundSize = _font.MeasureString(roundText);
            spriteBatch.DrawString(_font, roundText,
                new Vector2((_graphicsDevice.Viewport.Width - roundSize.X) / 2, barY - 18), Color.White);
        }
    }
}
