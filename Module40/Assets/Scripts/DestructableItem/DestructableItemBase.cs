using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DestructableItemBase : MonoBehaviour, IDamageable
{
    public HealthBase healthBase;

    public float shakeDuration = .2f;
    public int shakeForce = 1;

    public int dropCoinsAmount = 5;
    public GameObject coinPrefab;
    public Transform dropPosition;

    private void OnValidate()
    {
        if(healthBase == null)
        {
            healthBase = GetComponent<HealthBase>();
        }
    }

    private void Awake()
    {
        OnValidate();
        healthBase.OnDamage += OnDamage;
    }

    private void OnDamage(HealthBase h)
    {
        Debug.Log("OnDamage");
        transform.DOShakeScale(shakeDuration, Vector3.up/2, shakeForce);
        DropGroupCoins();
    }

    [NaughtyAttributes.Button]
    private void DropCoins()
    {
        var i = Instantiate(coinPrefab);
        i.transform.position = dropPosition.position;
        i.transform.DOScale(0, 1f).SetEase(Ease.OutBack).From();
    }

    [NaughtyAttributes.Button]
    private void DropGroupCoins()
    {
        StartCoroutine(DropGroupOfCoinsCoroutine());
    }

    IEnumerator DropGroupOfCoinsCoroutine()
    {
        for (int i = 0; i < dropCoinsAmount; ++i)
        {
            DropCoins();
            yield return new WaitForSeconds(.5f);
        }
    }

    public void Damage(float damage)
    {
        OnDamage(healthBase);
    }

    public void Damage(float damage, Vector3 dir)
    {
        OnDamage(healthBase);
    }
}
