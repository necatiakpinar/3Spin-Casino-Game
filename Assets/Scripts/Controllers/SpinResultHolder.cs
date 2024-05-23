using Data;

namespace Controllers
{
    public class SpinResultHolder
    {
        private int _spinIndex;
        private ResultData _result;
        private bool _isLocked = false;
        
        public int SpinIndex => _spinIndex;
        public ResultData Result { get => _result; set => _result = value; }
        public bool IsLocked => _isLocked;
        
        public SpinResultHolder(int spinIndex, ResultData result)
        {
            _spinIndex = spinIndex;
            _result = result;
            
        }
        
        public void SetResult(ResultData name)
        {
            if (_isLocked)
                return;
            
            _result = name;
            _isLocked = true;
        }
    }
}