using Abstractions;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TileMono : MonoBehaviour
{
    private Vector2Int _coordinates;
    private SlotObjectMono _slotObjectMono;
    public Vector2Int Coordinates => _coordinates;
    public SlotObjectMono SlotObjectMono => _slotObjectMono;
        
    public void Init(Vector2Int coordinates, SlotObjectMono slotObjectMono)
    {
        _coordinates = coordinates;
        _slotObjectMono = slotObjectMono;
        slotObjectMono.transform.localPosition = Vector2.zero;
    }
        
    public async UniTask DropObjectToBottom(TileMono bottomTile, int speed)
    {
        await _slotObjectMono.MoveToTile(bottomTile, speed);
    }

    public void SetSlotObject(SlotObjectMono slotObjectMono)
    {
        _slotObjectMono = slotObjectMono;
    }
}