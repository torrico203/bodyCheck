using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageText : PoolingObject
{

    public string text;
    public int floor = 0;

    public float startY = 1f;

    public float scale = 0.5f;

    public Color color = Color.grey;

    [SerializeField]
    private TextMeshPro tmp;


    [SerializeField]
    private Ease ease = Ease.OutBack;
 
    void Awake()
    {

    }

    void Update()
    {
    }

 
    protected override void OnInit(){
        tmp.text = this.text;
        tmp.transform.localPosition = new Vector3(0f,startY,0f);
        tmp.transform.localScale = Vector3.zero;
        tmp.alpha = 0f;

        float objY = startY+floor*(0.5f*scale);

        tmp.color = color;
        

        Sequence seq = DOTween.Sequence();
        seq.Append(this.tmp.transform.DOLocalMove(new Vector3(Random.Range(-0.25f,0.25f),objY,0f), 0.3f).SetEase(ease));
        seq.Join(this.tmp.transform.DOScale(Vector3.one*scale, 0.3f).SetEase(ease));
        seq.Join(this.tmp.DOFade(0.9f, 0.3f).SetEase(ease));
        seq.Append(this.tmp.transform.DOLocalMoveY(objY+0.1f, 0.3f));
        seq.Join(this.tmp.DOFade(0f, 0.3f).SetEase(Ease.InExpo));
        seq.OnComplete(()=>{
            ReturnObject();
        });
        
    }
    protected override void OnReturn(){
        
    }
    
    
    
}
