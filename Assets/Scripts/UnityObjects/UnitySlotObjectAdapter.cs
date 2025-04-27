using Abstractions;
using Cysharp.Threading.Tasks;
using Enums;
using Interfaces;

namespace UnityObjects
{
    public class UnitySlotObjectAdapter : ISlotObject
    {
        public SlotObjectMono SlotObjectMonoRef => _slotObjectMono;
        public SlotObjectType Type => _slotObjectMono.Type;
        public ITransform Transform { get; }
        
        private readonly SlotObjectMono _slotObjectMono;
        private readonly ILogger _logger;

        public UnitySlotObjectAdapter(SlotObjectMono slotObject, ILogger logger)
        {
            _slotObjectMono = slotObject;
            _logger = logger;
            if (_slotObjectMono == null)
            {
                _logger.LogError("SlotObjectMono component not found on GameObject!");
                return;
            }

            Transform = new UnityTransform(slotObject.transform);
        }

        public void SetSprite(bool isBlurred)
        {
            _slotObjectMono.SetSprite(isBlurred);
        }

        public async UniTask MoveToTile(ITile targetTile, int speed)
        {
            if (targetTile is UnityTileAdapter adapter)
            {
                await _slotObjectMono.MoveToTile(adapter.TileMonoRef, speed);
            }
            else
            {
                _logger.LogError("Target tile is not a UnityTileAdapter!");
            }
        }
    }
}