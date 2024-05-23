﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Abstractions
{
    [RequireComponent(typeof(Button))]
    public abstract class BaseButton : MonoBehaviour
    {
        private Button _button;

        private List<UnityAction> _buttonActions = new List<UnityAction>();

        private void OnDisable()
        {
            _button.onClick.RemoveAllListeners();
            _buttonActions.Clear();
        }

        private void Awake()
        {
            TryGetComponent(out _button);
        }

        public void AddAction(UnityAction buttonAction)
        {
            _buttonActions.Add(buttonAction);
            _button.onClick.AddListener(buttonAction);
        }

        public void RemoveAction(UnityAction actionToRemove)
        {
            if (_buttonActions.Contains(actionToRemove))
                _buttonActions.Remove(actionToRemove);
            else
                Debug.LogError("Action could not be found!");
        }
    }
}