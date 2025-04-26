using System.Collections.Generic;
using Abstractions;
using Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Managers
{

    public class VFXPoolManager : MonoBehaviour
    {
        [SerializeField] private List<VFXPoolObject> _pools;
        [SerializeField] private Transform _vfxParent;

        private Dictionary<VFXType, Queue<BaseVFX>> _poolDictionary;

        public static VFXPoolManager Instance;


        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _poolDictionary = new Dictionary<VFXType, Queue<BaseVFX>>();

            foreach (var pool in _pools)
            {
                Queue<BaseVFX> objectPool = new Queue<BaseVFX>();

                for (int i = 0; i < pool.Size; i++)
                {
                    BaseVFX vfx = Instantiate(pool.VfxPf, _vfxParent);
                    vfx.gameObject.SetActive(false);
                    vfx.Init();
                    objectPool.Enqueue(vfx);
                }

                _poolDictionary.Add(pool.VFXType, objectPool);
            }
        }

        public T SpawnFromPool<T>(VFXType vfxType, Vector3 position, Quaternion rotation, bool canSetPositionAndRotation = true) where T : BaseVFX
        {
            if (!_poolDictionary.ContainsKey(vfxType) || _poolDictionary[vfxType].Count == 0)
                return null;

            BaseVFX vfxToSpawn = _poolDictionary[vfxType].Dequeue();
            vfxToSpawn.gameObject.SetActive(true);
            if (canSetPositionAndRotation)
            {
                vfxToSpawn.transform.localPosition = position;
                vfxToSpawn.transform.rotation = rotation;
            }
            
            return (T)vfxToSpawn;
        }

        public void ReturnToPool(VFXType vfxType, BaseVFX vfxToReturn)
        {
            if (!_poolDictionary.ContainsKey(vfxType))
                return;

            vfxToReturn.ResetVFX();
            vfxToReturn.transform.parent = _vfxParent;
            _poolDictionary[vfxType].Enqueue(vfxToReturn);
        }
    }
}