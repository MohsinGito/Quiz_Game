using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomBehaviour : MonoBehaviour
{

    #region Main Attributes

    [SerializeField] List<ZoomComponent> zoomComponents;
    private List<Coroutine> animCoroutines = new List<Coroutine>();

    #endregion

    #region Main Methods

    public ZoomComponent ZoomComponent
    {
        set
        {
            if(value == null)
            {
                zoomComponents = new List<ZoomComponent>();
            }
            else
            {
                zoomComponents.Add(value);
            }
        }
    }

    protected void DisplayComponents()
    {
        ResetAnim();
        ResetState();

        foreach (ZoomComponent zoomComponent in zoomComponents)
            animCoroutines.Add(StartCoroutine(StartDisplaying(zoomComponent)));

        IEnumerator StartDisplaying(ZoomComponent zoomComponent)
        {
            foreach (Zoom zoomInfo in zoomComponent.zoomInfos)
            {
                if (zoomComponent.sequential)
                    yield return zoomInfo.m_Rect.DOScale(Vector3.one, zoomInfo.displaySpeed).WaitForCompletion();
                else
                    zoomInfo.m_Rect.DOScale(Vector3.one, zoomInfo.displaySpeed);
            }
        }
    }

    protected void HideComponents()
    {
        ResetAnim();
        foreach (ZoomComponent zoomComponent in zoomComponents)
        {
            foreach (Zoom zoomInfo in zoomComponent.zoomInfos)
            {
                zoomInfo.m_Rect.DOScale(Vector3.zero, zoomInfo.hideSpeed);
            }
        }
    }

    private void ResetAnim()
    {
        foreach (Coroutine coroutine in animCoroutines)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        animCoroutines = new List<Coroutine>();
        foreach (ZoomComponent zoomComponent in zoomComponents)
        {
            foreach (Zoom zoomInfo in zoomComponent.zoomInfos)
            {
                zoomInfo.m_Rect.DOKill();
            }
        }
    }

    private void ResetState()
    {
        foreach (ZoomComponent zoomComponent in zoomComponents)
        {
            foreach (Zoom zoomInfo in zoomComponent.zoomInfos)
            {
                zoomInfo.m_Rect.localScale = Vector3.zero;
            }
        }
    }

    #endregion

}

[System.Serializable]
public class ZoomComponent
{
    public bool sequential;
    public List<Zoom> zoomInfos;

    public ZoomComponent(bool _sequential, List<Zoom> _zoomInfos)
    {
        sequential = _sequential;
        zoomInfos = _zoomInfos;
    }
}

[System.Serializable]
public class Zoom
{
    public float displaySpeed;
    public float hideSpeed;
    public RectTransform m_Rect;

    public Zoom(float _displaySpeed, float _hideSpeed, RectTransform _rectTransform)
    {
        displaySpeed = _displaySpeed;
        hideSpeed = _hideSpeed;
        m_Rect = _rectTransform;
    }
}