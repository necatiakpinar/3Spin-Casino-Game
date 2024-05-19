using System;
using System.Collections.Generic;
using Enums;

namespace Data
{
    [Serializable]
    public class ResultData
    {
        private List<SlotObjectType> _resultObjects;
        private List<ResultInterval> _intervals;
        private int _totalSelectedAmount = 0;
        private List<int> _selectedIntervalIndexes = new List<int>();
        
        public List<SlotObjectType> ResultObjects => _resultObjects;
        public List<ResultInterval> Intervals => _intervals;
        public int TotalSelectedAmount => _totalSelectedAmount;
        
        public ResultData(List<SlotObjectType> resultObjects, List<ResultInterval> intervals)
        {
            _resultObjects = resultObjects;
            _intervals = intervals;
        }
        
        public void TrySelectInterval(int spinIndex)
        {
            foreach (var interval in _intervals)
            {
                if (interval.IsSelected)
                    continue;
                
                if (spinIndex >= interval.MinIndex && spinIndex <= interval.MaxIndex)
                {
                    interval.SetSelected(spinIndex);
                    _totalSelectedAmount++;
                    _selectedIntervalIndexes.Add(spinIndex);
                }
            }
        }
        
        public bool IsIntervalSelected(int spinIndex)
        {
            foreach (var interval in _intervals)
                if (spinIndex >= interval.MinIndex && spinIndex <= interval.MaxIndex)
                    if (interval.IsSelected)
                        return true;
                    else
                        return false;

            return false;
        }
        
    }
}