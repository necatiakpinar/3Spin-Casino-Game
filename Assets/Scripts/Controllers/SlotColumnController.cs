using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.ScriptableObjects;
using Enums;
using UnityEngine;

namespace Controllers
{
    public class SlotColumnController
    {
        private List<TileMono> _tiles;
        private TileMono _middleSlot;
        private SlotObjectType _targetSlotObjectType;
        private SlotColumnPropertiesDataSo _properties;
        private bool _isSlowingDown;
        private int _slowDownSpeed;

        public SlotColumnController(List<TileMono> tiles, SlotColumnPropertiesDataSo properties)
        {
            _properties = properties;
            _tiles = tiles;
            _middleSlot = _tiles[_properties.MiddleSlotIndex];
        }

        public async Task Spin(SlotObjectType objectType, SlotColumnStopType stopType)
        {
            _isSlowingDown = false;
            _targetSlotObjectType = objectType;
            _slowDownSpeed = _properties.StopSpeeds[stopType];
            await SpinForDuration();
        }

        private async Task SpinForDuration()
        {
            SetSlotObjectBlurVisibility(true);
            while (!_isSlowingDown)
            {
                await DoMovement(_properties.SpinSpeed);
            }
        }

        public async Task SlowDown()
        {
            _isSlowingDown = true;
            Debug.LogError(_targetSlotObjectType);
            await SlowDownToStop(_targetSlotObjectType);
        }

        private async Task SlowDownToStop(SlotObjectType objectType)
        {
            bool isObjectInPosition = IsSlotObjectInFirstTile(_middleSlot, objectType);

            while (!isObjectInPosition)
            {
                if (CheckProximityToTarget(objectType, 2))
                {
                    SetSlotObjectBlurVisibility(false); // Turn off blur as we start final approach
                    _slowDownSpeed = Math.Max(_slowDownSpeed / 2, 1); // Slow down more as you get closer
                }

                await DoMovement(_slowDownSpeed);
                isObjectInPosition = IsSlotObjectInFirstTile(_middleSlot, objectType);
            }

            SetSlotObjectBlurVisibility(false); // Ensure visibility is off when stopped
        }

        private bool CheckProximityToTarget(SlotObjectType objectType, int proximity)
        {
            var targetIndex = _tiles.FindIndex(t => t.SlotObjectMono.Type == objectType);
            return Mathf.Abs(targetIndex - _properties.MiddleSlotIndex) <= proximity;
        }

        private async Task DoMovement(int speed)
        {
            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                var tile = _tiles[i];
                var targetTile = i - 1 >= 0 ? _tiles[i - 1] : _tiles[^1];
                await tile.DropObjectToBottom(targetTile, speed);
            }

            await Task.Delay(speed);
        }

        private bool IsSlotObjectInFirstTile(TileMono tile, SlotObjectType objectType)
        {
            return tile.Coordinates.y == _properties.MiddleSlotIndex && tile.SlotObjectMono.Type == objectType;
        }

        public void SetSlotObjectBlurVisibility(bool isBlurred)
        {
            foreach (var tile in _tiles)
                tile.SlotObjectMono.SetSprite(isBlurred);
        }
    }
}