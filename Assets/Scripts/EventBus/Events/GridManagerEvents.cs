using System.Collections.Generic;
using Interfaces;

namespace EventBus.Events
{
    public struct TilesCreatedEvent : IEvent
    {
        public readonly Dictionary<int, List<TileMono>> GridDictionary;

        public TilesCreatedEvent(Dictionary<int, List<TileMono>> gridDictionary)
        {
            GridDictionary = gridDictionary;
        }
    }
}