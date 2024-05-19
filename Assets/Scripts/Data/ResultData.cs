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
        
        public List<SlotObjectType> ResultObjects => _resultObjects;
        public List<ResultInterval> Intervals => _intervals;
        
        public ResultData(List<SlotObjectType> resultObjects, List<ResultInterval> intervals)
        {
            _resultObjects = resultObjects;
            _intervals = intervals;
        }
    }
}