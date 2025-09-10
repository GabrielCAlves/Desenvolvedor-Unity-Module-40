using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth
{
    public class ClothItemBase : MonoBehaviour
    {
        public ClothType clothType;
        public float duration = 2f;

        public string compareTag = "Player";

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag(compareTag))
            {
                Collect();
            }
        }

        public virtual void Collect()
        {
            if (ClothManager.Instance == null || Player.Instance == null) return;

            var setup = ClothManager.Instance.GetSetupByType(clothType);
            if (setup != null)
            {
                Player.Instance.ChangeTexture(setup, duration);
                HideObject();
            }
        }

        private void HideObject()
        {
            gameObject.SetActive(false);
        }
    }
}