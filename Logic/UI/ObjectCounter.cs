using System;
using Engine.DI;
using Main.Level;
using TMPro;
using UnityEngine;

namespace Custom.Logic.UI
{
    public class ObjectCounter : MonoBehaviour
    {
        [SerializeField] private TMP_Text _conter;
        private PlayerInformation _playerInformation;
        private void Start()
        {
            _playerInformation = DIContainer.GetAsSingle<ILevelsManager>().level.PlayerInformation;
        }

        public void Update()
        {
            _conter.text = Mathf.Clamp(_playerInformation._currentObjects,0,_playerInformation._maxObjects) + "/" + _playerInformation._maxObjects;
        }
    }
}
