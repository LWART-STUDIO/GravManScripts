using System;
using System.Collections;
using System.Linq;
using AnimFollow;
using Engine;
using Engine.DI;
using Engine.Senser;
using example1;
using Main.Level;
using Template.CharSystem;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Custom.Logic
{
    public class EnemyNavMeshDestination : Fighter, ICoroutineRunner,ILevelFailed
    {
        [Header("Moving Range")] [SerializeField]
        private Vector2 _RangeX;

        [SerializeField] private Vector2 _RangeZ;
        [SerializeField] private Attacked _Attacked;
        [SerializeField] private Damage _Damage;
        [SerializeField] private Restoration _Restoration;
        [SerializeField] private EnemySettings m_Settings;
        private bool _inits;
        private Destinator _Destinator;
        [SerializeField] private Transform _movmentPoint;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Animator _animator;
        [SerializeField] HashIDs_AF hash;
        private bool _attacked;
        public float speedDampTime = .1f;
        private bool _playerInAttackRange;
        [SerializeField] private LayerMask _groundLayerMask, _playerLayerMask;
        [SerializeField] private Rigidbody[] _rigidbodies;
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private Material _deathMaterial;

        /*protected void Awake()
        {
            WakeUp();

            _Destinator = new DestinatorNavMesh(GetComponent<NavMeshAgent>(), m_Settings.movingSpeed, m_Settings.lookingSpeed);
            _Destinator.Reset(transform);
            _Destinator.SubscribeOnReached(OnReached);

            OnReached();
        }*/
        [NaughtyAttributes.Button()]
        private void TakeRigbodyes()
        {
            _rigidbodies = GetComponentsInChildren<Rigidbody>();
        }
        public void Init()
        {
            _movmentPoint=DIContainer.GetAsSingle<ILevelsManager>().level.PlayerInformation.CameraPoint;
            WakeUp();
            _Destinator = new DestinatorNavMesh(_agent, m_Settings.movingSpeed, m_Settings.lookingSpeed);
            _Destinator.Reset(transform);
            _Destinator.SubscribeOnReached(OnReached);
            if(_movmentPoint!=null)
                _Destinator.SetDestination(_movmentPoint.position);
            _inits = true;
            OnReached();
        }

        private void Update()
        {
            _playerInAttackRange = Physics.CheckSphere(transform.position, m_Settings.attackRange,_playerLayerMask);
        }

        private void LateUpdate()
        {
            if(!_inits)
                return;
            if(_movmentPoint==null)
                return;

            if (!_playerInAttackRange)
            {
                _agent.SetDestination(_movmentPoint.position);
                _animator.SetFloat(hash.speedFloat, 5.5f, speedDampTime, Time.fixedDeltaTime);
            }

            if (_playerInAttackRange)
            {
                AttackPlayer();
            }
            
            

        }
        private void AttackPlayer()
        {
            _animator.SetFloat(hash.speedFloat, 0, speedDampTime, Time.fixedDeltaTime);
            _agent.SetDestination(transform.position);
            transform.LookAt(_movmentPoint);
            
            if (!_attacked)
            {
                _animator.SetTrigger("Attack");
                Attack(_movmentPoint.GetComponent<Character>());
                _attacked = true;
                DIContainer.Bind<ISenser>().OfType<VibrationInfo>().AsSingleton().PlayVibtation(1,0.2f,0.1f);
                Invoke(nameof(ResetAttack),m_Settings.timeBetewenAttack);
            }
        }
        private void ResetAttack() =>
            _attacked = false;

        protected override IAttacked DefineAttacked() => _Attacked;
        protected override IDamage DefineDamage() => _Damage;
        
        

        private void OnReached()
        {
            if(_movmentPoint!=null)
                _Destinator.SetDestination(_movmentPoint.position);
            else
            {
                _Destinator.SetDestination(new Vector3(
                    Random.Range(_RangeX.x, _RangeX.y),
                    0,
                    Random.Range(_RangeZ.x, _RangeZ.y)
                ));
            }
            
        }

        protected override void OnDead(IDamage damage)
        {
            _meshRenderer.material = _deathMaterial;
            _agent.enabled = false;
            _inits = false;
            _animator.enabled = false;
            gameObject.layer = 7;
           
            foreach (var rigidbody in _rigidbodies)
            {
                rigidbody.gameObject.layer = 7;
                rigidbody.velocity = Vector3.zero;
                // rigidbody.AddForce(Vector3.back*200,ForceMode.Impulse);
            }
            _rigidbodies[0].AddForce(-transform.forward*500,ForceMode.Impulse);
            _rigidbodies[5].AddForce(-transform.forward*500,ForceMode.Impulse);
            StartCoroutine(DestroyWait());
            //Destroy(gameObject);
            
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 3)
            {
                Character attacked = collision.gameObject.GetComponent<Character>();

                if (attacked != null) Attack(attacked);
            }
        }
        

        protected void OnValidate()
        {
            if (_Attacked == null) _Attacked = new Attacked(1,this);
            if (_Damage == null) _Damage = new Damage(this, 1);
        }

        public void LevelFailed()
        {
            _inits = false;
            _animator.SetFloat(hash.speedFloat, 0, speedDampTime, Time.fixedDeltaTime);
        }

        private IEnumerator DestroyWait()
        {
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }
    }
}
