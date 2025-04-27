using Data;

namespace Interfaces
{
    public interface IObjectFactory
    {
        ITile CreateTile(object tilePrefab, ITransform parent, Vector3 localPosition);
        ISlotObject CreateSlotObject(object slotObjectPrefab, ITransform parent, Vector3 localPosition);
    }
}