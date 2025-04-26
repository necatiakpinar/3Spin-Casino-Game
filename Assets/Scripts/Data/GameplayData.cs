using System;
using System.Collections.Generic;
using Abstractions;
using Data.Currencies;
using Data.PersistentData;
using Enums;
using Miscs;

namespace Data
{
    [Serializable]
    public class GameplayData
    {
        public int TotalSpinRatio = 100;
        public int CurrentSpinIndex = 0;
        public List<ResultData> Results = new List<ResultData>();
        public SerializableDictionary<int, List<SlotObjectType>> ResultDictionary = new SerializableDictionary<int, List<SlotObjectType>>();
        public CurrencyDataController CurrencyDataController;

        public GameplayData()
        {
            CurrencyDataController = new CurrencyDataController();
        }
    }
}