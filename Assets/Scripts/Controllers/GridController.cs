﻿using System.Collections.Generic;
using Adapters;
using Data;
using EventBus;
using EventBus.Events;
using Interfaces;

namespace Controllers
{
    public class GridController
    {
        private List<ITile> _currentTiles;
        private int _currentCreatedSlotObjectIndex = 0;

        private readonly Dictionary<int, List<ITile>> _gridDictionary; // int: Column index, List<TileMono>: Tiles in the column
        private readonly IGridProperties _properties;
        private readonly IObjectFactory _objectFactory;
        private readonly ITransform _transformProvider;
        
        public GridController(IGridProperties gridProperties, IObjectFactory objectFactory, ITransform transformProvider)
        {
            _properties = gridProperties;
            _objectFactory = objectFactory;
            _transformProvider = transformProvider;
            _currentTiles = new List<ITile>();
            _gridDictionary = new Dictionary<int, List<ITile>>();
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for (int x = 0; x < _properties.Width; x++)
            {
                _currentCreatedSlotObjectIndex = 0;
                _currentTiles = new List<ITile>();
                for (int y = 0; y < _properties.Height; y++)
                {
                    var createdTile = CreateCell(x, y);
                    _currentTiles.Add(createdTile);
                }

                _gridDictionary.Add(x, _currentTiles);
            }

            EventBusManager.Raise(new TilesCreatedEvent(_gridDictionary));
        }

        private ITile CreateCell(int x, int y)
        {
            var coordinates = new Vector2Int(x, y);
                
            IVector3 position = new Vector3Adapter(new Vector3(
                x * _properties.CellSize,
                y * _properties.CellSize,
                0f));
            
            var tile = _objectFactory.CreateObject<ITile>(_properties.TilePrefab, _transformProvider, position); 
            var currentSlotObjectPf = _properties.NormalSlotObjectsPf[_currentCreatedSlotObjectIndex];
            var slotObject = _objectFactory.CreateObject<ISlotObject>(currentSlotObjectPf, tile.Transform, position);
            _currentCreatedSlotObjectIndex++;

            tile.Init(coordinates, slotObject);

            return tile;
        }
    }
}