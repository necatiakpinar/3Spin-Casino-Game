using Enums;
using UnityEngine;

namespace Data.ScriptableObjects.Properties
{
    [CreateAssetMenu(fileName = "SO_SlotObjectProperties", menuName = "Data/ScriptableObjects/Properties/SlotObjectProperties", order = 0)]
    public class SlotObjectPropertiesDataSo : ScriptableObject
    {
        [SerializeField] private string _blurredSuffix = "_Blur";
        [SerializeField] private float _blurredSpriteSpeedThreshold = 0.2f;
        public string BlurredSuffix => _blurredSuffix;
        public float BlurredSpriteSpeedThreshold => _blurredSpriteSpeedThreshold;
        
        public readonly float MilliSeconds = 1000;
    }
}