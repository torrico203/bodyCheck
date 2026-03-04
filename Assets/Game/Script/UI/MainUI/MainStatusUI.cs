using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainStatusUI : MainMenuUI
{
    
    [SerializeField]
    private StatAdder[] statAdders;

    [SerializeField]
    private TextMeshProUGUI levelText;

    private List<StatRow> statRows = new List<StatRow>();

    [SerializeField]
    private Transform statParent;

    void Awake(){
        foreach(Transform rowTransform in statParent){
            StatRow row = rowTransform.GetComponent<StatRow>();
            if(row != null)
                statRows.Add(row);
        }
    }
    void OnEnable(){
        Reload();
    }

    public override void Load(){
        Reload();
    }

    void Update(){
        
    }

    public void Reload(){
        bool isLevelup = true;

        for(int i=0; i<statAdders.Length; i++){
            if(!statAdders[i].IsMax){
                isLevelup = false;
                break;
            }
        }

        if(isLevelup) {
            Data.LevelUp();
            Effect.Play("Power_Up_v29",levelText.transform.position,UI.I.transform);


            StartCoroutine(LevelUpReward());
        }
        
        levelText.text = "Level " + Data.Player.level.ToString();

        for(int i=0; i<statRows.Count; i++){
            statRows[i].Stat = Data.ResultStat();
            statRows[i].Compare = Data.Player.stat;
        }
    }

    IEnumerator LevelUpReward(){
        yield return new WaitForSeconds(0.6f);
        List<Stuff> rewards = new List<Stuff>();
        Wealth wealth = new Wealth();
        wealth.dia = 100;
        Stuff stuff = new Stuff(wealth);
        rewards.Add(stuff);

        RewardPopup popup = UI.OpenPopup<RewardPopup>("RewardPopup");
        popup.rewards = rewards;
        popup.Title = Util.LocalStr("UI","Levelup");
        popup.Show();
        Data.AddStuffs(rewards);
    }
}
