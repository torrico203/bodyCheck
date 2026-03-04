using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatAdder : MonoBehaviour
{
    [SerializeField]
    private Stat.Type type;

    [SerializeField]
    private UIGauge gauge;

    [SerializeField]
    private TextMeshProUGUI valueText,lStatText,priceText;

    private float value;
    private int level;

    private int price = 10;

    private bool isMax = false;
    public bool IsMax { get => isMax; }

    void Awake(){
        
    }

    void Start(){
        lStatText.text = "<sprite name="+type.ToString()+"> + "+Info.Player.lStat[(int)type].ToString();
        //Sound.PlayBGM("Title");
    }

    void Update(){
        Reload();
    }

    void Reload(){
        if(value != Data.Player.stat[(int)type] || this.level != Data.Player.level){
            this.level = Data.Player.level;
            value = Data.Player.stat[(int)type];
            //gauge.SetValue(value);
            valueText.text = "<sprite name="+type.ToString()+"> "+value.ToString();

            int min = Mathf.FloorToInt(Info.Player.initial.stat[(int)type] + Info.Player.lStat[(int)type]*((Data.Player.level-1)*Info.MaxStat));
            int max = Mathf.FloorToInt(Info.Player.initial.stat[(int)type] + Info.Player.lStat[(int)type]*((Data.Player.level)*Info.MaxStat));

            
            gauge.Set(min,max,Mathf.FloorToInt(Data.Player.stat[(int)type]));

            price = 10+Mathf.FloorToInt((Mathf.FloorToInt(Data.Player.stat[(int)type])-Mathf.FloorToInt(Info.Player.initial.stat[(int)type]))/Info.Player.lStat[(int)type]*10);
            priceText.text = "<sprite name=exp> "+price.ToString();

            if(Data.Player.stat[(int)type] >= max){
                isMax = true;
                priceText.text = "<sprite name=exp> MAX";
            }
            else{
                isMax = false;
            }

        }
    }

    public void AddStat(){
        if(!isMax){
            if(Data.UseWealth(Wealth.Type.exp, price)){
                //Data.Player.stat[(int)type] += Info.Player.lStat[(int)type];
                Data.AddStat(type, Info.Player.lStat[(int)type]);
                Reload();

                Effect.Play("Power_Burst_V8",valueText.transform.position,UI.I.transform);
            }
        }
        else{

        }
        
    }

    

}