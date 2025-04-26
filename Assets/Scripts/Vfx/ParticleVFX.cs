using System.Threading.Tasks;
using Abstractions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Vfx
{
    public class ParticleVFX : BaseVFX
    {
        private ParticleSystem _particleSystem;
        private readonly float _milliSeconds = 1000;

        public async override UniTask Init()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            await Task.CompletedTask;
        }

        public async override Task Play()
        {
            _particleSystem.Play();
            await Task.Delay((int)(_particleSystem.main.duration * _milliSeconds));
            ReturnToPool(this);
        }
    }
}