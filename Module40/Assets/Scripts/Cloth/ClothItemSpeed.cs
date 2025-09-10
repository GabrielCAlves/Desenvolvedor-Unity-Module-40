using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth
{
    public class ClothItemSpeed : ClothItemBase
    {
        public float targetSpeed = 2f;

        public override void Collect()
        {
            if (Player.Instance == null) return;

            base.Collect();
            Player.Instance.ChangeSpeed(targetSpeed, duration);
            Player.Instance.puCloth = "Speed";
        }
    }
}