using System;
using System.Collections.Generic;
using Helpers;
using Interfaces;
using Miscs;
using UnityEngine;

namespace Abstractions
{
    public class BasePool<T, TK> : MonoBehaviour where T : Enum where TK : MonoBehaviour, IPoolable<TK>
    {
        [SerializeField] private List<PoolObject<T, TK>> _poolObjects;

        private Dictionary<T, Queue<TK>> _poolDictionary;
        private Dictionary<T, TK> _prefabDictionary;

        private void OnEnable()
        {
            _poolDictionary = new Dictionary<T, Queue<TK>>();
            _prefabDictionary = new Dictionary<T, TK>();

            foreach (var poolObject in _poolObjects)
            {
                _poolDictionary[poolObject.ObjectType] = new Queue<TK>();
                _prefabDictionary[poolObject.ObjectType] = poolObject.ObjectPf;
            }

            Init();
        }

        private void Init()
        {
            for (int i = 0; i < _poolObjects.Count; i++)
            {
                var poolObject = _poolObjects[i];
                _poolDictionary[poolObject.ObjectType] = new Queue<TK>();
                var prefab = poolObject.ObjectPf;
                for (int j = 0; j < poolObject.Size; j++)
                {
                    var createdPoolObject = Instantiate(prefab, transform);
                    _poolDictionary[poolObject.ObjectType].Enqueue(createdPoolObject);
                    createdPoolObject.gameObject.SetActive(false);
                }
            }
        }

        public TK SpawnFromPool(
            T objectType,
            Vector3 position,
            Quaternion rotation = default,
            Transform parent = null,
            bool updatePositionAndRotation = true)
        {
            if (!_poolDictionary.TryGetValue(objectType, out var objectQueue))
            {
                LoggerUtil.LogError($"No pool with ID {objectType} found!");
                return null;
            }

            TK objectToSpawn;

            if (objectQueue.Count == 0)
            {
                if (!_prefabDictionary.TryGetValue(objectType, out var prefab))
                {
                    LoggerUtil.LogError($"No prefab found for object type {objectType}!");
                    return null;
                }

                objectToSpawn = Instantiate(prefab);
                objectToSpawn.gameObject.SetActive(false);
            }
            else
            {
                objectToSpawn = objectQueue.Dequeue();
            }

            if (parent != null)
                objectToSpawn.transform.SetParent(parent);

            if (updatePositionAndRotation)
            {
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;
            }

            objectToSpawn.gameObject.SetActive(true);
            objectToSpawn.OnSpawn();

            return objectToSpawn;
        }

        public void ReturnToPool(T poolObjectType, TK poolObject)
        {
            if (!_poolDictionary.ContainsKey(poolObjectType))
            {
                LoggerUtil.LogError($"No pool with ID {poolObjectType} found!");
                return;
            }

            poolObject.gameObject.SetActive(false);
            _poolDictionary[poolObjectType].Enqueue(poolObject);
        }
    }
}