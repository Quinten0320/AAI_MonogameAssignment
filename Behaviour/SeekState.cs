using Project.Entities;

namespace Project.Behaviour
{
    public class SeekState : IState
    {
        private readonly Skeleton _skeleton;

        public SeekState(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }

        public void Enter()
        {
            _skeleton.CurrentSteering = _skeleton.SeekBehaviour;
            _skeleton.MaxSpeed = Skeleton.ChaseSpeed;
        }

        public void Execute(float deltaTime)
        {
            float dist = _skeleton.Pos.DistanceTo(_skeleton.Player.Pos);

            if (dist > Skeleton.DetectionRange)
            {
                _skeleton.StateMachine.ChangeState(_skeleton.WanderState);
                return;
            }

            if (_skeleton.HP < Skeleton.FleeHPThreshold)
            {
                _skeleton.StateMachine.ChangeState(_skeleton.FleeState);
            }
        }

        public void Exit() { }
    }
}
