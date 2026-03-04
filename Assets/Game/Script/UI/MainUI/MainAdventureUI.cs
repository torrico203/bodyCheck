using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainAdventureUI : MainMenuUI
{
    private GameObject map;

    [SerializeField]
    private TextMeshProUGUI stageName;

    void OnEnable(){
        if(!UI.Loaded) return;
        
        StageData stage = Data.Player.dungeon[Data.Player.dungeon.Count-1];
        if(stage.clear>0){
            if(Info.Dungeon[stage.infoKey].nextStage != null){
                Data.AddStageData(Info.Dungeon[stage.infoKey].nextStage.name);
                Data.DungeonName = Info.Dungeon[stage.infoKey].nextStage.name;
            }
        }
        InitStage();

    }

    public void InitStage(){
        Data.DungeonName = Data.Player.dungeon[Data.Player.dungeon.Count-1].infoKey;
        SetStage();
    }

    void SetStage(){
        if(map != null)
            Destroy(map);
        
        string stage = Data.DungeonName;
        
        
        map = Instantiate(Info.Dungeon[stage].maps[Info.Dungeon[stage].maps.Length-1]);

        stageName.text = (Info.Dungeon[stage].no+1)+". "+Util.LocalStr("Name",stage);
        

        

        //Debug.Log("info light : "+Info.Dungeon[stage].globalLightIntensity);
        //Debug.Log("main? : "+UI.Main.gameObject.name);

        //UI.Main.Light.intensity = Info.Dungeon[stage].globalLightIntensity;
        //UI.Main.Light.color = Info.Dungeon[stage].globalLightColor;

    }

    public void SelectStage(){
        DungeonSelectPopup popup = UI.OpenPopup<DungeonSelectPopup>("DungeonSelect");
        popup.callback = ()=>{
            SetStage();
        };
    }
}
