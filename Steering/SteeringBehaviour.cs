using System.Collections.Generic;
using AAI_MonogameAssignment;
using Project.Entities.BaseEntity;
using Project.PathFinding;

namespace Project.Steering
{
    public abstract class SteeringBehaviour
    {
        public abstract Vector2D Calculate(MovingEntity entity);
        public virtual List<NodeBase> GetCurrentPath() => null;
        public virtual int GetCurrentPathIndex() => 0;

        protected static Vector2D SeekToward(MovingEntity entity, Vector2D target)
        {
            Vector2D desired = target.Clone().Sub(entity.Pos).Normalize().Multiply(entity.MaxSpeed);
            return desired.Sub(entity.Velocity);
        }
    }
}
