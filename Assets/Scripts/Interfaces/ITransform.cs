using Data;

namespace Interfaces
{
    public interface ITransform
    {
        UnityEngine.Vector3 Position { get; set; } //todo: change to custom?
        UnityEngine.Vector3 LocalPosition { get; set; } //todo: change to custom?
        void SetParent(ITransform parent); //todo: change to custom? or remove
    }
}