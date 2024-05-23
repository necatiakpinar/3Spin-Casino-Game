using UnityEngine;

namespace Abstractions
{
    public abstract class BaseWindow : MonoBehaviour
    {
        public abstract void Init(BaseWindowParameters windowParameters = null);
    }
}