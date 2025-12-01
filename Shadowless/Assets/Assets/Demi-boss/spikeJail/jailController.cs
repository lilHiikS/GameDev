using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class jailController : MonoBehaviour
{
    public List<GameObject> spikes;
    public CinemachineCamera cam;
    public float zoomTarget = 16f;
    public float zoomDuration = 1f;
    public float zoomInTarget = 12f;
    public float zoomInDuration = 0.8f;
    public float delayAfterBossEnable = 2f;

    private bool hasActivated = false;
    public GameObject boss;

    void Start()
    {
        cam = GameManager.Instance.GetComponentInChildren<CinemachineCamera>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasActivated)
        {
            hasActivated = true;
            StopAllCoroutines();
            StartCoroutine(AnimateZoomThenActivate(cam, zoomTarget, zoomDuration));
        }
    }

    System.Collections.IEnumerator AnimateZoomThenActivate(CinemachineCamera camera, float targetSize, float duration)
    {
        float startSize = camera.Lens.OrthographicSize;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            camera.Lens.OrthographicSize = Mathf.Lerp(startSize, targetSize, alpha);
            yield return null;
        }
        camera.Lens.OrthographicSize = targetSize;
        for (int i = 0; i < spikes.Count; i++)
        {
            var spike = spikes[i];
            if (spike != null) spike.SetActive(true);
        }
        if (boss != null) boss.SetActive(true);

        // Wait so the boss drop is visible, then zoom back in
        yield return new WaitForSeconds(delayAfterBossEnable);

        float startIn = camera.Lens.OrthographicSize;
        float tIn = 0f;
        while (tIn < zoomInDuration)
        {
            tIn += Time.deltaTime;
            float alphaIn = Mathf.Clamp01(tIn / zoomInDuration);
            camera.Lens.OrthographicSize = Mathf.Lerp(startIn, zoomInTarget, alphaIn);
            yield return null;
        }
        camera.Lens.OrthographicSize = zoomInTarget;
    }
}
