using UnityEngine;

namespace Script.BossLady.attacks
{
    public class BossShooter : MonoBehaviour
    {
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private Transform missileSpawn;
        [SerializeField] private float projectileSpeed = 10f; // hastighed på missil
        public void Shoot()
        {
            if (missilePrefab == null || missileSpawn == null)
            {
                Debug.LogError("MissilePrefab eller MissileSpawn er ikke sat!");
                return;
            }

            GameObject missile = Instantiate(missilePrefab, missileSpawn.position, Quaternion.identity);
            MagicHomingMissile homing = missile.GetComponent<MagicHomingMissile>();

            if (homing != null)
            {
                float dir = transform.parent != null 
                    ? Mathf.Sign(transform.parent.localScale.x) 
                    : Mathf.Sign(transform.localScale.x);
                homing.Initialize(dir);
            }
            else
            {
                Debug.LogError("MissilePrefab mangler MagicHomingMissile komponenten!");
            }

            Debug.Log("Missile shot");
        }

    }
}