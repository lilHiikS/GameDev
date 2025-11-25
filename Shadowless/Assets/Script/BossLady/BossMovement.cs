using UnityEngine;
using System.Collections;

namespace Script.BossLady
{
    public class BossMovement : MonoBehaviour
    {
        public Transform[] movePoints;
        public float minWait = 2f;
        public float maxWait = 10f;
        public float moveSpeed = 6f; // rettet
        public LineRenderer teleportLine; // assign a LineRenderer prefab in Inspector
        public Animator animator;
        public boss_lady_script bossLadyScript;
        private Rigidbody2D rb2D;

        void Start()
        {
            rb2D = GetComponent<Rigidbody2D>();
            if (bossLadyScript == null)
                bossLadyScript = GetComponent<boss_lady_script>();
            if(animator == null)
                animator = GetComponent<Animator>();

            StartCoroutine(MovementRoutine()); // rettet stavefejl
        }

        IEnumerator MovementRoutine()
        {
            while (true)
            {
                float wait = Random.Range(minWait, maxWait);
                yield return new WaitForSeconds(wait);

                Transform target = movePoints[Random.Range(0, movePoints.Length)];
                int moveType = Random.Range(0, 2); // 0 = teleport, 1 = løb

                if (moveType == 0)
                {
                    if (rb2D != null)
                    {
                        rb2D.gravityScale = 0;
                    }
                    bossLadyScript.currentState = boss_lady_script.BossState.Run;
                    // TELEPORT med rød linje
                    if (teleportLine != null)
                    {
                        teleportLine.positionCount = 2; // Sørg for at der er 2 punkter
                        teleportLine.SetPosition(0, transform.position);
                        teleportLine.SetPosition(1, target.position);
                        teleportLine.enabled = true;
                    }
                    
                    // Glid hen mod target over 0.5 sekunder
                    float elapsed = 0f;
                    float duration = 0.5f;
                    Vector3 startPos = transform.position;

                    while (elapsed < duration)
                    {
                        transform.position = Vector3.Lerp(startPos, target.position, elapsed / duration);
                        elapsed += Time.deltaTime;
                        yield return null;
                    }

                    transform.position = target.position;
                    
                    bossLadyScript.currentState = boss_lady_script.BossState.Attack;
                    if (bossLadyScript != null)
                    {
                        bossLadyScript.FacePlayer();
                    }

                    if (teleportLine != null)
                    {
                        yield return new WaitForSeconds(0.2f); // vis linjen kort
                        teleportLine.enabled = false;
                    }
                }
                else
                {
                    // LØB
                    bossLadyScript.currentState = boss_lady_script.BossState.Run;
                    if (rb2D != null)
                    {
                        rb2D.gravityScale = 0;
                    }
                    if(animator != null)
                    {
                        animator.SetBool("isIdle", false);
                        animator.SetBool("isAttacking", false);
                        animator.SetBool("isRunning", true); // sæt running
                    }
                    while (Vector3.Distance(transform.position, target.position) > 0.1f)
                    {
                        transform.position = Vector3.MoveTowards(
                            transform.position,
                            target.position,
                            moveSpeed * Time.deltaTime
                        );
                        yield return null;
                    }
                    // Når hun er nået frem:
                    bossLadyScript.currentState = boss_lady_script.BossState.Attack;
                    if (bossLadyScript != null)
                    {
                        bossLadyScript.FacePlayer();
                    }
                    if(animator != null)
                    {
                        animator.SetBool("isRunning", false);
                        animator.SetBool("isAttacking", true);
                    }
                }
            }
        }
    }
}
