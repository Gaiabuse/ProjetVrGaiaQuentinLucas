using UnityEngine;

public class ZoomOnObject : MonoBehaviour
{
    public float zoomSpeed = 2f;
    public float zoomFOV = 30f;
    public float normalFOV = 60f;
    private Camera cam;
    private bool isZooming = false;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    public void ZoomIn()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomToFOV(zoomFOV));
    }

    public void ZoomOut()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomToFOV(normalFOV));
    }

    System.Collections.IEnumerator ZoomToFOV(float targetFOV)
    {
        isZooming = true;
        while (Mathf.Abs(cam.fieldOfView - targetFOV) > 0.1f)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        cam.fieldOfView = targetFOV;
        isZooming = false;
    }
}
