using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UiHorizontaleShake : MonoBehaviour
{

    #region Public Attributes

    [SerializeField] float shakeForce;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeDuration;
    [SerializeField] RectTransform uiRect;

    private bool canShake;
    private Vector2 newPos;

    #endregion

    #region Main Methods

    private void Awake()
    {
        Shake();
    }

    public void Shake()
    {
        StartCoroutine(ShakeTimer());
        ShakeLeft();

        IEnumerator ShakeTimer()
        {
            canShake = true;
            yield return new WaitForSeconds(shakeDuration);
            canShake = false;
        }
    }

    private void ShakeLeft()
    {
        if (!canShake)
            return;

        newPos = new Vector2(uiRect.anchoredPosition.x + shakeForce, uiRect.anchoredPosition.y);
        uiRect.DOAnchorPos(newPos, shakeSpeed).OnComplete(ShakeRight);
    }

    private void ShakeRight()
    {
        if (!canShake)
            return;

        newPos = new Vector2(uiRect.anchoredPosition.x - shakeForce, uiRect.anchoredPosition.y);
        uiRect.DOAnchorPos(newPos, shakeSpeed).OnComplete(ShakeLeft);
    }

    #endregion

}
