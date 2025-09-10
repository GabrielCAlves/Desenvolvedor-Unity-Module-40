using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Items
{
    public class ItemCollectableBase : MonoBehaviour
    {
        public string compareTag = "Player";
        public ParticleSystem particleSystem;
        public AudioSource audioSource;
        public float timeToEliminateAS = 2f;

        private TextMeshProUGUI textMeshProUGUI;

        public ItemType itemType;

        public SFXType sfxType;

        private void Awake()
        {
            textMeshProUGUI = GameObject.Find("Text_(TMP)_Contabilizador").GetComponent<TextMeshProUGUI>();

            if (particleSystem != null)
            {
                particleSystem.transform.SetParent(null);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Entrou no trigger do ItemCollectableBase");

            if (other.gameObject.CompareTag(compareTag))
            {
                Collect();
            }
        }

        private void PlaySFX()
        {
            SFXPool.Instance.Play(sfxType);
        }

        protected virtual void Collect()
        {
            PlaySFX();
            gameObject.SetActive(false);
            OnCollect();
        }

        protected virtual void OnCollect()
        {
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            if (audioSource != null)
            {
                audioSource.transform.SetParent(null);
                audioSource.Play();
            }

            ItemManager.Instance.AddByType(itemType);
        }
    }

}