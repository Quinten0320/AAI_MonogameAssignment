using System.Collections.Generic;
using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Behaviour;
using Project.Entities.BaseEntity;
using Project.FuzzyLogic;
using Project.PathFinding;
using Project.Steering;

namespace Project.Entities
{
    public class Skeleton : MovingEntity
    {
        public const int ContactDamage = 25;
        private const float DebugLineLength = 50f;

        public int HP { get; set; } = 100;
        public float LastAggression { get; private set; }

        private readonly Player _player;
        private readonly DungeonMap _map;
        private readonly ScriptedStateMachine _scriptedSM;
        private readonly FuzzyEngine _fuzzyEngine;
        private SteeringBehaviour _currentSteering;
        private readonly SeekBehaviour _seekBehaviour;
        private readonly FleeBehaviour _fleeBehaviour;
        private readonly WanderBehaviour _wanderBehaviour;
        private readonly HideBehaviour _hideBehaviour;
        private readonly ObstacleAvoidance _obstacleAvoidance;
        private readonly Dictionary<string, SteeringBehaviour> _steeringMap;
        private readonly Texture2D _texture;
        private readonly Texture2D _pixelTexture;

        public Skeleton(Vector2D pos, Player player, DungeonMap map, NavGraph navGraph,
            GraphicsDevice graphicsDevice, ScriptedStateMachine scriptedSM, FuzzyEngine fuzzyEngine)
            : base(pos, maxSpeed: 120f, radius: 12f)
        {
            _player = player;
            _map = map;
            _scriptedSM = scriptedSM;
            _fuzzyEngine = fuzzyEngine;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { new Color(160, 50, 200) });

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _seekBehaviour = new SeekBehaviour(player.Pos, navGraph);
            _fleeBehaviour = new FleeBehaviour(player.Pos, navGraph);
            _wanderBehaviour = new WanderBehaviour(navGraph);
            _hideBehaviour = new HideBehaviour(player.Pos, navGraph, map);
            _obstacleAvoidance = new ObstacleAvoidance(map);

            _steeringMap = new Dictionary<string, SteeringBehaviour>
            {
                { "Seek", _seekBehaviour },
                { "Flee", _fleeBehaviour },
                { "Wander", _wanderBehaviour },
                { "Hide", _hideBehaviour }
            };

            ApplyStateConfig();
        }

        public void TakeDamage(int amount)
        {
            HP -= amount;
            if (HP <= 0)
            {
                HP = 0;
                IsActive = false;
            }
        }

        public override void Update(float deltaTime)
        {
            float distance = Pos.DistanceTo(_player.Pos);
            var inputs = new Dictionary<string, float>
            {
                { "Distance", distance },
                { "Health", HP }
            };
            var fuzzyResult = _fuzzyEngine.Evaluate(inputs);
            LastAggression = fuzzyResult.ContainsKey("Aggression") ? fuzzyResult["Aggression"] : 50f;

            bool stateChanged = _scriptedSM.Update(LastAggression, distance);
            if (stateChanged)
                ApplyStateConfig();

            if (_currentSteering != null)
            {
                Vector2D primaryForce = _currentSteering.Calculate(this);
                Vector2D avoidanceForce = _obstacleAvoidance.Calculate(this);

                Vector2D totalForce = primaryForce.Add(avoidanceForce);
                totalForce.Truncate(MaxForce);

                Vector2D acceleration = totalForce.Clone().Divide(Mass);
                Velocity.Add(acceleration.Multiply(deltaTime));
                Velocity.Truncate(MaxSpeed);

                float moveX = Velocity.X * deltaTime;
                if (!_map.CollidesWithWall(Pos.X + moveX, Pos.Y, Radius))
                    Pos.X += moveX;
                else
                    Velocity.X = 0;

                float moveY = Velocity.Y * deltaTime;
                if (!_map.CollidesWithWall(Pos.X, Pos.Y + moveY, Radius))
                    Pos.Y += moveY;
                else
                    Velocity.Y = 0;

                if (!Velocity.IsZero())
                {
                    Direction = Velocity.Clone().Normalize();
                    Side = Direction.GetSide();
                }
            }
        }

        private void ApplyStateConfig()
        {
            var config = _scriptedSM.CurrentStateConfig;
            MaxSpeed = config.Speed;
            if (_steeringMap.TryGetValue(config.Steering, out var steering))
                _currentSteering = steering;
        }

        public override void Render(SpriteBatch spriteBatch)
        {
            int size = (int)(Radius * 2);
            var dest = new Rectangle((int)(Pos.X - Radius), (int)(Pos.Y - Radius), size, size);
            spriteBatch.Draw(_texture, dest, Color.White);

            int barWidth = size + 4;
            int barHeight = 4;
            int barX = (int)(Pos.X - barWidth / 2f);
            int barY = (int)(Pos.Y - Radius) - barHeight - 3;
            float hpRatio = HP / 100f;

            spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, barWidth, barHeight), new Color(40, 40, 40));
            spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, (int)(barWidth * hpRatio), barHeight), Color.Red);
        }

        public override void RenderDebug(SpriteBatch spriteBatch)
        {
            if (!Velocity.IsZero())
            {
                Vector2D end = Pos.Clone().Add(Velocity.Clone().Normalize().Multiply(DebugLineLength));
                DebugDrawer.DrawLine(spriteBatch, Pos, end, Color.LimeGreen);
            }

            if (_currentSteering == null) return;
            var path = _currentSteering.GetCurrentPath();
            int pathIndex = _currentSteering.GetCurrentPathIndex();
            if (path == null || pathIndex >= path.Count) return;

            DebugDrawer.DrawLine(spriteBatch, Pos, path[pathIndex].WorldPos, Color.Red);

            for (int i = pathIndex; i < path.Count - 1; i++)
                DebugDrawer.DrawLine(spriteBatch, path[i].WorldPos, path[i + 1].WorldPos, Color.Red);
        }

        public void RenderDebugText(SpriteBatch spriteBatch, SpriteFont font)
        {
            string stateName = _scriptedSM.CurrentStateName;
            string info = $"{stateName} A:{LastAggression:F0}";

            Vector2 textSize = font.MeasureString(info);
            Vector2 textPos = new Vector2(Pos.X - textSize.X / 2f, Pos.Y - Radius - 20);
            spriteBatch.DrawString(font, info, textPos, Color.White);
        }

    }
}
