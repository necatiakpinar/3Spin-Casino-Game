using System.Collections.Generic;
using System.Linq;
using Enums;
using Miscs;
using UnityEngine;

namespace Data.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SO_SlotObjectCurrenciesData", menuName = "Data/ScriptableObjects/SlotObjectCurrenciesData")]
    public class SlotObjectCurrenciesDataSo : ScriptableObject
    {
        [SerializeField] private SerializableDictionary<CurrencyType, List<SlotObjectCurrencyMultiplier>> _slotObjectCurrencyMultipliers;

        public SlotObjectCurrencyMultiplier GetSlotObjectCurrencyMultipliers(CurrencyType currencyType, SlotObjectType slotObjectType)
        {
            return _slotObjectCurrencyMultipliers[currencyType].FirstOrDefault(x => x.SlotObjectType == slotObjectType);
        }
    }
}