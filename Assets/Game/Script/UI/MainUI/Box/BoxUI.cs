using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;


public class BoxUI : MonoBehaviour
{
    
    [SerializeField]
    private Sprite[] close;

    [SerializeField]
    private Sprite[] open;

    [SerializeField]
    private GameObject effect1;
    

    [SerializeField]
    private Image image;
    
    private int idx = 0;
    public int Idx{
        get { return idx; }
        set
        {
            idx = value;
            SetBox();
        }
    }

    private Sequence seq = null;


    

    void Awake()
    {
        
    }


    public void SetBox()
    {
        if (idx < 0 || idx >= close.Length)
        {
            Debug.LogError("Index out of range");
            return;
        }

        image.sprite = close[idx];
        image.transform.localScale = Vector3.one*0.7f;

        image.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        
    }

    public void Open(bool effect = true,bool openEffect = true,Action callback = null){
        if(seq != null) return;
        
        Action openCallback = () =>
        {
            image.transform.localRotation = Quaternion.identity;
            image.sprite = open[idx];
            if(openEffect){
                Effect.Play("Particles_Basic_Floor_V3",transform.position,UI.I.transform);
                image.transform.localScale = Vector3.one * 0.7f;
                image.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    callback?.Invoke();
                    seq = null;
                    effect1.SetActive(false);
                    //image.sprite = close[idx];
                });
            }
            else{
                callback?.Invoke();
                seq = null;
                effect1.SetActive(false);
            }
                
        };
        image.sprite = close[idx];

        if(effect){
            effect1.SetActive(true);

            seq = DOTween.Sequence();

            float d = 0.1f;
            float duration = 0f;
            for(int i=0;i<30;i++){
                
                duration += d;
                seq.Append(image.transform.DORotate(new Vector3(0,0,UnityEngine.Random.Range(-10f,10f)),d));
                seq.Join(image.transform.DOLocalMoveX(UnityEngine.Random.Range(-20f,20f),d));
                d *= 0.98f;
            }
            Debug.Log("duration : "+duration);
            seq.Append(image.transform.DOLocalMoveX(0f,0f));
            seq.OnComplete(() =>
            {
                openCallback();
            });
        }
        else{
            openCallback();
        }
 
    }

    public void Close(){
        if(seq != null) return;
        //effect1.SetActive(false);
        image.sprite = close[idx];
    }
    
    

}