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
        [SerializeField] private bool _isAddedIntoResultHolder = false;
        [SerializeField] private bool _isLocked = false;
        
        
        public int MinIndex { get => _minIndex; set => _minIndex = value; }
        public int MaxIndex { get => _maxIndex; set => _maxIndex = value; }
        public bool IsAddedIntoResultHolder { get => _isAddedIntoResultHolder; set => _isAddedIntoResultHolder = value; }
        public int SelectedIntervalIndex { get => _selectedIntervalIndex; set => _selectedIntervalIndex = value; }
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