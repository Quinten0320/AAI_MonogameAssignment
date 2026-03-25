using Project.Entities;

namespace Project.Behaviour
{
    public class FleeState : IState
    {
        private readonly Skeleton _skeleton;

        public FleeState(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }

        public void Enter()
        {
            _skeleton.CurrentSteering = _skeleton.FleeBehaviour;
            _skeleton.MaxSpeed = Skeleton.ChaseSpeed;
        }

        public void Execute(float deltaTime)
        {
            float dist = _skeleton.Pos.DistanceTo(_skeleton.Player.Pos);

            if (dist > Skeleton.DetectionRange)
            {
                _skeleton.StateMachine.ChangeState(_skeleton.WanderState);
            }
        }

        public void Exit() { }
    }
}
