using Addressables;
using Controllers;
using Data.ScriptableObjects.Properties;
using Factories;
using Loggers;
using UnityEngine;
using UnityObjects;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform _gridParent;
        private GridPropertiesDataSo _properties;
        private GridController _gridController;
        
        private readonly UnityLogger _logger = new UnityLogger();
        
        private async void Start()
        {
            _properties = await AddressableLoader.LoadAssetAsync<GridPropertiesDataSo>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SO_GridPropertiesData));
            
            var objectFactory = new UnityObjectFactory(_logger);
            var transformProvider = new UnityTransform(_gridParent);
            _gridController = new GridController(_properties, objectFactory, transformProvider);
        }
    }
}