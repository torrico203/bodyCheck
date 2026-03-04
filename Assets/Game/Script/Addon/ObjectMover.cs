using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class ObjectMover : MonoBehaviour
{
    [SerializeField] private float delay = 0f;
    [SerializeField] private float duration = 0f;
    [SerializeField] private Vector3 target = Vector3.zero, origin = Vector3.zero;
    [SerializeField] private Ease ease = Ease.Linear;

    void OnEnable(){
        transform.localPosition = origin;
        transform.DOLocalMove(target, duration).SetDelay(delay).SetEase(ease);
    }

}