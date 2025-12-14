using UnityEngine;

public class SpikeManager : MonoBehaviour
{
    public GameObject[] spikesSet1; // First segment (5 spikes)
    public GameObject[] spikesSet2; // Second segment (10 spikes)

    [Header("Base Timings (Slow)")] public float set1HazardTime = 2.5f; public float set1GapTime = 1.5f; public float set2HazardTime = 2.2f; public float set2GapTime = 1.3f;
    [Header("Pattern Settings")] public bool loopPatterns = true; public float set2StartDelay = 3.5f; public bool showPreviewGap = true; public float previewExtraTime = 0.75f;
    [Header("Animation")] public bool useAnimatorAttack = true; public string attackTriggerName = "Attack";
    [Header("Difficulty Ramp (Optional)")] public bool enableRamp = false; public float rampFactor = 0.95f; public float minHazardTime = 1.1f; public int cyclesBeforeRamp = 3;

    // Two safe spots per wave (pairs). Limited to 3 rotations for memorization.
    private int[][] safePairsSet1 = new int[][] { new int[] { 0, 3 }, new int[] { 1, 4 }, new int[] { 2, 4 } };
    private int[][] safePairsSet2 = new int[][] { new int[] { 0, 5 }, new int[] { 3, 8 }, new int[] { 1, 6 } };

    private int index1; private int index2; private int cycles1; private int cycles2;

    void Start()
    {
        StartCoroutine(RunSet1SafePairs());
        StartCoroutine(RunSet2SafePairs());
    }

    System.Collections.IEnumerator RunSet1SafePairs()
    {
        while (loopPatterns)
        {
            int[] pair = safePairsSet1[index1];
            if (useAnimatorAttack) FireAttackExceptTwo(spikesSet1, pair[0], pair[1]);
            yield return new WaitForSeconds(set1HazardTime);
            if (showPreviewGap)
            {
                // Optional preview: currently no deactivation; keep all visible
                yield return new WaitForSeconds(previewExtraTime);
            }
            else
            {
                yield return new WaitForSeconds(set1GapTime);
            }
            index1 = (index1 + 1) % safePairsSet1.Length;
            if (index1 == 0) cycles1++;
            if (enableRamp && cycles1 >= cyclesBeforeRamp)
            {
                set1HazardTime = Mathf.Max(set1HazardTime * rampFactor, minHazardTime);
            }
        }
    }

    System.Collections.IEnumerator RunSet2SafePairs()
    {
        yield return new WaitForSeconds(set2StartDelay);
        while (loopPatterns)
        {
            int[] pair = safePairsSet2[index2];
            if (useAnimatorAttack) FireAttackExceptTwo(spikesSet2, pair[0], pair[1]);
            yield return new WaitForSeconds(set2HazardTime);
            if (showPreviewGap)
            {
                yield return new WaitForSeconds(previewExtraTime);
            }
            else
            {
                yield return new WaitForSeconds(set2GapTime);
            }
            index2 = (index2 + 1) % safePairsSet2.Length;
            if (index2 == 0) cycles2++;
            if (enableRamp && cycles2 >= cyclesBeforeRamp)
            {
                set2HazardTime = Mathf.Max(set2HazardTime * rampFactor, minHazardTime);
            }
        }
    }

    void FireAttackExceptTwo(GameObject[] set, int a, int b)
    {
        if (set == null) return;
        for (int i = 0; i < set.Length; i++)
        {
            if (i == a || i == b) continue;
            var go = set[i];
            if (go == null) continue;
            var anim = go.GetComponent<Animator>();
            if (anim != null && !string.IsNullOrEmpty(attackTriggerName)) anim.SetTrigger(attackTriggerName);
        }
    }

}