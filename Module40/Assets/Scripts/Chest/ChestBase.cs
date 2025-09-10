using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ChestBase : MonoBehaviour
{
    public Animator animator;
    public string triggerOpen = "Open";
    public string triggerClose = "Close";

    public KeyCode keyCode = KeyCode.Z;

    [Header("Notification")]
    public GameObject notification;
    public float tweenDuration = .2f;
    public Ease tweenEase = Ease.OutBack;

    [Space]
    public ChestItemBase chestItem;
    
    public SFXType sfxType;


    private float startScale;

    private bool _chestOpened = false;

    void Start()
    {
        startScale = notification.transform.localScale.x;
        HideNotification();
    }

    private void Update()
    {
        if(Input.GetKeyDown(keyCode) && notification.activeSelf)
        {
            OpenChest();
        }
    }

    [NaughtyAttributes.Button]
    private void OpenChest()
    {
        if (_chestOpened)
        {
            return;
        }

        animator.SetTrigger(triggerOpen);
        _chestOpened = true;
        
        PlaySFX();

        HideNotification();

        Invoke(nameof(ShowItem), 1f);
    }

    private void ShowItem()
    {
        chestItem.ShowItem();

        Invoke(nameof(CollectItem), 1f);
    }

    private void CollectItem()
    {
        chestItem.Collect();
    }

    [NaughtyAttributes.Button]
    private void CloseChest()
    {
        if (!_chestOpened)
        {
            return;
        }

        animator.SetTrigger(triggerClose);
        
        // Só pode abrir o chest uma vez

        //_chestOpened = false; //Caso queira abrir mais vezes, mas precisa evitar a repetição de recompensa
    }

    private void OnTriggerEnter(Collider other)
    {
        Player p = other.transform.GetComponent<Player>();

        if (p != null)
        {
            ShowNotification();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player p = other.transform.GetComponent<Player>();

        if (p != null)
        {
            HideNotification();
            CloseChest();
        }

        if (notification.transform.localScale == Vector3.zero)
        {
            notification.SetActive(false);
        }
    }

    [NaughtyAttributes.Button]
    private void ShowNotification()
    {
        notification.SetActive(true);
        notification.transform.localScale = Vector3.zero;
        notification.transform.DOScale(startScale, tweenDuration);
    }

    [NaughtyAttributes.Button]
    private void HideNotification()
    {
        notification.transform.DOScale(Vector3.zero, tweenDuration).From(startScale).OnComplete(() => {
            notification.SetActive(false);
        });
    }

    private void PlaySFX()
    {
        SFXPool.Instance.Play(sfxType);
    }
}