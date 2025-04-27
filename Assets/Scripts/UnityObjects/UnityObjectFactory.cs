using Abstractions;
using Data;
using Interfaces;

namespace UnityObjects
{
    public class UnityObjectFactory : IObjectFactory
    {
        private readonly ILogger _logger;

        public UnityObjectFactory(ILogger logger)
        {
            _logger = logger;
        }

        public ITile CreateTile(object tilePrefab, ITransform parent, Vector3 localPosition)
        {
            var prefab = tilePrefab as TileMono;
            if (prefab == null)
            {
                _logger.LogError("Prefab is not a TileMono");
                return null;
            }

            var unityParent = (parent as UnityTransform)?.TransformRef;
            var instance = UnityEngine.Object.Instantiate(prefab, unityParent);
            instance.transform.localPosition = new UnityEngine.Vector3(localPosition.x, localPosition.y, localPosition.z);

            return new UnityTileAdapter(instance);
        }

        public ISlotObject CreateSlotObject(object slotObjectPrefab, ITransform parent, Vector3 localPosition)
        {
            var prefab = slotObjectPrefab as SlotObjectMono;
            if (prefab == null)
            {
                _logger.LogError("Prefab is not a GameObject");
                return null;
            }

            var unityParent = (parent as UnityTransform)?.TransformRef;
            var instance = UnityEngine.Object.Instantiate(prefab, unityParent);
            instance.transform.localPosition = new UnityEngine.Vector3(localPosition.x, localPosition.y, localPosition.z);

            return new UnitySlotObjectAdapter(instance, _logger);
        }
    }
}