using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_ResultPossibilitiesData", menuName = "Data/ScriptableObjects/Properties/ResultPossibilitiesData", order = 0)]
    public class ResultPossibilitiesDataSo : ScriptableObject, IResultPossibilityProvider
    {
        [SerializeField] private List<ResultPossibility> _resultPossibilities;
        
        public List<ResultPossibility> ResultPossibilities => _resultPossibilities;
        public List<ResultPossibility> GetResultPossibilities() => _resultPossibilities;
    }
}