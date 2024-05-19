using System;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ResultInterval
    {
        private int _minIndex;
        private int _maxIndex;
        private int _selectedIntervalIndex;
        private bool _isSelected = false;
        
        public int MinIndex => _minIndex;
        public int MaxIndex => _maxIndex;
        public int SelectedIntervalIndex => _selectedIntervalIndex;
        public bool IsSelected => _isSelected;
        public ResultInterval(int minIndex, int maxIndex)
        {
            _minIndex = minIndex;
            _maxIndex = maxIndex;
        }
        
        public void SetSelected(int selectedIntervalIndex)
        {
            _selectedIntervalIndex = selectedIntervalIndex;
            _isSelected = true;
        }
        
    }
}