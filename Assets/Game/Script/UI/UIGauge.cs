using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIGauge : Gauge
{
    [SerializeField]
    protected Image bar;

    [SerializeField]
    protected TextMeshProUGUI valueText,nameText;

    public TextMeshProUGUI NameText { get => nameText; }

    [SerializeField]
    protected bool isValue, isRate;

    public bool isFull
    {
        get
        {
            return this.value >= this.max;
        }
    }

    public void Init(){
        this.rate = 0f;
        bar.fillAmount = this.rate;
    }

    public virtual void Set(long min, long max, long value)
    {
        this.min = min;
        this.max = max;
        this.value = value;
    }
    
    protected void OnGUI(){
        float rate = 0f;
        if(this.max-this.min != 0)
            rate = (float)(this.value-this.min) / (float)(this.max - this.min);

        if(rate < 0f) rate = 0f;
        if(rate > 1f) rate = 1f;

        float diff = rate - this.rate;
        if(0.001f>=diff&&diff>=-0.001f) this.rate = rate;
        else this.rate += (rate - this.rate)/10f;
        //Debug.Log(this.rate);

        bar.fillAmount = this.rate;

        if(valueText != null){
            string str = "";
            if(isValue){
                str += (this.value-this.min).ToString("#,##0")+"/" +(this.max - this.min).ToString("#,##0");
            }
            if(isRate){
                str += "("+(this.rate*100).ToString("F2") + "%)";
            }
            if(!isValue && !isRate){
                str = "";
            }
            valueText.text = str;
        }
    }
}
