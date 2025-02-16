using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float targetScale = .93f;
    private Vector3 calculatedTargetScale;
    private float scaleDuration = .040f;
    private float recoverDuration = .025f;
    private Button button;
    
    private Tween scaleTween;
    private Vector3 initialScale;

    private bool isEnter;
    private bool isUp;
    private bool IsDown;
    private bool IsExit;

    private void Awake()
    {
        initialScale = transform.localScale;
        calculatedTargetScale = initialScale * targetScale;
        button = GetComponent<Button>();
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        IsExit = true;
        scaleTween.Complete();
        
        scaleTween = transform.DOScale(initialScale, recoverDuration).SetEase(Ease.Linear);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUp && IsDown && IsExit)
        {
            IsExit = false;
            scaleTween.Complete();
            if (button.interactable)
            {
                scaleTween = transform.DOScale(calculatedTargetScale, recoverDuration).SetEase(Ease.Linear);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDown = true;
        isEnter = true;
        isUp = false;
        IsExit = false;
        
        scaleTween.Complete();
        if (button.interactable)
        {
            scaleTween = transform.DOScale(calculatedTargetScale, recoverDuration).SetEase(Ease.Linear);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isUp = true;
        IsDown = false;
        IsExit = true;
        isEnter = false;
        
        scaleTween.Complete();
        if (button.interactable)
        {
            scaleTween = transform.DOScale(initialScale, recoverDuration).SetEase(Ease.Linear);
        }
    }
}
