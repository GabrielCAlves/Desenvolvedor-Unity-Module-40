using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;

public class ItemCollectableCoin : ItemCollectableBase
{

    protected override void OnCollect()
    {
        base.OnCollect();
        //ItemManager.Instance.textMeshProUGUI = textMeshProUGUI;

        Debug.Log("Passou em OnCollect de ItemCollectableCoin");

        //ItemManager.Instance.AddCoins(gameObject);
        //ItemManager.Instance.AddByType(ItemType.COIN);
    }
}
