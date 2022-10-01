using System;
using UnityEngine;

namespace Custom.Logic
{
    public class PlayerInformation:MonoBehaviour
    {
        [SerializeField] private Transform _cameraPoint;
        [SerializeField] private PlayerMagneticControll _playerMagneticControll;
        public float _currentObjects;
        public float _maxObjects;
        public Transform CameraPoint => _cameraPoint;

        private void Update()
        {
            _currentObjects = _playerMagneticControll.MagneticObjects.Count;
            _maxObjects = _playerMagneticControll.MaxCount;
        }
    }
}