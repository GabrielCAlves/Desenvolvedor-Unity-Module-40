using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cloth;

public class HealthBase : MonoBehaviour
{
    public int startLife = 10;

    [SerializeField] public float currentLife;
    public bool destroyOnKill = false;

    public UIFillUpdater uiFillUpdater;
    //
    public Action<HealthBase> OnDamage;
    public Action<HealthBase> OnKill;

    public float damageMultiply = 1;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        //destroyOnKill = false;
        if (gameObject.CompareTag("Player") && SaveManager.Instance.Setup.healthBar != 0)
        {
            currentLife = SaveManager.Instance.Setup.healthBar;
            UpdateUI();
        }
        else
        {
            currentLife = startLife;

            ResetLife();
        }
    }

    public void ResetLife()
    {
        currentLife = startLife;
        UpdateUI();
    }

    public void Damage(float amount)
    {
        currentLife -= amount * damageMultiply;

        if (currentLife <= 0 && !CheckpointManager.Instance.HasCheckpoint())
        {
            Kill();
        }

        UpdateUI();

        OnDamage?.Invoke(this);
    }

    [NaughtyAttributes.Button]
    public void Damage()
    {
        Damage(5);
    }

    protected virtual void Kill()
    {
        if (destroyOnKill)
        {
            Destroy(gameObject, 3f);
        }

        OnKill?.Invoke(this);
    }

    public void UpdateUI()
    {
        if(uiFillUpdater != null)
        {
            uiFillUpdater.UpdateValue((float) (currentLife/startLife));

            if (currentLife > 7.5f && currentLife <= 10f)
            {
                uiFillUpdater.uiImage.color = Color.green;
            }
            else if (currentLife > 5f && currentLife <= 7.5f)
            {
                uiFillUpdater.uiImage.color = Color.yellow;
            }
            else if (currentLife > 2f && currentLife <= 5f)
            {
                uiFillUpdater.uiImage.color = new Color(1f, 0.5f, 0f);
            }
            else if (currentLife > 0f && currentLife <= 2f)
            {
                uiFillUpdater.uiImage.color = Color.red;
            }
        }
    }

    public void ChangeDamageMultiply(float damageMultiply, float duration)
    {
        StartCoroutine(ChangeDamageMultiplyCoroutine(damageMultiply, duration));
    }

    IEnumerator ChangeDamageMultiplyCoroutine(float damageMultiply, float duration)
    {
        this.damageMultiply = damageMultiply;
        yield return new WaitForSeconds(duration);
        this.damageMultiply = 1;
        //Player.Instance.puCloth = "Health";
    }
}
