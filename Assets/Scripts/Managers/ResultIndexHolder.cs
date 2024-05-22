using Data;

namespace Managers
{
    public class ResultIndexHolder
    {
        private int _index;
        private ResultData _result;
        private bool _isLocked = false;
        
        public int Index => _index;
        public ResultData Result => _result;
        public bool IsLocked => _isLocked;
        
        public ResultIndexHolder(int index, ResultData result)
        {
            _index = index;
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