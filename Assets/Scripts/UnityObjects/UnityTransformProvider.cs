using Interfaces;

namespace UnityObjects
{
    public class UnityTransformProvider : ITransformProvider
    {
        private readonly UnityEngine.Transform _transform;

        public UnityTransformProvider(UnityEngine.Transform transform)
        {
            _transform = transform;
        }

        public ITransform GetTransform()
        {
            return new UnityTransform(_transform);
        }
    }
}