using System.Threading.Tasks;
using Abstractions;
using Managers;
using UnityEngine;

namespace Vfx
{
    public class ParticleVFX : BaseVFX
    {
        private ParticleSystem _particleSystem;

        private readonly float _milliSeconds = 1000;

        public override void Init()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        public override async Task Play()
        {
            _particleSystem.Play();
            await Task.Delay((int)(_particleSystem.main.duration * _milliSeconds));
            ReturnToPool();
        }

        public override void ReturnToPool()
        {
            VFXPoolManager.Instance.ReturnToPool(_vfxType, this);
        }
    }
}