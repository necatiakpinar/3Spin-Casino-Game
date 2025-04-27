using Cysharp.Threading.Tasks;
using Interfaces;
using UnityEngine;
using Vector2Int = Data.Vector2Int;

namespace UnityObjects
{
    public class UnityTileAdapter : ITile
    {
        private readonly TileMono _tileMono;
        private readonly ISlotObject _slotObjectMono;
        public ITransform Transform { get; }

        public TileMono TileMonoRef => _tileMono; 

        public Data.Vector2Int Coordinates => _tileMono.Coordinates;
        public ISlotObject SlotObject => _tileMono.SlotObject;

        public UnityTileAdapter(TileMono tileMono)
        {
            _tileMono = tileMono;
            Transform = new UnityTransform(_tileMono.transform);
        }

        public void Init(Vector2Int coordinates, ISlotObject slotObject)
        {
            if (slotObject is UnitySlotObjectAdapter adapter)
            {
                _tileMono.Init(coordinates, adapter.SlotObjectMonoRef);
            }
            else
            {
                Debug.LogError("SlotObject must be a UnitySlotObjectAdapter!");
            }
        }

        public void SetSlotObject(ISlotObject slotObject)
        {
            if (slotObject is UnitySlotObjectAdapter adapter)
            {
                _tileMono.SetSlotObject(adapter.SlotObjectMonoRef);
            }
            else
            {
                Debug.LogError("SlotObject must be a UnitySlotObjectAdapter!");
            }
        }

        public async UniTask DropObjectToBottom(ITile bottomTile, int speed)
        {
            if (bottomTile is UnityTileAdapter adapter)
            {
                await _tileMono.DropObjectToBottom(adapter.TileMonoRef, speed);
            }
            else
            {
                Debug.LogError("Bottom tile must be a UnityTileAdapter!");
            }
        }
    }
}