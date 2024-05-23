using Enums;
using Miscs;
using UnityEngine;

namespace Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_SlotColumnProperties", menuName = "SlotColumnProperties", order = 0)]
    public class SlotColumnProperties : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<SlotColumnStopType, float> _stopSpeeds;
    }
}