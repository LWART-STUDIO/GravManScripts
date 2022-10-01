using System;
using System.Collections;
using Custom.Logic.Upgrades;
using Engine;
using Engine.DI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Custom.Logic.UI
{
    public class BuffPanelUI : MonoBehaviour
    {
        private BuffId _buffId;
        private ILevelBufsControll _levelBufsControll;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _icon;
        private BufsContainer _bufsContainer;

        public void Init(BuffId buffId)
        {
            _buffId = buffId;
            _levelBufsControll = DIContainer.GetAsSingle<ILevelBufsControll>();
            _name.text = _levelBufsControll.GetBuffName(buffId);
            _levelText.text = "LEVEL " + _levelBufsControll.GetBuffLevel(buffId);
            _bufsContainer = GetComponentInParent<BufsContainer>();
            _icon.sprite = _levelBufsControll.GetBuffIcon(buffId);
        }

        public void Buy()
        {
            _levelBufsControll.BuyBuff(_buffId);
            Timer.ContinueGame();
            _bufsContainer.Hide();
        }

    }
}
