using Abstractions;
using Enums;
using UnityEngine;

namespace Managers
{
    [System.Serializable]
    public class VFXPoolObject
    {
        [SerializeField] private VFXType _vfxType;
        [SerializeField] private BaseVFX _vfxPf;
        [SerializeField] private int _size;

        public VFXType VFXType => _vfxType;
        public BaseVFX VfxPf => _vfxPf;
        public int Size => _size;
    }
}