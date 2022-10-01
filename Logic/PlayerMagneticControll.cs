using System;
using System.Collections.Generic;
using Custom.Logic.Upgrades;
using Engine;
using Engine.DI;
using Engine.Input;
using Main;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Custom.Logic
{
    public class PlayerMagneticControll : MonoBehaviour,IDrag,IEndDrag,IBeginDrag,ILevelStarted
    {
        [SerializeField] private float _sphereMainRadius;
        [SerializeField] private float _sphereStartRadius;
        [SerializeField] private float _defaultSphereMainRadius;
        [SerializeField] private float _defaultSphereStartRadius;
        [SerializeField] private Transform _centerPoint;
        [SerializeField] private GameObject _gravityHolder;
        private List<MagneticObject> _magneticObjects=new List<MagneticObject>();
        public List<MagneticObject> MagneticObjects => _magneticObjects;
        private BuffId _buffId = BuffId.GrabRadius;
        private LevelBuff _levelBuff;
        private BuffId _amountBuffId = BuffId.AmountOfObjects;
        private LevelBuff _amountLevelBuff;
        [SerializeField] private Animator _animator;
        private bool _blokGravity = true;
        private bool _isDraggingState = false;
        [SerializeField] private int _maxCount=30;
        [SerializeField] private int _defaultMaxCont = 30;
        [SerializeField] private ParticleSystem _getParticleSystem;
        public int MaxCount => _maxCount;

        

        private void OnEnable()
        {
            InputEvents.Drag.Subscribe(this);
            InputEvents.EndDrag.Subscribe(this);
            InputEvents.BeginDrag.Subscribe(this);
            //_levelBuff = DIContainer.GetAsSingle<ILevelBufsControll>().LevelsBuffs.Find(x => x.BuffId == _buffId);
            _amountLevelBuff = DIContainer.GetAsSingle<ILevelBufsControll>().LevelsBuffs.Find(x => x.BuffId == _amountBuffId);
            LevelStatueStarted.Subscribe(this);

        }
        private void OnDisable()
        {
            InputEvents.Drag.Unsubscribe(this);
            InputEvents.EndDrag.Unsubscribe(this);
            InputEvents.BeginDrag.Unsubscribe(this);
            LevelStatueStarted.Unsubscribe(this);
        }

        private void FixedUpdate()
        { 
           // _gravityHolder.transform.localPosition = transform.position;
            foreach (MagneticObject magneticObject in _magneticObjects)
            {
                if (!magneticObject.InList)
                {
                    _magneticObjects.Remove(magneticObject);
                    magneticObject.Stop();
                    break;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0f, 1f, 0f, 0.1f);
            Gizmos.DrawSphere(_centerPoint.position, _sphereMainRadius);
            Gizmos.color = new Color(1f, 0.92f, 0.02f, 0.14f);
            Gizmos.DrawSphere(_centerPoint.position,_sphereStartRadius);
        }

        public void OnDrag(InputInfo data)
        {
            if(Time.timeScale!=1)
                return;
            if(_blokGravity)
                return;

            _sphereMainRadius = _defaultSphereMainRadius;//+(_levelBuff.Level*_levelBuff.MultiPlier)-1;
            _sphereStartRadius = _defaultSphereStartRadius;// +(_levelBuff.Level*_levelBuff.MultiPlier)-1;
            _maxCount = _defaultMaxCont +(int)(_amountLevelBuff.Level * _amountLevelBuff.MultiPlier) -(int)_amountLevelBuff.MultiPlier;
            Collider[] hitColiders = Physics.OverlapSphere(_centerPoint.position, _sphereStartRadius);
            foreach (var colider in hitColiders)
            {
                if (colider.TryGetComponent(out MagneticObject magneticObject))
                {
                    if (!magneticObject.InList&&magneticObject.CanBeMoved&&magneticObject.CanBeDraged&& _magneticObjects.Count <= _maxCount)
                    {
                        magneticObject.InList = true; 
                        magneticObject.Rotating(_gravityHolder.transform,_sphereMainRadius);
                        _magneticObjects.Add(magneticObject);
                        
                    }
                }

            }
          
        }
        

        public void OnEndDrag(InputInfo data)
        {
            if(_blokGravity)
                return;
            if(Time.timeScale!=1)
                return;
            _animator.SetTrigger("Push");
            foreach (MagneticObject magneticObject in _magneticObjects)
            {
                if (!magneticObject.InList)
                {
                    magneticObject.Stop();
                }
                else
                {
                    magneticObject.PushInConus(_centerPoint);
                }
                
            }
            _magneticObjects.Clear();
            _isDraggingState = false;
        }

        public void OnBeginDrag(InputInfo data)
        {

            if(_blokGravity)
                return;
            _getParticleSystem.Play();
            _animator.SetTrigger("HandsUp");
            _isDraggingState = true;
        }

        public void LevelStarted()
        {
            _blokGravity = false;
        }
    }
}