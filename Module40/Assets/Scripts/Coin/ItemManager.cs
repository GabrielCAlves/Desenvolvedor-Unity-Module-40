using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Singleton;
using TMPro;

namespace Items
{
    public enum ItemType
    {
        COIN,
        LIFE_PACK
    }

    public class ItemManager : Singleton<ItemManager>
    {
        public List<ItemSetup> itemSetups;

        //public SOInt coins;
        //public TextMeshProUGUI textMeshProUGUI;

        //private string goldenCoin = "Gold Coin";
        //private string silverCoin = "Silver Coin";
        //private string bronzeCoin = "Bronze Coin";

        void Start()
        {
            Reset();
            LoadItemsFromSave();
        }

        private void LoadItemsFromSave()
        {
            AddByType(ItemType.COIN, (int)SaveManager.Instance.Setup.coins);
            AddByType(ItemType.LIFE_PACK, (int)SaveManager.Instance.Setup.health);
            Player.Instance.puCloth = SaveManager.Instance.Setup.puCloth;
            Player.Instance.healthBase.currentLife = SaveManager.Instance.Setup.healthBar;
            
            //Player.Instance.healthBase.UpdateUI();
        }

        private void Reset()
        {
            foreach(var i in itemSetups)
            {
                i.soInt.value = 0;
            }

            //coins.value = 0;
            //textMeshProUGUI.text = "0" + coins.value.ToString();
        }

        public void AddByType(ItemType itemType /*GameObject gameObject*/, int amount = 1)
        {
            if(amount < 0)
            {
                return;
            }

            itemSetups.Find(i => i.itemType == itemType).soInt.value += amount;

            //if (gameObject.CompareTag(goldenCoin))
            //{
            //    coins.value += 10;
            //}
            //else if (gameObject.CompareTag(silverCoin))
            //{
            //    coins.value += 5;
            //}
            //else
            //{
            //    coins.value += amount;
            //}
            //UpdateUI();
        }

        public ItemSetup GetItemByType(ItemType itemType)
        {
            return itemSetups.Find(i => i.itemType == itemType);
        }

        public void RemoveByType(ItemType itemType /*GameObject gameObject*/, int amount = -1)
        {
            if(amount > 0)
            {
                return;
            }

            var item = itemSetups.Find(i => i.itemType == itemType);
            item.soInt.value += amount;
            
            if(item.soInt.value < 0)
            {
                item.soInt.value = 0;
            }
        }

        [NaughtyAttributes.Button]
        private void AddCoin()
        {
            AddByType(ItemType.COIN);
        }

        [NaughtyAttributes.Button]
        private void AddLifePack()
        {
            AddByType(ItemType.LIFE_PACK);
        }

        //private void UpdateUI()
        //{
        //    Debug.Log("coins.value = " + coins.value);

        //    if (coins.value < 10)
        //    {
        //        textMeshProUGUI.text = "0" + coins.value.ToString();
        //        //UiInGameManager.Instance.UpdateTextCoins("0"+coins.ToString());
        //        //UiInGameManager.UpdateTextCoins("0"+coins.ToString());
        //    }
        //    else
        //    {
        //        textMeshProUGUI.text = coins.value.ToString();
        //        //UiInGameManager.Instance.UpdateTextCoins(coins.ToString());
        //        //UiInGameManager.UpdateTextCoins(coins.ToString());
        //    }
        //}
    }

    [System.Serializable]
    public class ItemSetup
    {
        public ItemType itemType;
        public SOInt soInt;
        public Sprite icon;
        public string inputButton;
    }
}