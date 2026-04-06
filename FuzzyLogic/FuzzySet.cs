namespace Project.FuzzyLogic
{
    public abstract class FuzzySet
    {
        public string Name { get; }

        protected FuzzySet(string name)
        {
            Name = name;
        }

        public abstract float GetDOM(float value);

        public abstract float RepresentativeValue { get; }
    }

    public class TriangleFuzzySet : FuzzySet
    {
        private readonly float _left;
        private readonly float _peak;
        private readonly float _right;

        public TriangleFuzzySet(string name, float left, float peak, float right)
            : base(name)
        {
            _left = left;
            _peak = peak;
            _right = right;
        }

        public override float RepresentativeValue => _peak;

        public override float GetDOM(float value)
        {
            if (value < _left || value > _right)
                return 0f;
            if (value == _peak)
                return 1f;
            if (value < _peak)
                return (value - _left) / (_peak - _left);
            return (_right - value) / (_right - _peak);
        }
    }

    public class TrapezoidFuzzySet : FuzzySet
    {
        private readonly float _left;
        private readonly float _leftPeak;
        private readonly float _rightPeak;
        private readonly float _right;

        public TrapezoidFuzzySet(string name, float left, float leftPeak, float rightPeak, float right)
            : base(name)
        {
            _left = left;
            _leftPeak = leftPeak;
            _rightPeak = rightPeak;
            _right = right;
        }

        public override float RepresentativeValue => (_leftPeak + _rightPeak) / 2f;

        public override float GetDOM(float value)
        {
            if (value < _left || value > _right)
                return 0f;
            if (value >= _leftPeak && value <= _rightPeak)
                return 1f;
            if (value < _leftPeak)
                return (value - _left) / (_leftPeak - _left);
            return (_right - value) / (_right - _rightPeak);
        }
    }
}
