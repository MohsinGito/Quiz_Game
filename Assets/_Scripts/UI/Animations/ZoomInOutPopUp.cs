using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoomInOutPopUp : ZoomBehaviour
{

    #region Main Attributes

    [Header("Animation Tweaking")]
    [SerializeField] float animationSpeed;
    [SerializeField] List<float> animScaleFactors;

    Vector3 originalSize;
    RectTransform m_RectTransfrom;
    CanvasGroup m_CanvasGroup;
    Coroutine animCoroutine;

    #endregion

    #region Main Methods

    private void Awake()
    {
        originalSize = Vector3.one;
        m_RectTransfrom = GetComponent<RectTransform>();
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    public void Animate(bool _state)
    {
        if(_state)
        {
            DisplayPopUp();
            DisplayComponents();
        }
        else
        {
            HidePopUp();
            HideComponents();
        }
    }

    private void DisplayPopUp()
    {
        ResetAnim();

        animCoroutine = StartCoroutine(Animate());
        IEnumerator Animate()
        {
            int factorIndex = 0;
            float animSpeed = animationSpeed;
            Vector3 animSize = Vector3.zero;

            if(m_CanvasGroup != null)
            {
                m_CanvasGroup.alpha = 0;
                m_CanvasGroup.DOFade(1, animationSpeed);
            }

            for (int i = 0; i < animScaleFactors.Count * 2 && m_RectTransfrom != null; i++)
            {
                if ((i + 1) % 2 == 0)
                {
                    animSpeed -= (animationSpeed * 0.1f);
                    animSize = originalSize;
                }
                else
                {
                    animSpeed -= (animationSpeed * 0.1f);
                    animSize = originalSize * animScaleFactors[factorIndex++];
                }

                if (i == 0) { m_RectTransfrom.localScale = animSize; continue; }
                yield return m_RectTransfrom.DOScale(animSize, animSpeed).WaitForCompletion();
            }
        }
    }

    private void HidePopUp()
    {
        ResetAnim();
        if (m_CanvasGroup != null) m_CanvasGroup.DOFade(0, animationSpeed);
        if (m_RectTransfrom != null) m_RectTransfrom.DOScale(originalSize * animScaleFactors[0], animationSpeed); 
    }

    private void ResetAnim()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        if (m_CanvasGroup != null) m_CanvasGroup.DOKill();
        if (m_RectTransfrom != null) m_RectTransfrom.DOKill();
    }

    private void OnDisable()
    {
        if (m_CanvasGroup != null) m_CanvasGroup.DOKill();
        if (m_RectTransfrom != null) m_RectTransfrom.DOKill();
    }

    #endregion

}
