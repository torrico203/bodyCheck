//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

public class DungeonPurchasePopup : MonoBehaviour
{
    [SerializeField]
    private Popup popup;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private TextMeshProUGUI priceText,nameText,explainText;

    [SerializeField]
    private TagListUI tagList;

    [SerializeField]
    private Button purchaseButton;
    public Button PurchaseButton{
        get => purchaseButton;
    }

    public int price;

    public DungeonShopStuff stuff;

    private ArtifactInfo artifactInfoInfo;
    public ArtifactInfo ArtifactInfoInfo{
        set {
            artifactInfoInfo = value;
            icon.sprite = artifactInfoInfo.icon;
            //price = artifactInfoInfo.price;
            priceText.text = "<sprite name=silver> "+price.ToString();
            nameText.text = Util.LocalStr("Name",artifactInfoInfo.name);

            Stat stat = Util.DeepCopy<Stat>(artifactInfoInfo.stat);

            if(UI.Game.Manager!=null && !artifactInfoInfo.addition){
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
            tagList.Clear();
            tagList.Add("Artifact");

        }
    }

    public void Purchase(){
        if(Data.UseWealth(Wealth.Type.silver,price)){
            
            UI.Game.Manager.AddArtifact(artifactInfoInfo.name);

            stuff.Purchase = true;
            Effect.Play("SilverEffect", stuff.transform.position);

            DamageText dmgtxt = Pool.GetObject<DamageText>("DamageText");
            dmgtxt.transform.position = stuff.transform.position;
            dmgtxt.text = "<sprite name=silver>-"+Mathf.FloorToInt(price);
            dmgtxt.floor = 0;
            dmgtxt.color = Color.white;
            dmgtxt.scale = 0.25f;
            dmgtxt.startY = 0f;
            dmgtxt.InitObject();

            Close();
        }
    }
    


    public void Open(){

        //Effect.Play("Power_Up_v29",titleTransform.position,UI.I.transform);

    
    }

    public void Close(){
        popup.Close(()=>{
            gameObject.SetActive(false);
        });
    }

}
