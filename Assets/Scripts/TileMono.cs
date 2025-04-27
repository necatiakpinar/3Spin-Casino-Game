using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using UnityObjects;

public class TileMono : MonoBehaviour, ITile
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
        _slotObjectMono.Transform.LocalPosition = Vector2.zero;
    }
    
    public void SetSlotObject(ISlotObject slotObject)
    {
        _slotObjectMono = slotObject;
    }

    public async UniTask DropObjectToBottom(ITile bottomTile, int speed)
    {
        await _slotObjectMono.MoveToTile(bottomTile, speed);
    }
}