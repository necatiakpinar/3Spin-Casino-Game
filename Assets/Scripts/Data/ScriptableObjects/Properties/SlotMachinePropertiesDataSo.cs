using Enums;
using UnityEngine;

namespace Data.ScriptableObjects.Properties
{
    [CreateAssetMenu(fileName = "SO_SlotMachinePropertiesData", menuName = "Data/ScriptableObjects/Properties/SlotMachineProperties", order = 0)]   
    public class SlotMachinePropertiesDataSo : ScriptableObject
    {
        [SerializeField] private SlotColumnStopType _firstTwoStopType = SlotColumnStopType.Fast;
        [SerializeField] private int _spinStartingDelay = 75;
        [SerializeField] private int _spinDuration = 1000;
        
        public SlotColumnStopType FirstTwoStopType => _firstTwoStopType;
        public int SpinStartingDelay => _spinStartingDelay;
        public int SpinDuration => _spinDuration;
        
    }
}