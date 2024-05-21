using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ResultInterval
    {
        [SerializeField] private int _minIndex;
        [SerializeField] private int _maxIndex;
        [SerializeField] private int _selectedIntervalIndex = -1;
        
        public int MinIndex => _minIndex;
        public int MaxIndex => _maxIndex;
        public int SelectedIntervalIndex => _selectedIntervalIndex;
        public ResultInterval(int minIndex, int maxIndex)
        {
            _minIndex = minIndex;
            _maxIndex = maxIndex;
        }
        
        public void SetSelected(int selectedIntervalIndex)
        {
            _selectedIntervalIndex = selectedIntervalIndex;
        }
        
    }
}