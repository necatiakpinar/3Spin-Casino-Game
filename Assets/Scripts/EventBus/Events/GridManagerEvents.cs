using System.Collections.Generic;
using Interfaces;

namespace EventBus.Events
{
    public struct TilesCreatedEvent : IEvent
    {
        public readonly Dictionary<int, List<ITile>> GridDictionary;

        public TilesCreatedEvent(Dictionary<int, List<ITile>> gridDictionary)
        {
            GridDictionary = gridDictionary;
        }
    }
}