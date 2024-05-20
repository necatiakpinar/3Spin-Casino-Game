using System;
using Addressables;
using DefaultNamespace;
using DG.Tweening;
using Enums;
using UnityEngine;
using UnityEngine.U2D;

namespace Abstractions
{
    public abstract class BaseSlotObjectMono : MonoBehaviour
    {
        [SerializeField] private SlotObjectType _type;
        
        private SpriteRenderer _spriteRenderer;
        private SpriteAtlas _normalSpriteAtlas;
        private SpriteAtlas _blurredSpriteAtlas;
        private Sprite _normalSprite;
        private Sprite _blurredSprite;

        public SlotObjectType Type => _type;
        
        private readonly float _milliSeconds = 1000; // todo:(necatiakpinar)Convert into SO and fetch it from addressables
        private readonly float _blurredSpriteSpeedThreshold = 0.2f;
        private readonly string _blurredSuffix = "_Blur";
        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Init();
        }

        /// <summary>
        /// Initialize slot object
        /// </summary>
        protected virtual async void Init()
        {
            _normalSpriteAtlas = await AddressableLoader.LoadAssetAsync<SpriteAtlas>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SA_NormalSlotObjects));
            _blurredSpriteAtlas = await AddressableLoader.LoadAssetAsync<SpriteAtlas>(AddressableKeys.GetKey(AddressableKeys.AssetKeys.SA_BlurredSlotObjects));
            
            _normalSprite = _normalSpriteAtlas.GetSprite(_type.ToString());
            _blurredSprite = _blurredSpriteAtlas.GetSprite(_type + _blurredSuffix);

            SetSprite(false);
        }
        
        /// <summary>
        /// Set sprite for slot object
        /// </summary>
        /// <param name="isBlurred"></param>
        public void SetSprite(bool isBlurred)
        {
            _spriteRenderer.sprite = isBlurred ? _blurredSprite : _normalSprite;
        }

        public void MoveToTile(TileMono targetTile, int speed)
        {
            if (targetTile.Coordinates.y == 4) // todo(necatiakpinar): change this magic number!
                _spriteRenderer.enabled = false;
            else
                _spriteRenderer.enabled = true;
            
            var duration = speed / _milliSeconds;
            
            transform.SetParent(targetTile.transform);
            transform.DOLocalMove(Vector3.zero, duration).SetEase(Ease.Linear).OnComplete(() =>
            {
                targetTile.SetSlotObject(this);
            });
        }
    }
}