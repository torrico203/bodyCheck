using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Demo_Project;

public class SkillExplainUI : MonoBehaviour
{
    
    public SkillType type;

    private string infoKey;
    public string InfoKey{
        get{return infoKey;}
        set{infoKey = value;}
    }

    private int level = 1;
    public int Level{
        get{return level;}
        set{level = value;}
    }

    [SerializeField]
    private TextMeshProUGUI nameText,explainText,activeText;

    [SerializeField]
    private Image icon,color;





    void Start(){
        
    }

    void Update(){
        
    }

    public void Set(ActiveData data){
        infoKey = data.infoKey;
        level = data.level;

        type = SkillType.Active;

        Setup();
    }

    public void Setup(){
        Grade grade = Grade.Common;
        string aText = "";
        if(type == SkillType.Active){
            icon.sprite = Info.Active[infoKey].icon;
            nameText.text = "LV."+level+" "+Util.LocalStr("Name",infoKey);

            List<ActiveExplain> activeExplain = new List<ActiveExplain>();
            for(int i=0;i<Info.Active[infoKey].behaviours.Length;i++){
                ActiveInfo.Behaviour behaviour = Info.Active[infoKey].behaviours[i];
                ActiveExplain explain = new ActiveExplain();
                explain.deal = behaviour.deal + (behaviour.lDeal * (level-1));
                explain.variable = behaviour.variable + (behaviour.lVariable * (level-1));
                explain.variable.Floor();
                activeExplain.Add(explain);
            }
            
            explainText.text = Util.LocalStr("Explain",infoKey,new[]{activeExplain});
            
                
            aText += Util.LocalStr("UI","Range") + " : ";
            if(Info.Active[infoKey].range>0)
                aText += Info.Active[infoKey].range+"m\n";
            else
                aText += Util.LocalStr("UI","DefaultAttack")+"\n";
            aText += Util.LocalStr("UI","CoolTime") + " : ";
            if(Info.Active[infoKey].coolTime>0)
                aText += Info.Active[infoKey].coolTime+"s";
            else 
                aText += Util.LocalStr("UI","DefaultAttack");
            

            grade = Info.Active[infoKey].grade;
        }
        else if(type == SkillType.Passive){
            icon.sprite = Info.Passive[infoKey].icon;
            nameText.text = "LV."+level+" "+Util.LocalStr("Name",infoKey);
            Stat stat = Util.DeepCopy<Stat>(Info.Passive[infoKey].stat);
            //explainText.text = "";//Util.LocalStr("Explain",infoKey,new[]{stat});

            
            if(UI.Game.Manager!=null && !Info.Passive[infoKey].addition){
                for(int i=0;i<stat.Count();i++){
                    if(stat[i] != 0){
                        stat[i] = UI.Game.Player.NStat[i] * stat[i];
                        if(Info.IntStat[i] != 0) {
                            stat[i] = Mathf.Round(stat[i]);
                            if(stat[i]<1) stat[i] = 1;
                        }
                    }
                }
            }
            
            
            explainText.text = Util.StatToExplain(stat);
            // for(int i=0;i<stat.Count();i++){
            //     if(stat[i] != 0){
            //         explainText.text += Util.LocalStr("Stat",((Stat.Type)i).ToString());
            //         float value = stat[i];
            //         if(nStat && UI.Game.Manager!=null && !Info.Passive[infoKey].addition){
            //             value = UI.Game.Player.NStat[i] * stat[i];
            //             if(Info.IntStat[i] != 0) {
            //                 value = Mathf.Round(value);
            //                 if(value<1) value = 1;
            //             }
            //         }
            //         string valueStr = Util.StatToString(value,(Stat.Type)i);

            //         if(stat[i] > 0){
            //             explainText.text += " " +Util.LocalStr("UI","increase",new[]{new{value = (valueStr)}});
            //         }
            //         else{
            //             explainText.text += " " +Util.LocalStr("UI","decrease",new[]{new{value = (valueStr)}});
            //         }
                    
            //     }
            // }

            grade = Info.Passive[infoKey].grade;
        }
        else if(type == SkillType.Reward){
            icon.sprite = Info.Reward[infoKey].icon;
            nameText.text = Util.LocalStr("Name",infoKey);
            explainText.text = Util.LocalStr("Explain",infoKey);

            grade = Info.Reward[infoKey].grade;
        }

        if(color != null){
            Color _c = Info.GradeColor[(int)grade];
            Color c = new Color(_c.r,_c.g,_c.b,color.color.a);
            color.color = c;
        }

        if(activeText != null)
            activeText.text = aText;
        
    }
    

}