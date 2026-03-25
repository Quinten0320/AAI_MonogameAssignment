using AAI_MonogameAssignment;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Behaviour;
using Project.Entities.BaseEntity;
using Project.PathFinding;
using Project.Steering;

namespace Project.Entities
{
    public class Skeleton : MovingEntity
    {
        public const float DetectionRange = 5f * DungeonMap.TileSize;
        public const int FleeHPThreshold = 20;
        public const float ChaseSpeed = 120f;
        public const float WanderSpeed = 50f;
        public const int ContactDamage = 25;
        private const float DebugLineLength = 50f;

        public int HP { get; set; } = 100;
        public Player Player { get; }
        public DungeonMap Map { get; }

        public StateMachine StateMachine { get; }
        public SteeringBehaviour CurrentSteering { get; set; }

        public SeekBehaviour SeekBehaviour { get; }
        public FleeBehaviour FleeBehaviour { get; }
        public WanderBehaviour WanderBehaviour { get; }

        public WanderState WanderState { get; }
        public SeekState SeekState { get; }
        public FleeState FleeState { get; }

        private readonly Texture2D _texture;
        private readonly Texture2D _pixelTexture;

        public Skeleton(Vector2D pos, GameWorld world, Player player, DungeonMap map, NavGraph navGraph, GraphicsDevice graphicsDevice)
            : base(pos, world, maxSpeed: 120f, radius: 12f)
        {
            Player = player;
            Map = map;

            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { new Color(160, 50, 200) });

            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            SeekBehaviour = new SeekBehaviour(player.Pos, navGraph);
            FleeBehaviour = new FleeBehaviour(player.Pos, navGraph);
            WanderBehaviour = new WanderBehaviour(navGraph);

            WanderState = new WanderState(this);
            SeekState = new SeekState(this);
            FleeState = new FleeState(this);

            StateMachine = new StateMachine();
            StateMachine.ChangeState(WanderState);
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
            StateMachine.Update(deltaTime);

            if (CurrentSteering != null)
            {
                Vector2D force = CurrentSteering.Calculate(this);

                Vector2D acceleration = force.Clone().Divide(Mass);
                Velocity.Add(acceleration.Multiply(deltaTime));
                Velocity.Truncate(MaxSpeed);

                float moveX = Velocity.X * deltaTime;
                if (!Map.CollidesWithWall(Pos.X + moveX, Pos.Y, Radius))
                    Pos.X += moveX;
                else
                    Velocity.X = 0;

                float moveY = Velocity.Y * deltaTime;
                if (!Map.CollidesWithWall(Pos.X, Pos.Y + moveY, Radius))
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

        public override void Render(SpriteBatch spriteBatch)
        {
            int size = (int)(Radius * 2);
            var dest = new Rectangle((int)(Pos.X - Radius), (int)(Pos.Y - Radius), size, size);
            spriteBatch.Draw(_texture, dest, Color.White);

            //HP
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
            //Velocity vector
            if (!Velocity.IsZero())
            {
                Vector2D end = Pos.Clone().Add(Velocity.Clone().Normalize().Multiply(DebugLineLength));
                DebugDrawer.DrawLine(spriteBatch, Pos, end, Color.LimeGreen);
            }

            //A* path
            if (CurrentSteering == null) return;
            var path = CurrentSteering.GetCurrentPath();
            int pathIndex = CurrentSteering.GetCurrentPathIndex();
            if (path == null || pathIndex >= path.Count) return;

            //line to first waypoint
            var firstWp = NodeWorldPos(path[pathIndex]);
            DebugDrawer.DrawLine(spriteBatch, Pos, firstWp, Color.Red);

            //line to waypoints
            for (int i = pathIndex; i < path.Count - 1; i++)
            {
                var from = NodeWorldPos(path[i]);
                var to = NodeWorldPos(path[i + 1]);
                DebugDrawer.DrawLine(spriteBatch, from, to, Color.Red);
            }
        }

        private static Vector2D NodeWorldPos(NodeBase node)
        {
            return new Vector2D(
                (node.Col + 0.5f) * DungeonMap.TileSize,
                (node.Row + 0.5f) * DungeonMap.TileSize
            );
        }
    }
}
