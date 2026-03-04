//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class DungeonSelectPopup : MonoBehaviour
{
    [SerializeField]
    private Popup popup;

    [SerializeField]
    private GameObject dungeonIconPrefab;

    [SerializeField]
    private Transform dungeonIconParent;

    [SerializeField]
    private Button leftButton,rightButton;

    public Action callback = null;

    private int currentIndex = 0;

    [SerializeField]
    private List<DungeonIcon> icons = new List<DungeonIcon>();

    private Tween tween = null;


    void Start(){
        if(!UI.Loaded) return;

        //Info.Dungeon.Count;
        //Debug.Log("Dungeon Info Count : "+Info.Dungeon.Count);
        //icons = new List<DungeonIcon>(Info.Dungeon.Count);

        
        // ReloadCurrentIndex();
        // Reload();
    }

    void OnEnable(){
        //Reload();
        if(icons.Count<=0){
            for(int i=0;i<Info.Dungeon.Count;i++)
            icons.Add(null);

            foreach(var dungeon in Info.Dungeon){
                GameObject row = Instantiate(dungeonIconPrefab,dungeonIconParent);
                DungeonIcon button = row.GetComponent<DungeonIcon>();
                //button.popup = this;
                button.InfoKey = dungeon.Value.name;
                row.transform.SetSiblingIndex(dungeon.Value.no);
                icons[dungeon.Value.no] = button;
            }
        }
        ReloadCurrentIndex();
        Reload();
    }

    public void OnLeftButton(){
        currentIndex--;
        Reload();
    }

    public void OnRightButton(){
        currentIndex++;
        Reload();
    }

    void ReloadCurrentIndex(){
        for(int i = 0; i < icons.Count; i++)
        {
            if(icons[i] != null && icons[i].InfoKey == Data.DungeonName)
            {
                if(i != currentIndex)
                    dungeonIconParent.DOLocalMoveX(i*-900f,0f);
                
                currentIndex = i;
                break;
            }
        }
    }

    void Reload(){
        

        if(currentIndex <= 0)
            leftButton.Disable = true;
        else
            leftButton.Disable = false;
        
        if(currentIndex >= Info.Dungeon.Count-1)
            rightButton.Disable = true;
        else
            rightButton.Disable = false;
        
        if(tween != null) tween.Kill();
        tween = dungeonIconParent.DOLocalMoveX(currentIndex*-900f,0.6f).SetEase(Ease.OutBack).OnComplete(()=>{
            tween = null;
        });
        

        if (Data.Player.dungeon.Exists(x => x.infoKey == icons[currentIndex].InfoKey)){
            Data.DungeonName = icons[currentIndex].InfoKey;
            if(callback != null) callback();
        }
        

        
    }

    public void Open(){
        Debug.Log("StageSelectPopup Open");

        

    }
    public void Close(){
        popup.Close(()=>{
            
            gameObject.SetActive(false);
        });
    }


}
