using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ActiveSlotButton : MonoBehaviour
{   
    [SerializeField]
    private int slotNo;
    public int SlotNo { 
        get => slotNo; 
        set{
            slotNo = value;
            if(Info.Active[UI.Game.Player.Active[slotNo].infoKey].icon!=null){
                icon.sprite = Info.Active[UI.Game.Player.Active[slotNo].infoKey].icon;
            }
        }
    }

    public bool on = true;
    private bool _on = true;

    [SerializeField]
    private Image frame, icon, cooltimeBar, onEffect;
    [SerializeField]
    private TextMeshProUGUI cooltimeText,levelText;

    [SerializeField]
    private CanvasGroup canvasGroup;

    private int level = 0;



    private bool isActive = false;

    private Tween tween = null;

    void Start(){
    }

    void Update(){
        
        float now = Time.time;

        if(level != UI.Game.Player.Active[slotNo].level){
            level = UI.Game.Player.Active[slotNo].level;
            levelText.text = "LV"+level;
        }

        // if(UI.Game.SlotStatus.Count>slotNo){
        //     if(UI.Game.SlotStatus[slotNo] != isActive){
        //         isActive = UI.Game.SlotStatus[slotNo];
        //         if(isActive) {
        //             onEffect.gameObject.SetActive(true);
        //             //ActiveEffect();
        //         }
        //         else{
        //             onEffect.gameObject.SetActive(false);
        //         }
        //     }
        // }

        if(on!=_on){
            _on = on;
            if(on){
                onEffect.gameObject.SetActive(true);
                canvasGroup.alpha = 1f;
            }
            else{
                onEffect.gameObject.SetActive(false);
                canvasGroup.alpha = 0.2f;
            }
        }

        

        float coolTime = 1f/UI.Game.Player.NStat.attackSpeed;
        if(Info.Active[UI.Game.Player.Active[slotNo].infoKey].type != ActiveType.DefaultAttack)
            coolTime = Info.Active[UI.Game.Player.Active[slotNo].infoKey].coolTime * (1f - UI.Game.Player.NStat.coolRdx);
        float remain = coolTime - (now - UI.Game.Player.Active[slotNo].useTime);
        if(remain > 0f){
            cooltimeBar.fillAmount = remain/coolTime;
            cooltimeText.text = remain.ToString("0.0");  
        }
        else{
            if(cooltimeBar.fillAmount > 0f)
            {
                cooltimeBar.fillAmount = 0f;
                cooltimeText.text = "";
                if(tween != null) tween.Kill();
                transform.localScale = Vector3.one * 0.8f;
                tween = transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(()=>{
                    tween = null;
                });
            }
        }
    }


    public void OnClick(){
        //UI.Game.SlotStatus[slotNo] = !UI.Game.SlotStatus[slotNo];
        if(Info.Active[UI.Game.Player.Active[slotNo].infoKey].mobility){
            UI.Game.Player.UseActive(slotNo,true);
        }
        else{
            on = !on;
        }
    }

}