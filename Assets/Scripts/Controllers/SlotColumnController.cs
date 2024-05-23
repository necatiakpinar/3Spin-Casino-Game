using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using Enums;
using UnityEngine;

namespace Controllers
{
    public class SlotColumnController
    {
        private List<TileMono> _tiles;
        private TileMono _middleSlot;
        
        private readonly int _spinSpeed = 45; 
        private readonly int _slowDownSpeed = 250; 
        private readonly int _middleSlotIndex = 2; 
        private readonly float _defaultSlotDuration = 500f;

        private SlotObjectType _targetSlotObjectType;

        public SlotColumnController(List<TileMono> tiles)
        {
            _tiles = tiles;
            _middleSlot = _tiles[_middleSlotIndex];
        }

        public async Task Spin(SlotObjectType objectType)
        {
            _targetSlotObjectType = objectType;
            
            await SpinForDuration();
            await SlowDownToStop(objectType);
        }

        private async Task SpinForDuration()
        {
            SetSlotObjectBlurVisibility(true);
            var startTime = Time.time;
            while (Time.time - startTime < _defaultSlotDuration / 1000f)
            {
                await DoMovement(_spinSpeed);
            }
        }

        private async Task SlowDownToStop(SlotObjectType objectType)
        {
            var isNearTarget = false;
            while (!IsSlotObjectInFirstTile(_middleSlot, objectType))
            {
                if (CheckProximityToTarget(objectType, 2) && !isNearTarget)
                {
                    isNearTarget = true; 
                    SetSlotObjectBlurVisibility(false);
                }
                await DoMovement(_spinSpeed);
            }
            
            SetSlotObjectBlurVisibility(false);
        }

        private bool CheckProximityToTarget(SlotObjectType objectType, int proximity)
        {
            var targetIndex = _tiles.FindIndex(t => t.SlotObjectMono.Type == objectType);
            return Mathf.Abs(targetIndex - _middleSlotIndex) <= proximity;
        }

        private async Task DoMovement(int speed)
        {
            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                var tile = _tiles[i];
                if (i - 1 >= 0)
                {
                    tile.DropObjectToBottom(_tiles[i - 1], speed);
                }
                else
                {
                    tile.DropObjectToBottom(_tiles[_tiles.Count - 1], speed);
                }
            }

            await Task.Delay(speed);
        }

        private bool IsSlotObjectInFirstTile(TileMono tile, SlotObjectType objectType)
        {
            return tile.Coordinates.y == _middleSlotIndex && tile.SlotObjectMono.Type == objectType;
        }
        
        public void SetSlotObjectBlurVisibility(bool isBlurred)
        {
            foreach (var tile in _tiles)
                tile.SlotObjectMono.SetSprite(isBlurred);
        }
    }
}
