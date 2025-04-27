using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data.ScriptableObjects.Properties;
using Enums;
using Interfaces;
using UnityEngine;

namespace Controllers
{
    public class SlotColumnController
    {
        private SlotObjectType _targetSlotObjectType;
        private bool _isSlowingDown;
        private int _slowDownSpeed;
        private bool _isSpinning;
        private bool _shouldContinueSpinning;

        private readonly SlotColumnPropertiesDataSo _properties;
        private readonly List<ITile> _tiles;
        private readonly ITile _middleSlot;
        private readonly int _targetProximity = 2;
        private readonly int _slowDownDivider = 2;
        
        public SlotColumnController(List<ITile> tiles, SlotColumnPropertiesDataSo properties)
        {
            _properties = properties;
            _tiles = tiles;
            _middleSlot = _tiles[_properties.MiddleSlotIndex];
        }

        public async UniTask Spin(SlotObjectType objectType, SlotColumnStopType stopType)
        {
            _isSlowingDown = false;
            _shouldContinueSpinning = true;
            _targetSlotObjectType = objectType;
            _slowDownSpeed = _properties.StopSpeeds[stopType];
            await SpinForDuration();
        }

        private async UniTask SpinForDuration()
        {
            SetSlotObjectBlurVisibility(true);
            try
            {
                while (_shouldContinueSpinning && !_isSlowingDown)
                {
                    await DoMovement(_properties.SpinSpeed);
                    await UniTask.Yield();
                }
            }
            finally
            {
                _shouldContinueSpinning = false;
            }
        }

        public async UniTask SlowDown()
        {
            _isSlowingDown = true;
            _shouldContinueSpinning = false;
            await SlowDownToStop(_targetSlotObjectType);
        }

        private async UniTask SlowDownToStop(SlotObjectType objectType)
        {
            await UniTask.WaitUntil(() => !_isSpinning);
            
            bool isObjectInPosition = IsSlotObjectInFirstTile(_middleSlot, objectType);
            while (!isObjectInPosition)
            {
                if (CheckProximityToTarget(objectType, _targetProximity))
                    SetSlotObjectBlurVisibility(false);

                await DoMovement(_slowDownSpeed / _slowDownDivider);
                isObjectInPosition = IsSlotObjectInFirstTile(_middleSlot, objectType);

                await UniTask.Yield();
            }

            SetSlotObjectBlurVisibility(false);
        }

        private bool CheckProximityToTarget(SlotObjectType objectType, int proximity)
        {
            var targetIndex = _tiles.FindIndex(t => t.SlotObject.Type == objectType);
            return Mathf.Abs(targetIndex - _properties.MiddleSlotIndex) <= proximity;
        }

        private async UniTask DoMovement(int speed)
        {
            _isSpinning = true;

            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                var tile = _tiles[i];
                var targetTile = i - 1 >= 0 ? _tiles[i - 1] : _tiles[^1];
                await tile.DropObjectToBottom(targetTile, speed);
            }

            await UniTask.Delay(speed);
            _isSpinning = false;
        }

        private bool IsSlotObjectInFirstTile(ITile tile, SlotObjectType objectType)
        {
            return tile.Coordinates.y == _properties.MiddleSlotIndex && tile.SlotObject.Type == objectType;
        }

        private void SetSlotObjectBlurVisibility(bool isBlurred)
        {
            foreach (var tile in _tiles)
                tile.SlotObject.SetSprite(isBlurred);
        }
    }
}