using System;
using System.Collections.Generic;
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
    }
}