using System;
using System.Collections;
using System.Collections.Generic;
using Custom.Logic.Upgrades;
using Engine;
using Engine.DI;
using Main.UI;
using UnityEngine;

namespace Custom.Logic.UI
{
    public class BufsContainer : Panel,IBufsContainer
    {
        [SerializeField] private GameObject _panelPrefab;
        [SerializeField] private List<BuffPanelUI> _buffPanels;
        private ILevelBufsControll _levelBufsControll;
        private List<LevelBuff> _levelBuffs;
        [SerializeField] private bool useUpgrades;
        [SerializeField] private CanvasGroup _grayPanel;

        private void Awake()
        {
            _levelBufsControll = DIContainer.GetAsSingle<ILevelBufsControll>();
            _levelBuffs=_levelBufsControll.LevelsBuffs;
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            _grayPanel.alpha = 0f;
            foreach (var VARIABLE in _buffPanels)
            {
                Destroy(VARIABLE.gameObject);
            }
            _buffPanels.Clear();
        }

        private void Init()
        {
            for (int i = 0; i < _levelBuffs.Count; i++)
            {
                if (_levelBuffs[i].Level < _levelBuffs[i].MaxLevel)
                {
                    BuffPanelUI buffPanel = Instantiate(_panelPrefab, gameObject.transform).GetComponent<BuffPanelUI>();
                    buffPanel.Init(_levelBuffs[i].BuffId);
                    _buffPanels.Add(buffPanel);
                }
                
            }
        }

        public void ShowPanel()
        {
            if(!useUpgrades)
                return;
            Show();
            //Timer.PauseGame();
            StartCoroutine(LongPause(0.02f));
        }

        public void Inject()
        {
            DIContainer.RegisterAsSingle<IBufsContainer>(this);
        }
        public  IEnumerator LongPause(float speed)
        {
           float t = 0;
            while (Time.timeScale>0)
            {
                _grayPanel.alpha=Mathf.Lerp(0, 0.5f, t*50);
                Time.timeScale = Mathf.Lerp(Time.timeScale, 0, t);
                t +=speed* Time.unscaledDeltaTime;
                yield return null;
                
            }
            
        }
    }

   
    public interface IBufsContainer:IDependency
    {
        void ShowPanel();
        void Hide();
    }
}
