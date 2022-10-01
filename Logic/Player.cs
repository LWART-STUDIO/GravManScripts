using System;
using Custom.Logic.Upgrades;
using Engine;
using Engine.DI;
using example1;
using Main;
using Main.Level;
using Template.CharSystem;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;

namespace Custom.Logic
{
    public class Player : Character,ILevelStarted,ICoroutineRunner,ILevelCompleted
    {
        [Header("Settings")]
        [SerializeField] private PlayerSettings m_Settings;
        [SerializeField] protected Animator m_Animator;
        private float _speed;
        private BuffId _speedBuffId = BuffId.MoveSpeed;
        private LevelBuff _speedLevelBuff;
        private BuffId _healthBuffId = BuffId.Health;
        private LevelBuff _healthLevelBuff;
        private bool CanMoved=false;
        [SerializeField] private Image _healthBar;
        [SerializeField] private float _startHp;
        private int _lastHelthLevel;


        [SerializeField] private DirectionController m_Movement;

        public PlayerSettings settings => m_Settings;


        protected override IAttacked DefineAttacked() => new Attacked(m_Settings.hitPoint,this);

        private void OnEnable()
        {
            LevelStatueStarted.Subscribe(this);
            LevelStatueCompleted.Subscribe(this);
        }

        private void OnDisable()
        {
            LevelStatueStarted.Unsubscribe(this);
            LevelStatueCompleted.Unsubscribe(this);
        }


        protected void Awake()
        {
            WakeUp();
            _speed = m_Settings.movingSpeed;
            _healthLevelBuff = DIContainer.GetAsSingle<ILevelBufsControll>().LevelsBuffs.Find(x => x.BuffId == _healthBuffId);
            _speedLevelBuff = DIContainer.GetAsSingle<ILevelBufsControll>().LevelsBuffs.Find(x => x.BuffId == _speedBuffId);
            m_Movement.Reset(transform);
            
            m_Attacked.maxHit = m_Settings.hitPoint;
            m_Attacked.hitPoint = m_Attacked.maxHit ;
            _startHp = m_Attacked.maxHit;
            _lastHelthLevel = _healthLevelBuff.Level;

        }

        protected void FixedUpdate()
        {
            Debug.Log(_lastHelthLevel);
            if(!CanMoved)
                return;
            if (_lastHelthLevel != _healthLevelBuff.Level)
            {
                _lastHelthLevel = _healthLevelBuff.Level;
                m_Attacked.TakeHealth(new Restoration((_healthLevelBuff.Level * _healthLevelBuff.MultiPlier)));
            }
            _healthBar.fillAmount = m_Attacked.hitPoint / m_Attacked.maxHit ;
            m_Attacked.maxHit =_startHp + ((_healthLevelBuff.Level -1) * _healthLevelBuff.MultiPlier);
            _speed = m_Settings.movingSpeed + (_speedLevelBuff.Level * _speedLevelBuff.MultiPlier)-1;
            m_Movement.SetMovingSpeed(Mathf.Clamp01(joystick.ControllerJoystick.vector.magnitude) * _speed);
            m_Movement.SetDirection(joystick.ControllerJoystick.vector * Time.fixedDeltaTime);

            m_Movement.Move(Time.fixedDeltaTime);
            m_Movement.Look(Time.fixedDeltaTime);
            
            
        }

        protected void LateUpdate()
        {
            if(!isInited)
                return;
            m_Animator.SetInteger("Run", (m_Movement.isMoving) ? 1 : -1);
            m_Animator.SetFloat("Speed",joystick.ControllerJoystick.vector.magnitude);
        }

        protected void OnValidate()
        {
            if (m_Movement == null)
                m_Movement = new DirectionController(GetComponent<CharacterController>(), _speed, m_Settings.lookingSpeed);
        }
        

        protected override void OnDead(IDamage damage)
        {
            Destroy(gameObject);
            DIContainer.GetAsSingle<Engine.IMakeFailed>().MakeFailed(0);

        }


        public void LevelStarted()
        {
            CanMoved = true;
        }

        public void LevelCompleted()
        {
            m_Animator.SetTrigger("Victory");
        }
    }
}