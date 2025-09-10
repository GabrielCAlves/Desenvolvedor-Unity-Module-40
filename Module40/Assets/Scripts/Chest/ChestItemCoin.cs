using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Items;

public class ChestItemCoin : ChestItemBase
{
    public int coinNumber = 5;
    public GameObject coinObject;

    public Vector2 randomRange = new Vector2(-.5f, .5f);

    public float tweenEndTime = .5f;

    private List<GameObject> _items = new List<GameObject>();

    public override void ShowItem()
    {
        base.ShowItem();
        CreateItems();
    }

    [NaughtyAttributes.Button]
    private void CreateItems()
    {
        for(int i = 0; i < coinNumber; ++i)
        {
            var item = Instantiate(coinObject);
            item.transform.position = transform.position + Vector3.forward * Random.Range(randomRange.x, randomRange.y) + Vector3.right * Random.Range(randomRange.x, randomRange.y);
            item.transform.DOScale(0, tweenEndTime).SetEase(Ease.OutBack).From();
            _items.Add(item);
        }
    }

    [NaughtyAttributes.Button]
    public override void Collect()
    {
        base.Collect();

        foreach(var i in _items)
        {
            i.transform.DOMoveY(2f, tweenEndTime).SetRelative();
            i.transform.DOScale(0, tweenEndTime/2).SetDelay(tweenEndTime/2);

            ItemManager.Instance.AddByType(ItemType.COIN);
        }
    }
}
