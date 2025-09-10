using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cloth
{
    public class ClothItemCartoony : ClothItemBase
    {
        public override void Collect()
        {
            if (Player.Instance == null) return;

            base.Collect();
            Player.Instance.puCloth = "Cartoony";
        }
    }
}