using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SynthesisUI : MainMenuUI
{

    [SerializeField]
    private List<int> offeringList = new List<int>();

    [SerializeField]
    private EquipmentButton[] offeringButton;

    [SerializeField]
    private Transform bottomLine,top,smith;

    private Vector3 bottomLinePos = Vector3.zero, topPos, smithPos;

    [SerializeField]
    private Button doButton;

    [SerializeField]
    private Image curtain;

    private Sequence seq;
    private bool inSynthesis = false;

    [SerializeField]
    private GameObject synthesisEffect;

    public void OnEnable(){

        if(bottomLinePos == Vector3.zero) {
            bottomLinePos = bottomLine.localPosition;
            topPos = top.localPosition;
            smithPos = smith.localPosition;
        }
        bottomLine.localPosition = new Vector3(bottomLinePos.x+500f, bottomLinePos.y, bottomLinePos.z);
        bottomLine.DOLocalMoveX(bottomLinePos.x, 0.3f).SetEase(Ease.OutExpo);

        top.localPosition = new Vector3(topPos.x-500f, topPos.y, topPos.z);
        top.DOLocalMoveX(topPos.x, 0.3f).SetEase(Ease.OutExpo);
        
        smith.localPosition = new Vector3(smithPos.x+300f, smithPos.y, smithPos.z);
        smith.DOLocalMoveX(smithPos.x, 0.3f).SetDelay(0.1f).SetEase(Ease.OutBack);

        UI.Main.Equip.Synthesis(true);
        offeringList.Clear();
        UI.Main.Equip.OrderOffering();
        SetOffering();
    }

    public void OnDisable(){
        UI.Main.Equip.Synthesis(false);
        UI.Main.Equip.Order();
    }

    public void AddOffering(int idx){
        if(offeringList.Contains(idx)) {
            RemoveOffering(idx);
            UI.Main.Equip.Buttons[idx].Equipment.EquipChecker.gameObject.SetActive(false);
        }
        else if(offeringList.Count < offeringButton.Length) {
            offeringList.Add(idx);
            UI.Main.Equip.Buttons[idx].Equipment.EquipChecker.gameObject.SetActive(true);
            
        }

        if(offeringList.Count>0){
           
            UI.Main.Equip.OrderOffering((int) Data.Player.inventory[offeringList[0]].grade);
        }
        else{
            UI.Main.Equip.OrderOffering();
        }
        SetOffering();
    }
    void RemoveOffering(int idx){
        if(offeringList.Count > 0) {
            offeringList.Remove(idx);
        }
    }

    public void SetOffering(){
        for(int i=0;i<offeringButton.Length;i++){
            offeringButton[i].gameObject.SetActive(false);
        }
        for(int i=0;i<offeringList.Count;i++){
            offeringButton[i].gameObject.SetActive(true);
            offeringButton[i].data = Data.Player.inventory[offeringList[i]];
            offeringButton[i].idx = offeringList[i];
            offeringButton[i].Load();
            offeringButton[i].IsSynthesis = true;
        }

        if(offeringList.Count == offeringButton.Length){
            doButton.Disable = false;
        }
        else{
            doButton.Disable = true;
        }
    }

    public void DoSynthesis(){
        if(offeringList.Count < 2) return;
        if(inSynthesis) return;
        inSynthesis = true;
        synthesisEffect.SetActive(true);
        curtain.gameObject.SetActive(true);
        curtain.color = new Color(0,0,0,0);
        
        
        Vector3 pos0 = offeringButton[0].transform.position;
        Vector3 pos2 = offeringButton[2].transform.position;
        seq = DOTween.Sequence();
        seq.Append(offeringButton[0].transform.DOMoveX(offeringButton[1].transform.position.x, 1.5f).SetEase(Ease.OutQuart));
        seq.Join(offeringButton[2].transform.DOMoveX(offeringButton[1].transform.position.x, 1.5f).SetEase(Ease.OutQuart));
        seq.Join(curtain.DOFade(1, 0.5f));
        seq.OnComplete(() => {
            offeringButton[0].transform.position = pos0;
            offeringButton[2].transform.position = pos2;
            inSynthesis = false;
            synthesisEffect.SetActive(false);
            curtain.gameObject.SetActive(false);

            List<EquipmentData> resultList = new List<EquipmentData>();
            resultList.Add(Data.DoSynthesis(offeringList.ToArray()));
            Result(resultList);

            for(int i=0;i<Data.Player.inventory.Count;i++){
                if(Data.Player.inventory[i].flag == 1){
                    Data.Player.inventory[i].flag = 2;
                }
            }

        });
    }

    public void DoAllSynthesis(){

        List<EquipmentData> resultList = new List<EquipmentData>();
        
        for(int i=0;i<(int)Grade.Legend;i++){
            List<int> list = new List<int>();
            bool isSynthesis = false;
            do{
                isSynthesis = false;
                list.Clear();
                for(int j=0;j<Data.Player.inventory.Count;j++){
                    if(Data.Player.inventory[j].grade == (Grade)i){
                        bool isEquip = false;
                        for(int k=0;k<Data.Player.equip.Count();k++){
                            if(Data.Player.equip[k]==j)
                                isEquip = true;
                        }
                        if(!isEquip) 
                        {
                            list.Add(j);
                            if(list.Count>=3) {
                                isSynthesis = true;
                                break;
                            }
                        }
                    }
                }
                if(isSynthesis) 
                    Data.DoSynthesis(list.ToArray());
                    
            }while(isSynthesis);
        }
        for(int i=0;i<Data.Player.inventory.Count;i++){
            if(Data.Player.inventory[i].flag == 1){
                resultList.Add(Data.Player.inventory[i]);
                Data.Player.inventory[i].flag = 2;
            }
        }
        if(resultList.Count>0)
            Result(resultList);
        else
            UI.SmallNotice("NoSynthesis");

    }



    void Result(List<EquipmentData> data){
        SynthesisPopup popup = UI.OpenPopup<SynthesisPopup>("SynthesisResult");
        popup.rewards = new List<Stuff>();
        
        for(int i=0;i<data.Count;i++)
            popup.rewards.Add(new Stuff(data[i]));
            

        popup.Show();
        
        offeringList.Clear();
        UI.Main.Equip.Load();
        UI.Main.Equip.OrderOffering();
        SetOffering();
        UI.Main.Equip.Synthesis(true);
    }
}

