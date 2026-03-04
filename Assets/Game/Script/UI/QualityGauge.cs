using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QualityGauge : UIGauge
{
    
    private float colorRate = 0f;

    [SerializeField]
    private Ricimi.Gradient gradient;


    protected void OnGUI(){
        base.OnGUI();

        if(this.colorRate != this.rate){
            this.colorRate = this.rate;
            if(this.colorRate >= 1f){
                gradient.Color1 = new Color(0f,0.75f,0.5f,1f);
                gradient.Color2 = new Color(1f,0.5f,0f,1f);
            }
            else if(this.colorRate >= 0.9f){
                gradient.Color1 = new Color(0.75f,0.5f,1f,1f);
                gradient.Color2 = new Color(0.5f,0f,1f,1f);
            }
            else if(this.colorRate >= 0.7f){
                gradient.Color1 = new Color(0.5f,0.5f,1f,1f);
                gradient.Color2 = new Color(0f,0f,1f,1f);
            }
            else if(this.colorRate >= 0.3f){
                gradient.Color1 = new Color(0.5f,1f,0.5f,1f);
                gradient.Color2 = new Color(0f,1f,0f,1f);
            }
            else if(this.colorRate >= 0.1f){
                gradient.Color1 = new Color(1f,1f,0.5f,1f);
                gradient.Color2 = new Color(1f,1f,0f,1f);
            }
            else{
                gradient.Color1 = new Color(1f,0.5f,0.5f,1f);
                gradient.Color2 = new Color(1f,0f,0f,1f);
            }
        }
    }
}
