using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunShootLimit : GunBase
{
    public List<UIFillUpdater> uiFillUpdaters;

    public float maxShoot = 5f;
    public float timeToRecharge = 1f;

    public SFXType sfxType;

    private float _currentShoots;
    private bool _recharging = false;

    private void Awake()
    {
        GetAllUIs();
    }

    protected override IEnumerator ShootCoroutine()
    {
        if (_recharging)
        {
            yield break;
        }

        while (true)
        {
            if (Player.Instance.healthBase.currentLife <= 0)
            {
                yield break;
            }

            if(_currentShoots < maxShoot)
            {
                Shoot();
                PlaySFX();
                ++_currentShoots;
                CheckRecharge();
                UpdateUI();
                yield return new WaitForSeconds(timeBetweenShoot);
            }
        }
    }

    private void PlaySFX()
    {
        SFXPool.Instance.Play(sfxType);
    }

    private void CheckRecharge()
    {
        if(_currentShoots >= maxShoot)
        {
            StopShoot();
            StartRecharge();
        }
    }

    private void StartRecharge()
    {
        _recharging = true;
        StartCoroutine(RechargeCoroutine());
    }

    IEnumerator RechargeCoroutine()
    {
        float time = 0;
        while(time < timeToRecharge)
        {
            time += Time.deltaTime;
            Debug.Log("Recarregando arma. Tempo: "+time);
            uiFillUpdaters.ForEach(i => i.UpdateValue(time/timeToRecharge));
            yield return new WaitForEndOfFrame();
        }

        _currentShoots = 0;
        _recharging = false;
    }

    private void UpdateUI()
    {
        uiFillUpdaters.ForEach(i => i.UpdateValue(maxShoot, _currentShoots));
    }

    private void GetAllUIs()
    {
        uiFillUpdaters = GameObject.FindObjectsOfType<UIFillUpdater>().ToList();
        
        uiFillUpdaters.ForEach(i => Debug.Log("(Non-Filtered) UI name: " + i.name));

        GetMunitionUIs();
    }

    private void GetMunitionUIs()
    {
        uiFillUpdaters = uiFillUpdaters.FindAll(i => i.CompareTag("Munition")).ToList();

        uiFillUpdaters.ForEach(i => Debug.Log("(Filtered) UI name: "+i.name));
    }
}
