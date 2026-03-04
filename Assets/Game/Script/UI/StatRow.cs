using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StatRow : MonoBehaviour
{
    [SerializeField]
    private Stat.Type statType;
    public Stat.Type StatType{
        get{
            return statType;
        }
        set{
            statType = value;
            SetName();
        }
    }
    [SerializeField]
    private bool simpleName = false;

    [SerializeField]
    private TextMeshProUGUI nameText,valueText;

    private Stat stat;
    public Stat Stat{
        get{
            return stat;
        }
        set{
            stat = value;
        }
    }

    private Stat compare;
    public Stat Compare{
        get{
            return compare;
        }
        set{
            compare = value;
        }
    }


    private float value = -99999f;

    void Start(){

        SetName();
    }

    void SetName(){
        string text = "";
        if(simpleName)
            text = "<sprite name="+statType.ToString()+">";
        else
            text = Util.LocalStr("Stat",statType.ToString());

        
        nameText.text = text;
    }

    void Update(){
        UpdateValue(this.stat[(int)statType]);
    }
    // bool IsRateType(Stat.Type type){
    //     switch(type){
    //         case Stat.Type.absorb: case Stat.Type.critRate: case Stat.Type.critDamage: case Stat.Type.coolRdx: case Stat.Type.avoid:
    //             return true;
    //         default:
    //             return false;
    //     }
    // }
    // bool IsFloatType(Stat.Type type){
    //     switch(type){
    //         case Stat.Type.attackSpeed: case Stat.Type.moveSpeed:
    //             return true;
    //         default:
    //             return false;
    //     }
    // }

    void UpdateValue(float v){
        if(value != v){
            value = v;
            string text = Util.StatToString(value,statType);
            // if(Info.IntStat[(int)statType] != 0)
            //     text = Mathf.FloorToInt(value).ToString();
            // else if(Info.FloatStat[(int)statType] != 0)
            //     text = (Mathf.Round(value*100f)/100f).ToString("F2");
            // else
            //     text = (Mathf.Round(value*100f)).ToString()+"%";

            //text = Util.StatToString(value,(int)statType);

            if(compare != null){
                float compareValue =  (value) - (compare[(int)statType]);

                string compareString = Util.StatToString(compareValue,statType,true);
                
                //if(Info.IntStat[(int)statType] != 0)
                //    compareValue = Mathf.FloorToInt(value) - Mathf.FloorToInt(compare[(int)statType]);
                // else if(Info.FloatStat[(int)statType] != 0)
                //     compareValue = Mathf.Round(value*100f)/100f - Mathf.Floor(compare[(int)statType]*100f)/100f;
                // else
                //     compareValue = Mathf.Round(value*100f) - Mathf.Floor(compare[(int)statType]*100f);
                
                //float compareValue = Mathf.Floor(value) - Mathf.Floor(compare[(int)statType]);
                if(compareValue>0){
                    text += " <color=green>(▲"+compareString+")</color>";
                }
                else if(compareValue<0){
                    text += " <color=red>(▼"+compareString+")</color>";
                }
                else{
                    text += " (-)";
                }
            }
            valueText.text = text;

        }

    }
    

}