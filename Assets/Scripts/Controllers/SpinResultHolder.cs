using Data;

namespace Controllers
{
    public class SpinResultHolder
    {
        private int _spinIndex;
        private ResultData _result;

        public int SpinIndex => _spinIndex;

        public ResultData Result
        {
            get => _result;
            set => _result = value;
        }

        public SpinResultHolder(int spinIndex, ResultData result)
        {
            _spinIndex = spinIndex;
            _result = result;
        }
    }
}