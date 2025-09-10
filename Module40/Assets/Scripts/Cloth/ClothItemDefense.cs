using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth
{
    public class ClothItemDefense : ClothItemBase
    {
        public float damageMultiply = .5f;

        public override void Collect()
        {
            if (Player.Instance == null) return;

            base.Collect();
            Player.Instance.healthBase.ChangeDamageMultiply(damageMultiply, duration);
            Player.Instance.puCloth = "Defense";
        }
    }
}