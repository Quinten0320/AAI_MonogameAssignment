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
    }
}
