using Data;

namespace Managers
{
    public class ResultIndexHolder
    {
        private int _index;
        private string _result;
        private bool _isLocked = false;
        
        public int Index => _index;
        public string Result => _result;
        public bool IsLocked => _isLocked;
        
        public ResultIndexHolder(int index, string result)
        {
            _index = index;
            _result = result;
        }
        
        public void SetResult(string name)
        {
            if (_isLocked)
                return;
            
            _result = name;
            _isLocked = true;
        }
    }
}