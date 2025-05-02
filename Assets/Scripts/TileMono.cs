using Adapters;
using Cysharp.Threading.Tasks;
using Extensions;
using Interfaces;
using UnityEngine;
using UnityObjects;

public class TileMono : MonoBehaviour, ITile, ISpawnable
{
    private Data.Vector2Int _coordinates;
    private ISlotObject _slotObjectMono;

    public Data.Vector2Int Coordinates => _coordinates;
    public ISlotObject SlotObject => _slotObjectMono;
    public ITransform Transform => new UnityTransform(transform);

    public void Init(Data.Vector2Int coordinates, ISlotObject slotObject)
    {
        _coordinates = coordinates;
        _slotObjectMono = slotObject;
        _slotObjectMono.Transform.LocalPosition = new Vector3Adapter(Vector3.zero.ToDataVector3());
    }
    
    public void SetSlotObject(ISlotObject slotObject)
    {
        _slotObjectMono = slotObject;
    }

    public void DropObjectToBottom(ITile bottomTile, int speed)
    {
         _slotObjectMono.MoveToTile(bottomTile, speed);
    }
}