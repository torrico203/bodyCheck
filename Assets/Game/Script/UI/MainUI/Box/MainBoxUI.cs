using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainBoxUI : MainMenuUI
{

    [SerializeField]
    private BoxUI box;

    [SerializeField]
    private CategoryUI category;


    [SerializeField]
    private Image curtain;

    [SerializeField]
    private TextMeshProUGUI priceText;
    private int price = 0;

    [SerializeField]
    private Rate[] rate = new Rate[5];

    [SerializeField]
    private TextMeshProUGUI explainText;

    private bool unBoxing = false;


    public override void Load(){
        SetBox();
    }
    public void SetBox(){
        box.Idx = category.Idx;

        explainText.text = Util.LocalStr("UI","box"+box.Idx+"Explain",new[]{new{grade0=Info.GradeNameColor[0],grade1=Info.GradeNameColor[1],grade2=Info.GradeNameColor[2],grade3=Info.GradeNameColor[3],grade4=Info.GradeNameColor[4],grade5=Info.GradeNameColor[5]}});
    }

    void Update(){

        if(!unBoxing){
            int _price = 0;
            if(Data.Player.wealth[(5+box.Idx)]>=10){
                _price = 10;
            }
            else{
                _price = Data.Player.wealth[(5+box.Idx)];
            }

            if(price != _price){
                price = _price;
                priceText.text = "x"+price.ToString();
            }
        }
        

    }


    public void OpenBox(){

        if(price <=0){
            UI.SmallNotice("ne_box"+box.Idx);
            return;
        }

        if(Data.UseWealth((Wealth.Type)5+box.Idx, price)){
            unBoxing = true;
            curtain.gameObject.SetActive(true);
            Color color = curtain.color;
            color.a = 0;
            curtain.color = color;
            Sound.PlaySFX("BoxOpen");
            curtain.DOFade(1, 0.5f).OnComplete(() => {
                
                //curtain.gameObject.SetActive(false);
            });
            box.Open(true,false,() => {
                Debug.Log("Box Opened");
                //curtain.gameObject.SetActive(false);
                BoxRewardPopup popup = UI.OpenPopup<BoxRewardPopup>("BoxRewards");
                popup.rewards = new List<Stuff>();

                popup.Box.Idx = box.Idx;

                // List<string> keys = new List<string>();
                // foreach(var i in Info.Equipment.Keys){
                //     if(Info.Equipment[i].inUse)
                //         keys.Add(i);
                // }

                //rate[box.Idx].rate
                float totalRate = 0f;
                for(int i=0;i<rate[box.Idx].rate.Length;i++){
                    totalRate += rate[box.Idx].rate[i].rate;
                }

                for(int i=0;i<price;i++){
                    EquipmentData equipment = new EquipmentData();

                    float _totalRate = totalRate;
                    Grade grade = Grade.Common;
                    float rateValue = Random.Range(0f,_totalRate);
                    for(int j=0;j<rate[box.Idx].rate.Length;j++){
                        if(rateValue < rate[box.Idx].rate[j].rate){
                            grade = rate[box.Idx].rate[j].grade;
                            break;
                        }
                        _totalRate -= rate[box.Idx].rate[j].rate;
                        rateValue -= rate[box.Idx].rate[j].rate;
                    }

                    equipment.infoKey = Info.RandomEquipment();
                    equipment.grade = grade;
                    equipment.level = 1;
                    equipment.quality = Util.GenerateQuality();
                    //Debug.Log("Box Reward : "+equipment.infoKey+" Grade : "+equipment.grade.ToString());
                    popup.rewards.Add(new Stuff(equipment));
                }

                Data.AddStuffs(popup.rewards);

                popup.Show();

                popup.callback = () => {
                    Debug.Log("Box Reward Popup Closed");
                    curtain.DOFade(0, 0.5f).OnComplete(() => {
                        curtain.gameObject.SetActive(false);
                    });
                    box.Close();
                };

                unBoxing = false;

            });
        }
        else{

            //UI.SmallNotice("NotEnoughBox"+box.Idx);
        }
    }
}
