using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;

namespace Data.ScriptableObjects
{
    [Serializable]
    public struct ResultPossibility
    {
        public List<SlotObjectType> TargetTypes;
        public int Possibility;
        
    }
    [CreateAssetMenu(fileName = "SO_ResultPossibilitiesData", menuName = "Data/ScriptableObjects/ResultPossibilitiesData")]
    public class ResultPossibilitiesDataSo : ScriptableObject
    {
        [SerializeField] private List<ResultPossibility> _resultPossibilities;
        
        public List<ResultPossibility> ResultPossibilities => _resultPossibilities;
    }
}