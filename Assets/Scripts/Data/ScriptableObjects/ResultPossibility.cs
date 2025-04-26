using System;
using System.Collections.Generic;
using Enums;

namespace Data.ScriptableObjects
{
    [Serializable]
    public struct ResultPossibility
    {
        public List<SlotObjectType> TargetTypes;
        public int Possibility;
        public string Name => string.Join(", ", TargetTypes);
    }
}