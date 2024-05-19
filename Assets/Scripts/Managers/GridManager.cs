using System.Collections.Generic;
using DefaultNamespace;
using Enums;
using SlotObjects;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private int _width = 3;
        [SerializeField] private int _height = 3;
        [SerializeField] private float _cellSize = 2.5f;
        [SerializeField] private TileMono _tileMono;
        [SerializeField] private List<NormalSlotObject> _normalSlotObjectsPF;
        
        private List<TileMono> _currentTiles;
        private Dictionary<int, List<TileMono>> _gridDictionary; // int: Column index, List<TileMono>: Tiles in the column
        
        private int _currentCreatedSlotObjectIndex = 0;
        
        private void Start()
        {
            _currentTiles = new List<TileMono>();
            _gridDictionary = new Dictionary<int, List<TileMono>>();
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for (int x = 0; x < _width; x++)
            {
                _currentCreatedSlotObjectIndex = 0;
                _currentTiles = new List<TileMono>();
                for (int y = 0; y < _height; y++)
                {
                    var createdTile = CreateCell(x, y);
                    _currentTiles.Add(createdTile);
                }
                _gridDictionary.Add(x, _currentTiles);
            }
            
            EventManager.Notify(ActionType.OnTilesCreated, _gridDictionary);
            
        }

        private TileMono CreateCell(int x, int y)
        {
            var coordinates = new Vector2Int(x, y);
            Vector3 position = new Vector3(x * _cellSize, y * _cellSize, 0);
            var tile = Instantiate(_tileMono, transform);
            tile.transform.localPosition = position;
            
            var currentSlotObjectPF = _normalSlotObjectsPF[_currentCreatedSlotObjectIndex];
            var slotObject = Instantiate(currentSlotObjectPF, tile.transform);
            _currentCreatedSlotObjectIndex++;
            
            tile.Init(coordinates, slotObject);
            return tile;

        }
    }
}