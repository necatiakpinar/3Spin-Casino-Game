using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Enums;
using EventBus;
using EventBus.Events;
using Interfaces;
using UnityEngine;

namespace Abstractions
{
    public abstract class BaseVFX : MonoBehaviour , IPoolable<BaseVFX>
    {
        [SerializeField] protected VFXType _vfxType;
        public abstract UniTask Init();
        public abstract Task Play();

        public virtual void ResetVFX()
        {
            gameObject.SetActive(false);
        }
        
        public void OnSpawn() 
        {
        }
        public virtual void ReturnToPool(BaseVFX poolObject)
        {
            EventBus<ReturnToPoolEvent<VFXType, BaseVFX>>.Raise(new ReturnToPoolEvent<VFXType, BaseVFX>(_vfxType, this));
        }
    }
}