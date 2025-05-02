using Cysharp.Threading.Tasks;
using Data;

namespace Interfaces
{
    public interface ITile
    {
        Vector2Int Coordinates { get; }
        ISlotObject SlotObject { get; }
        ITransform Transform { get; }
        void Init(Vector2Int coordinates, ISlotObject slotObject);
        void SetSlotObject(ISlotObject slotObject);
        void DropObjectToBottom(ITile bottomTile, int speed);
    }
}