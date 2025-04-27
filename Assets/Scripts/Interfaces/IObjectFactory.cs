using Data;

namespace Interfaces
{
    public interface IObjectFactory
    {
        T CreateObject<T>(T prefab, ITransform parent, Vector3 localPosition) where T : class;
    }
}