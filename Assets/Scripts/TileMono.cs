using Abstractions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TileMono : MonoBehaviour
{
    private Vector2Int _coordinates;
    private BaseSlotObjectMono _slotObjectMono;
    public Vector2Int Coordinates => _coordinates;
    public BaseSlotObjectMono SlotObjectMono => _slotObjectMono;
        
    public void Init(Vector2Int coordinates, BaseSlotObjectMono slotObjectMono)
    {
        _coordinates = coordinates;
        _slotObjectMono = slotObjectMono;
        slotObjectMono.transform.localPosition = Vector2.zero;
    }
        
    public async UniTask DropObjectToBottom(TileMono bottomTile, int speed)
    {
        await _slotObjectMono.MoveToTile(bottomTile, speed);
    }

    public void SetSlotObject(BaseSlotObjectMono slotObjectMono)
    {
        _slotObjectMono = slotObjectMono;
    }
}