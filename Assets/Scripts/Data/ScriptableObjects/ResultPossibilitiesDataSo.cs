using System;
using System.Collections.Generic;
using Enums;
using Interfaces;
using UnityEngine;

namespace Data.ScriptableObjects
{
    [Serializable]
    public struct ResultPossibility
    {
        public List<SlotObjectType> TargetTypes;
        public int Possibility;
        public string Name => string.Join(", ", TargetTypes);
        
    }
    [CreateAssetMenu(fileName = "SO_ResultPossibilitiesData", menuName = "Data/ScriptableObjects/Properties/ResultPossibilitiesData", order = 0)]
    public class ResultPossibilitiesDataSo : ScriptableObject, IResultPossibilityProvider
    {
        [SerializeField] private List<ResultPossibility> _resultPossibilities;
        
        public List<ResultPossibility> ResultPossibilities => _resultPossibilities;
        public List<ResultPossibility> GetResultPossibilities() => _resultPossibilities;
    }
}