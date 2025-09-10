using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Items
{
    public class ItemLayout : MonoBehaviour
    {
        private ItemSetup _currSetup;

        public Image uiIcon;
        public TextMeshProUGUI uiValue;
        public TextMeshProUGUI uiPressAction;

        public void Load(ItemSetup setup)
        {
            _currSetup = setup;
            UpdateUI();
        }

        private void UpdateUI()
        {
            uiIcon.sprite = _currSetup.icon;
            uiPressAction.text = _currSetup.inputButton;
        }

        private void Update()
        {
            uiValue.text = _currSetup.soInt.value.ToString();
        }
    }
}