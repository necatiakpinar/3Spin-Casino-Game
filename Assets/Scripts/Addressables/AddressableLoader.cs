﻿using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Addressables
{
    public static class AddressableLoader
    {
        public static async Task<T> LoadAssetAsync<T>(string key)
        {
            var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var loadedObject = handle.Result;
                return loadedObject;
            }

            return default(T);
        }

        public static async Task<T> LoadComponentAsync<T>(string key) where T : Component
        {
            GameObject prefab = await LoadAssetAsync<GameObject>(key);
            if (prefab != null)
            {
                T component = prefab.GetComponent<T>();
                if (component != null)
                {
                    return component;
                }
                else
                {
                    Debug.LogError($"Component of type {typeof(T).Name} not found on prefab loaded from key: {key}.");
                }
            }
            else
            {
                Debug.LogError($"Prefab not loaded from key: {key}.");
            }

            return null;
        }
    }
}