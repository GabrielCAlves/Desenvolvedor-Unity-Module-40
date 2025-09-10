using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth
{
    public class ClothItemHealth : ClothItemBase
    {
        public float targetHealth = 100f;

        public override void Collect()
        {
            if (Player.Instance == null) return;

            base.Collect();
            Player.Instance.ChangeHealth(targetHealth, duration);
            Player.Instance.puCloth = "Health";
        }
    }
}