using Project.Entities;

namespace Project.Behaviour
{
    public class WanderState : IState
    {
        private readonly Skeleton _skeleton;

        public WanderState(Skeleton skeleton)
        {
            _skeleton = skeleton;
        }

        public void Enter()
        {
            _skeleton.CurrentSteering = _skeleton.WanderBehaviour;
            _skeleton.MaxSpeed = Skeleton.WanderSpeed;
        }

        public void Execute(float deltaTime)
        {
            float dist = _skeleton.Pos.DistanceTo(_skeleton.Player.Pos);

            if (dist <= Skeleton.DetectionRange)
            {
                if (_skeleton.HP < Skeleton.FleeHPThreshold)
                    _skeleton.StateMachine.ChangeState(_skeleton.FleeState);
                else
                    _skeleton.StateMachine.ChangeState(_skeleton.SeekState);
            }
        }

        public void Exit() { }
    }
}
