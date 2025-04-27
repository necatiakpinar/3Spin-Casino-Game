using Cysharp.Threading.Tasks;
using Enums;

namespace Interfaces
{
    public interface ISlotObject
    {
        SlotObjectType Type { get; }
        ITransform Transform { get; }
        void SetSprite(bool isBlurred);
        UniTask MoveToTile(ITile targetTile, int speed);
    }
}