using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyShoot : EnemyBase
    {
        public GunBase gunBase;

        public override void Init()
        {
            base.Init();

            Debug.Log("(Shoot) (EnemyShoot) Init()");
            gunBase.StartShoot();
        }

        public void StopShooting()
        {
            gunBase.StopShoot();
        }
    }
}
