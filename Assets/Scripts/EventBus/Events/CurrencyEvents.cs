using Enums;
using Interfaces;
using Miscs;

namespace EventBus.Events
{
    public struct CurrencyUpdatedEvent : IEvent
    {
        public readonly CurrencyType Type;
        public int Amount;

        public CurrencyUpdatedEvent(CurrencyType type, int amount)
        {
            Type = type;
            Amount = amount;
        }
    }
}