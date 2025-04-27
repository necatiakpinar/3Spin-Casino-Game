using System.Collections.Generic;
using Abstractions;
using Interfaces;
using UnityEngine;

namespace Data.ScriptableObjects.Properties
{
    [CreateAssetMenu(fileName = "SO_GridProperties", menuName = "Data/ScriptableObjects/Properties/GridProperties", order = 0)]
    public class GridPropertiesDataSo : ScriptableObject, IGridProperties
    {
        [SerializeField] private int _width = 3;
        [SerializeField] private int _height = 3;
        [SerializeField] private float _cellSize = 2.5f;
        [SerializeField] private TileMono _tile;
        [SerializeField] private List<SlotObjectMono> _normalSlotObjectsPf;

        public int Width => _width;
        public int Height => _height;
        public float CellSize => _cellSize;
        public object TilePrefab => _tile;
        public List<ISlotObject> NormalSlotObjectsPf => new(_normalSlotObjectsPf);

    }
}