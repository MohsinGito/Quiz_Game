using DG.Tweening;
using UnityEngine;

public class ZoomInOutAnim : MonoBehaviour
{
    public float zoomFactor;
    public float speed;

    private RectTransform m_Rect;
    private Vector3 originalScale;

    private void Awake()
    {
        m_Rect = GetComponent<RectTransform>();
        originalScale = m_Rect.localScale;
    }

    private void ZoomIn()
    {
        m_Rect.DOScale(originalScale + (Vector3.one * zoomFactor), speed).OnComplete(ZoomOut);
    }

    private void ZoomOut()
    {
        m_Rect.DOScale(originalScale, speed).OnComplete(ZoomIn);
    }

    private void OnEnable()
    {
        ZoomIn();
    }

    private void OnDisable()
    {
        m_Rect.DOKill();
    }
}
