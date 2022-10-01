using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimFollow;
using Engine;
using Engine.DI;
using Engine.Senser;
using example1;
using Main;
using Main.Level;
using Template.CharSystem;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Custom.Logic
{
    public class EnemyPhysicsDestination : Fighter,ICoroutineRunner,ILevelCompleted,ILevelFailed
    {
        [SerializeField] private EnemySettings m_Settings;

        [Header("Moving Range")]
        [SerializeField] private Vector2 m_RangeX;
        [SerializeField] private Vector2 m_RangeZ;

        [SerializeField] private Destinator m_Movement;
        [SerializeField] private Damage _Damage;
        [SerializeField] private Transform _movmentPoint;
        private bool _inits;
        protected override IDamage DefineDamage() => _Damage;
        [SerializeField] private Animator _animator;
       // [SerializeField] private PlayerMovement_AF _playerMovement;
        [SerializeField] private Collider[] _colliders;
        [SerializeField] private EnemyHitArea[] _enemyHitAreas;
        //[SerializeField] private RagdollControl_AF _ragdollControl;
       // public bool Falling => _ragdollControl.falling;
        //public bool GettingUp => _ragdollControl.gettingUp;
        private bool _attacked;
        
        [NaughtyAttributes.Button()]
        private void SetCollider()
        {
            _colliders = gameObject.GetComponentsInChildren<Collider>();
            _enemyHitAreas = gameObject.GetComponentsInChildren<EnemyHitArea>();
        }
        public void Init()
        {
            _movmentPoint=DIContainer.GetAsSingle<ILevelsManager>().level.PlayerInformation.CameraPoint;
            WakeUp();
            m_Movement = new DestinatorNavMesh(GetComponent<NavMeshAgent>(), m_Settings.movingSpeed, m_Settings.lookingSpeed);
            m_Movement.Reset(transform);
            m_Movement.SubscribeOnReached(OnReached);
            if(_movmentPoint!=null)
                m_Movement.SetDestination(_movmentPoint.position);
            _inits = true;
            OnReached();
        }

        private void OnEnable()
        {
            LevelStatueFailed.Subscribe(this);
            LevelStatueCompleted.Subscribe(this);
        }

        private void OnDisable()
        {
            LevelStatueFailed.Unsubscribe(this);
            LevelStatueCompleted.Unsubscribe(this);
        }


        protected override IAttacked DefineAttacked() => new Attacked(m_Settings.hitPoint,this);

        protected void FixedUpdate()
        {
            /*if(_movmentPoint==null)
                return;*/
          //  m_Movement.Move(Time.fixedDeltaTime);
          //  _playerMovement.MoveForce = 1;
           // _playerMovement.LookTargert = _movmentPoint;
            m_Movement.Look(Time.fixedDeltaTime);
            m_Movement.Move(Time.fixedDeltaTime);
        }
        private void Update()
        {
            /*if(!_inits)
                return;
            if(_movmentPoint==null)
                return;*/
            m_Movement.SetDestination(_movmentPoint.position);
        }

        private void LateUpdate()
        {
            if(!_inits)
                return;
            if(_movmentPoint==null)
                return;
           // _animator.SetInteger("Run", (m_Movement.isMoving) ? 1 : -1);
            _animator.speed = m_Settings.movingSpeed/4;
        }

        

        private void OnReached()
        {
            m_Movement.SetDestination(new Vector3(Random.Range(m_RangeX.x, m_RangeX.y), 0, Random.Range(m_RangeZ.x, m_RangeZ.y)));
        }
        


        protected override void OnDead(IDamage damage)
        {
            foreach (EnemyHitArea enemyHitArea in _enemyHitAreas)
            {
                enemyHitArea.enabled = false;
            }
            StartCoroutine(DeathWait());
        }

        private IEnumerator DeathWait()
        {
            yield return new WaitForSeconds(1f);
            Destroy(gameObject);
        }

        public void LevelCompleted()
        {
            _inits = false;
            m_Movement.UnsubscribeOnReached(OnReached);
            m_Movement.Reset(transform,false);
            m_Movement.SetDestination(transform.position);
         // _playerMovement.MoveForce = 0;
          _animator.speed = 0;
          _animator.SetBool("Dead",true);
          
        }

        public void LevelFailed()
        {
            _inits = false;
            m_Movement.UnsubscribeOnReached(OnReached);
            m_Movement.Reset(transform,false);
            m_Movement.SetDestination(transform.position);
           // _playerMovement.MoveForce = 0;
            _animator.speed = 0;
            _animator.SetBool("Dead",true);
            
        }
    }
}