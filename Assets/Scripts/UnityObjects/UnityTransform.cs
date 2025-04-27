using Interfaces;
using UnityEngine;

namespace UnityObjects
{
    public class UnityTransform : ITransform
    {
        private readonly Transform _transform;
        
        public Transform TransformRef => _transform;

        public UnityTransform(Transform transform)
        {
            _transform = transform;
        }

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public Vector3 LocalPosition
        {
            get => _transform.localPosition;
            set => _transform.localPosition = value;
        }

        public void SetParent(ITransform parent)
        {
            var unityTransform = parent as UnityTransform;
            if (unityTransform != null)
            {
                _transform.SetParent(unityTransform._transform);
            }
        }
    }
}