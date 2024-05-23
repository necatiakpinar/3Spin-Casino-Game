using System.Threading.Tasks;
using Enums;
using UnityEngine;

namespace Abstractions
{
    public abstract class BaseVFX : MonoBehaviour
    {
        [SerializeField] protected VFXType _vfxType;
        public abstract void Init();
        public abstract Task Play();

        public virtual void ResetVFX()
        {
            gameObject.SetActive(false);
        }

        public abstract void ReturnToPool();
    }
}