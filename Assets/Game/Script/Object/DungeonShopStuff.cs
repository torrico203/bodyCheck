using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using TMPro;

public class DungeonShopStuff : MonoBehaviour
{
    private ArtifactInfo selectedArtifact;

    [SerializeField]
    private SpriteRenderer icon;
    [SerializeField]
    private TextMeshPro priceText;

    private int price = 0;

    private DungeonPurchasePopup popup = null;

    private bool purchase = false;
    public bool Purchase{
        set{
            icon.gameObject.SetActive(false);
            priceText.gameObject.SetActive(false);
            purchase = value;
        }
    }
    
    void Awake()
    {
        
    }

    void Start()
    {
        
    }

    void Update()
    {
    }

    void OnEnable()
    {
        var artifacts = Info.Artifact.Values.ToArray();
        selectedArtifact = artifacts[Random.Range(0, artifacts.Length)];
        //Debug.Log($"Selected Artifact: {selectedArtifact.name}");
        icon.sprite = selectedArtifact.icon;
        price = Random.Range(selectedArtifact.priceMin/10, selectedArtifact.priceMax/10+1)*10;
        priceText.text = "<sprite name=silver> "+price;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!purchase){
            if(other.gameObject == UI.Game.Player.gameObject)
            {
                //Debug.Log("Player Shop In");
                DungeonPurchasePopup popup = UI.OpenPopup<DungeonPurchasePopup>("DungeonPurchase");
                popup.price = price;
                popup.ArtifactInfoInfo = selectedArtifact;
                popup.stuff = this;
                this.popup = popup;
                popup.PurchaseButton.gameObject.SetActive(true);
            }
        }
        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(!purchase){
            if(other.gameObject == UI.Game.Player.gameObject)
            {
                if(popup!=null){
                    if(popup.gameObject.activeSelf)
                    {
                        popup.Close();
                    }
                    popup = null;
                }
            }
        }
    }
}
