using System.Collections.Generic;
using Addressables;
using Data.ScriptableObjects.Properties;
using EventBus;
using EventBus.Events;
using UnityEngine;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        private GridPropertiesDataSo _properties;
        private List<TileMono> _currentTiles;
        private Dictionary<int, List<TileMono>> _gridDictionary; // int: Column index, List<TileMono>: Tiles in the column
        private int _currentCreatedSlotObjectIndex = 0;
        
        private async void Start()
        {
            _properties = await AddressableLoader.LoadAssetAsync<GridPropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_GridPropertiesData));
            _currentTiles = new List<TileMono>();
            _gridDictionary = new Dictionary<int, List<TileMono>>();
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for (int x = 0; x < _properties.Width; x++)
            {
                _currentCreatedSlotObjectIndex = 0;
                _currentTiles = new List<TileMono>();
                for (int y = 0; y < _properties.Height; y++)
                {
                    var createdTile = CreateCell(x, y);
                    _currentTiles.Add(createdTile);
                }
                _gridDictionary.Add(x, _currentTiles);
            }
            
            EventBus<TilesCreatedEvent>.Raise(new TilesCreatedEvent(_gridDictionary));
        }

        private TileMono CreateCell(int x, int y)
        {
            var coordinates = new Vector2Int(x, y);
            var position = new Vector3(x * _properties.CellSize, y * _properties.CellSize, 0);
            var tile = Instantiate(_properties.Tile, transform);
            tile.transform.localPosition = position;
            
            var currentSlotObjectPf = _properties.NormalSlotObjectsPf[_currentCreatedSlotObjectIndex];
            var slotObject = Instantiate(currentSlotObjectPf, tile.transform);
            _currentCreatedSlotObjectIndex++;
            
            tile.Init(coordinates, slotObject);
            return tile;
        }
    }
}
