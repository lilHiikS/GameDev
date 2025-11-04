using UnityEngine;

namespace Script.BossLady.attacks
{
    public class BossShooter : MonoBehaviour
    {
        [SerializeField] private GameObject missilePrefab;
        [SerializeField] private Transform missileSpawn;

        public void Shoot()
        {
            Instantiate(missilePrefab, missileSpawn.position, Quaternion.identity);
            Debug.Log("Missile shot");
        }
    }
}