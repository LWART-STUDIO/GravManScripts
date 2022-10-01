using System;
using Engine;
using Engine.DI;
using Main;
using Main.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Custom.Logic.UI
{
    public class WaveProgressUI : MonoBehaviour,ILevelStarted
    {
        [SerializeField] private Image _levelProgressBar;
        [SerializeField] private Image _waveProgressBar;
        [SerializeField] private TMP_Text _waveText;
        private ILevel _levelInfo;
        
        private void OnEnable()
        {
            LevelStatueStarted.Subscribe(this);
        }

        private void OnDisable()
        {
            LevelStatueStarted.Unsubscribe(this);
        }

        private void Start()
        {
            _levelInfo = DIContainer.GetAsSingle<ILevelsManager>().level;
        }

        public void LevelStarted()
        {
           _levelInfo = DIContainer.GetAsSingle<ILevelsManager>().level;
        }

        private int GetMaxValue()
        {
            int maxValue=0;
            for (int i = 0; i < _levelInfo.WaveInfos.Count; i++)
            {
                maxValue += _levelInfo.WaveInfos[i].NumberOfEnemys;
            }
            return maxValue;
        }

        private int GetCurrentValue()
        {
            int currentValue=0;
            for (int i = 0; i < _levelInfo.WaveInfos.Count; i++)
            {
                currentValue += _levelInfo.WaveInfos[i].NumberOfEnemys -
                                 _levelInfo.WaveInfos[i].NumberOfEnemysToSlayWave;
            }
            return currentValue;
        }

        private void Update()
        {
            if(_levelInfo==null)
                return;
            int maxValue = GetMaxValue();
            int currentValue = GetCurrentValue();
            _waveProgressBar.fillAmount = (float)(_levelInfo.WaveInfos[_levelInfo.CurrentWave].NumberOfEnemys -
                                                  _levelInfo.WaveInfos[_levelInfo.CurrentWave]
                                                      .NumberOfEnemysToSlayWave) /
                                          _levelInfo.WaveInfos[_levelInfo.CurrentWave].NumberOfEnemys;
            _levelProgressBar.fillAmount =(float)currentValue / maxValue;
            _waveText.text = "wave: " + (_levelInfo.CurrentWave+1);

        }
    }
}
