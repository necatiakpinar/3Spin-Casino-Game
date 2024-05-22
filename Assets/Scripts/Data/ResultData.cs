using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Data
{
    [Serializable]
    public class ResultData
    {
        [SerializeField] private List<SlotObjectType> _resultObjects;
        [SerializeField] private List<ResultInterval> _intervals;
        [SerializeField] private int _possibility;

        public string Name => string.Join(", ", _resultObjects);
        
        public List<SlotObjectType> ResultObjects => _resultObjects;
        public List<ResultInterval> Intervals { get => _intervals; set => _intervals = value;}
        public int Possibility => _possibility;
        
        public ResultData(List<SlotObjectType> resultObjects, List<ResultInterval> intervals, int possibility)
        {
            _resultObjects = resultObjects;
            _intervals = intervals;
            _possibility = possibility;
        }
        
    }
}