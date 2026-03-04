using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainEquipUI : MainMenuUI
{
    [SerializeField]
    private Transform listParent;

    private List<StuffButton> buttons = new List<StuffButton>();
    public List<StuffButton> Buttons{
        get{
            return buttons;
        }
    }

    [SerializeField]
    private List<EquipmentButton> equipButton = new List<EquipmentButton>();

    [SerializeField]
    private Player player;

    public CategoryUI category;

    [SerializeField]
    private GameObject equip,synthesis;

    [SerializeField]
    private SynthesisUI synth;
    public SynthesisUI Synth{
        get{
            return synth;
        }
    }


    public void Order(){
        for(int i=0;i<buttons.Count;i++){//카테고라이즈
            if(category.Idx == (int)Info.Equipment[buttons[i].Stuff.equipment.infoKey].type || category.Idx == -1){
                buttons[i].gameObject.SetActive(true);
            }
            else{
                buttons[i].gameObject.SetActive(false);
            }
            //buttons[i].transform.SetSiblingIndex(i);
        }
    }

    public void OrderOffering(int grade = -1){
        for(int i=0;i<buttons.Count;i++){
            if(buttons[i].Equipment.EquipChecker.activeSelf) {
                buttons[i].gameObject.SetActive(false);
                continue;
            }
            if(buttons[i].Equipment.data.grade == (Grade)grade || grade == -1)
                buttons[i].gameObject.SetActive(true);
            else
                buttons[i].gameObject.SetActive(false);
            
            if(buttons[i].Equipment.data.grade == Grade.Legend)
                buttons[i].gameObject.SetActive(false);
            
            
        }
    }

    public void Synthesis(bool isSynthesis){
        
        for(int i=0;i<buttons.Count;i++){
            //if(!buttons[i].Equipment.EquipChecker.activeSelf) 
            if(Data.Player.equip[(int)Info.Equipment[buttons[i].Equipment.data.infoKey].type] != buttons[i].Equipment.idx)
                buttons[i].Equipment.IsSynthesis = isSynthesis;
        }
    }

    public override void Unload(){
        
        equip.SetActive(true);
        synthesis.SetActive(false);
    }

    public override void Load(){

        int i=0;
        for(;i<Data.Player.inventory.Count;i++){
            bool isExist = false;
            if(i < buttons.Count){
                buttons[i].Stuff = new Stuff(Data.Player.inventory[i]);
                buttons[i].Equipment.idx = i;
                buttons[i].InitObject();
                isExist = true;
            }

            if(!isExist){
                StuffButton button = Pool.GetObject<StuffButton>("StuffButton");
                button.transform.SetParent(listParent);
                button.transform.localScale = Vector3.one;
                button.Stuff = new Stuff(Data.Player.inventory[i]);
                button.Equipment.idx = i;
                button.InitObject();
                buttons.Add(button);
            }
        }

        for(;i<buttons.Count;i++){
            bool isExist = false;
            if(i < Data.Player.inventory.Count){
                isExist = true;
            }

            if(!isExist){
                buttons[i].ReturnObject();
                buttons.RemoveAt(i);
                i--;
            }
        }

        PlayerEquipRefresh();
        Order();
    }


    public void PlayerEquipRefresh(){
        player.Equip = Data.Player.equip;

        for(int i=0;i<player.Equip.Count();i++){
            if(player.Equip[i] == -1) {
                if(i < equipButton.Count)
                    equipButton[i].gameObject.SetActive(false);
                continue;
            }
            if(i < equipButton.Count){
                equipButton[i].gameObject.SetActive(true);
                equipButton[i].data = Data.Player.inventory[player.Equip[i]];
                equipButton[i].idx = player.Equip[i];
                equipButton[i].Load();
                //equipButton[i].InitObject();
            }
        }
    }
}
