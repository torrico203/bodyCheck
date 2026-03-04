//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

public class OptionPopup : MonoBehaviour
{
    [SerializeField]
    private Popup popup;

    [SerializeField]
    private Slider[] soundSliders;

    [SerializeField]
    private ToggleButton[] muteButtons;

    [SerializeField]
    private ToggleButton mobilityRightToggle;


    [SerializeField]
    private GameObject giveupObject;

    public void Start()
    {
        foreach (Sound.Type type in Enum.GetValues(typeof(Sound.Type)))
        {
            float vol = Sound.GetVolume(type);
            soundSliders[(int)type].value = vol;
            if(vol == -80f) muteButtons[(int)type].IsOn = true;
            else muteButtons[(int)type].IsOn = false;
        }


    }

   

    public void Open(){

        //Effect.Play("Power_Up_v29",titleTransform.position,UI.I.transform);
        //Sound.GetVolume(Sound.Type.Master);

        giveupObject.SetActive(UI.Game.gameObject.activeSelf);
        
        int mr = PlayerPrefs.GetInt("MobilityRight",0);
        if(mr == 1) mobilityRightToggle.IsOn = true;
    }

    public void MobilityRight(){
        if(mobilityRightToggle.IsOn)
            PlayerPrefs.SetInt("MobilityRight",1);
        else
            PlayerPrefs.SetInt("MobilityRight",0);
        
        if(UI.Game.gameObject.activeSelf){
            //인게임일때는 직접 변경
            UI.Game.RepositionMobilitySlots();
        }
    }

    public void DungeonGiveUp(){
        UI.Confirm(()=>{
            Close();
            //Loader.LoadScene("Main");
            Deal deal = new Deal();
            deal.absolute = UI.Game.Player.hp;
            UI.Game.Player.Hit(deal,null,0,true);
        },"GiveupDungeonContent","GiveupDungeon",()=>{

        });
    }

    public void SetVolume(string type){
        Sound.Type soundType = (Sound.Type)Enum.Parse(typeof(Sound.Type), type);
        float vol = soundSliders[(int)soundType].value;
        Sound.SetVolume(soundType,vol);
        if(vol==-80f)
            muteButtons[(int)soundType].IsOn = true;
        else
            muteButtons[(int)soundType].IsOn = false;
    }

    public void SetMute(string type){
        int soundType = (int)Enum.Parse(typeof(Sound.Type), type);
        if(muteButtons[soundType].IsOn)
            soundSliders[soundType].value = -80f;
        else
            soundSliders[soundType].value = 0f;
    
    }

    public void Close(){
        popup.Close(()=>{
            gameObject.SetActive(false);
        });
    }


}
