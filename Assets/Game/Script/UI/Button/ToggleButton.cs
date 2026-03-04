//BOMIN
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class ToggleButton : Button
{
    [SerializeField]
    private Sprite toggleImage, unToggleImage;

    [SerializeField]
    private bool isOn = false;
    public bool IsOn{
        get{return isOn;}
        set{
            isOn = value;
            SetToggle();
        }
    }

    void Start(){
        SetToggle();
    }

    void SetToggle(){
        if(isOn){
            image.sprite = toggleImage;
        }else{
            image.sprite = unToggleImage;
        }
    }

    
    public override void Execute(){
        if(disable) return;
        if(sound != ""){
            //Sound.PlaySFX(sound);
        }
        IsOn = !isOn;
        if(action != null) action.Invoke();
    }
}
