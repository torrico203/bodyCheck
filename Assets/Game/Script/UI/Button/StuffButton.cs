using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StuffButton : PoolingObject
{   
    [SerializeField]
    private EquipmentButton equipment;
    public EquipmentButton Equipment{
        get => equipment;
    }
    [SerializeField]
    private WealthButton wealth;
    public WealthButton Wealth{
        get => wealth;
    }

    [SerializeField]
    private Image lightImage;

    private int grade = 0;

    [SerializeField]
    private Stuff stuff;
    public Stuff Stuff{
        get => stuff;
        set{
            stuff = value;
            if(stuff.equipment != null){
                if(stuff.equipment.infoKey != ""){
                    equipment.gameObject.SetActive(true);
                    equipment.data = stuff.equipment;
                    equipment.EquipChecker.SetActive(false);
                    equipment.SetEquipChecker();
                    equipment.Load();
                    lightImage.color = Info.GradeColor[(int)stuff.equipment.grade];
                    grade = (int)stuff.equipment.grade;
                }
                else{
                    equipment.gameObject.SetActive(false);
                }
            }else{
                equipment.gameObject.SetActive(false);
            }
            if(stuff.wealth != null){
                if(!stuff.wealth.IsEmpty()){
                    wealth.gameObject.SetActive(true);
                    for(int i = 0; i < stuff.wealth.Count(); i++){
                        if(stuff.wealth[i] > 0){
                            wealth.Type = (Wealth.Type)i;
                            wealth.Val = stuff.wealth[i];
                            break;
                        }
                    }
                    lightImage.color = Color.white;
                    grade = 0;
                }
                else{
                    wealth.gameObject.SetActive(false);
                }
                
            }else{
                wealth.gameObject.SetActive(false);
            }
        }
    }

    [SerializeField]
    private CanvasGroup light;

    protected override void OnInit(){
        transform.localScale = Vector3.one;
        light.alpha = 0;
        // if(equipment != null){
        //     equipment.EquipChecker.SetActive(false);
        //     equipment.SetEquipChecker();
        //     equipment.Load();
        // }
        
    }
    protected override void OnReturn(){
        equipment.idx = -1;
        equipment.data = null;
        equipment.EquipChecker.SetActive(false);
        equipment.IsSynthesis = false;
    }

    public void Show(){
        if(grade >= 3)
            Effect.Play("Power_Burst_V1",this.transform.position,UI.I.transform);
        else if(grade >= 6)
            Effect.Play("Power_Burst_V8",this.transform.position,UI.I.transform);
        light.DOFade(1,0.05f).SetEase(Ease.OutSine).OnComplete(() => {
            light.DOFade(0,0.1f).SetEase(Ease.InSine);
        });
    }
}