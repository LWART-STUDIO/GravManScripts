using System;
using System.Collections.Generic;
using Engine;
using Engine.Coin;
using Engine.DI;
using Main;
using UnityEngine;

namespace Custom.Logic.Upgrades
{
    public class LevelBufsControll : MonoBehaviour,ILevelBufsControll
    {
        [SerializeField] private List<LevelBuff> _levelsBuffs;
        public List<LevelBuff> LevelsBuffs => _levelsBuffs;
        private void OnEnable()
        {
            LevelStatueStarted.Subscribe(this);
        }

        private void OnDisable()
        {
            LevelStatueStarted.Unsubscribe(this);
        }

        public void InitBuff(BuffId buffId)
        {
            LevelBuff buff = _levelsBuffs.Find(x => x.BuffId == buffId);
            buff.Level = 1;

        }

        public int GetBuffLevel(BuffId buffId)
        {
            LevelBuff buff = _levelsBuffs.Find(x => x.BuffId == buffId);
            return buff.Level;
        }

        public Sprite GetBuffIcon(BuffId buffId)
        {
            LevelBuff buff = _levelsBuffs.Find(x => x.BuffId == buffId);
            return buff._buffSprite;
        }

        public string GetBuffName(BuffId buffId)
        {
            switch (buffId)
            {
                case (BuffId.Health):
                    return "HEALTH";
                case (BuffId.GrabRadius):
                    return "RADIUS";
                case (BuffId.MoveSpeed):
                    return "MOVING";
                case (BuffId.ObjectDamage):
                    return "DAMAGE";
                case (BuffId.AmountOfObjects):
                    return "MAX COUNT";
                default:
                    return "";
            }
        }

        public void BuyBuff(BuffId buffId)
        {
            LevelBuff buff = _levelsBuffs.Find(x => x.BuffId == buffId);
            if (buff.Level >= buff.MaxLevel)
                return;
            buff.Level++;
            Debug.Log("Buy");

        }

        public void LevelStarted()
        {
            foreach (LevelBuff buff in _levelsBuffs)
            {
                InitBuff(buff.BuffId);
            }
        }

        private void Awake()
        {
            foreach (LevelBuff buff in _levelsBuffs)
            {
                InitBuff(buff.BuffId);
            }
        }

        public void Inject()
        {
            DIContainer.RegisterAsSingle<ILevelBufsControll>(this);
        }
    }

    public interface ILevelBufsControll:IDependency,ILevelStarted
    {
         List<LevelBuff> LevelsBuffs { get; }
        void BuyBuff(BuffId buffId);
        void InitBuff(BuffId buffId);
        string GetBuffName(BuffId buffId);
        int GetBuffLevel(BuffId buffId);

        Sprite GetBuffIcon(BuffId buffId);
    }

    public enum BuffId
    {
        ObjectDamage=0,
        GrabRadius=1,
        MoveSpeed=3,
        Health=4,
        AmountOfObjects=5,
    }
}
