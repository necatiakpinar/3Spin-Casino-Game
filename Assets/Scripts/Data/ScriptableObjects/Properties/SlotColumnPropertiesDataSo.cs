using Enums;
using Miscs;
using UnityEngine;

namespace Data.ScriptableObjects.Properties
{
    [CreateAssetMenu(fileName = "SO_SlotColumnProperties", menuName = "Data/ScriptableObjects/Properties/SlotColumnProperties", order = 0)]
    public class SlotColumnPropertiesDataSo : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<SlotColumnStopType, int> _stopSpeeds;
        [SerializeField] private int _middleSlotIndex = 2;
        [SerializeField] private int _spinSpeed = 45;

        public SerializableDictionary<SlotColumnStopType, int> StopSpeeds => _stopSpeeds;
        public int MiddleSlotIndex => _middleSlotIndex;
        public int SpinSpeed => _spinSpeed;
    }
}