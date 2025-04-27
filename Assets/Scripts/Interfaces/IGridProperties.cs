using System.Collections.Generic;
using Abstractions;

namespace Interfaces
{
    public interface IGridProperties
    {
        int Width { get; }
        int Height { get; }
        float CellSize { get; }
        ITile TilePrefab { get; }
        List<ISlotObject> NormalSlotObjectsPf { get; }
    }
}