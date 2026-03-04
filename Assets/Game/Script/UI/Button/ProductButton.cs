using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.SocialPlatforms;

public class ProductButton : MonoBehaviour
{   
    [SerializeField]
    private Stuff[] products;

    [SerializeField]
    private StuffButton[] buttons;

    [SerializeField]
    private string productId = "";

    [SerializeField]
    private PriceLabel priceLabel;

    void Awake(){
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].Stuff = products[i];
            buttons[i].InitObject();
        }
    }


    public void OnClick(){
        if(productId != ""){
            IAP.Purchase(productId,()=>{
                Give();
            });
        }
        else{
            if(Data.UseWealth(priceLabel.Type,priceLabel.Price)){
                Give();
            }
        }
        
       
    }

    public void Give(){
        List<Stuff> rewards = new List<Stuff>(products);
        RewardPopup popup = UI.OpenPopup<RewardPopup>("RewardPopup");
        popup.rewards = rewards;
        popup.Title = Util.LocalStr("UI","Acquisition");
        
        
        popup.Show();

        Data.AddStuffs(rewards);
    }

}