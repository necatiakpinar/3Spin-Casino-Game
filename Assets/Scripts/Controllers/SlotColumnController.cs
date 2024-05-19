using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Enums;
using UnityEngine;

namespace DefaultNamespace.Controllers
{
    public class SlotColumnController
    {
        private List<TileMono> _tiles;
        private TileMono _middleSlot;

        private bool _isCheckingForTarget = false;
        private bool _isStopped = false;

        private readonly int _spinSpeed = 100; //todo:(necatiakpinar) Convert into SO and fetch it from addressables
        private readonly int _fastStopSpeed = 100;
        private readonly int _normalStopSpeed = 1000;
        private readonly int _slowDownSpeed = 500;
        private readonly int _middleSlotIndex = 2;

        private SlotObjectType _targetSlotObjectType;

        public List<TileMono> Tiles => _tiles;


        public SlotColumnController(List<TileMono> tiles)
        {
            _tiles = tiles;
            _middleSlot = _tiles[_middleSlotIndex];
        }

        public async Task Spin(SlotObjectType objectType)
        {
            var isExist = Player.GameplayData.ResultDictionary.ContainsKey(Player.GameplayData.CurrentSpinIndex);
            if (isExist)
            {
                _targetSlotObjectType = objectType;
                await CheckForSpinResult();
            }
        }

        private async Task CheckForSpinResult()
        {
            await TryToDoMovement(_targetSlotObjectType, _middleSlot);

            SetSlotObjectsSpriteToNormal(false);
        }

        private async Task TryToDoMovement(SlotObjectType objectType, TileMono middleSlot)
        {
            while (!IsSlotObjectInFirstTile(middleSlot, objectType))
            {
                if (IsSlotObjectInFirstTile(middleSlot, objectType))
                {
                    _isStopped = true;
                    break;
                }

                await DoMovement();
                await CheckForSpinResult();
                return;
            }
        }

        private async Task DoMovement()
        {
            for (int i = _tiles.Count - 1; i >= 0; i--)
            {
                var tile = _tiles[i];
                if (i - 1 >= 0)
                {
                    tile.DropObjectToBottom(_tiles[i - 1], GetCurrentSpeed());
                }
                else
                {
                    tile.DropObjectToBottom(_tiles[_tiles.Count - 1], GetCurrentSpeed());
                }
            }

            await Task.Delay(GetCurrentSpeed());
        }

        private bool IsSlotObjectInFirstTile(TileMono tile, SlotObjectType objectType)
        {
            return tile.Coordinates.y == _middleSlotIndex && tile.SlotObjectMono.Type == objectType;
        }

        private int GetCurrentSpeed()
        {
            if (_isStopped)
            {
                return _slowDownSpeed;
            }

            return _spinSpeed;
        }

        private void SetSlotObjectsSpriteToNormal(bool isBlurred)
        {
            for (var i = 0; i < _tiles.Count; i++)
            {
                var tile = _tiles[i];
                tile.SlotObjectMono.SetSprite(isBlurred);
            }
        }
    }
}