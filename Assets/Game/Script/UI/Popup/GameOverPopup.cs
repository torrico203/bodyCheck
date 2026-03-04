//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine.Localization.Components;

public class GameOverPopup : MonoBehaviour
{


    [SerializeField]
    private Transform defeatTitleTransform,clearTitleTransform;


    [SerializeField]
    private TextMeshProUGUI strTime,strKill,strLevel,strSilver;

    

    [SerializeField]
    private RectTransform body;

    [SerializeField]
    private CanvasGroup touch;

    [HideInInspector]
    public List<Stuff> rewards = new List<Stuff>();

    [SerializeField]
    private Transform rewardParent,skillParent;

    private List<StuffButton> rewardList = new List<StuffButton>();

    private bool on = false;

    [HideInInspector]
    public bool clear = false;


    void OnEnable(){
        if(!UI.Loaded) return;

        

        
    
        //LayoutRebuilder.ForceRebuildLayoutImmediate(body);
        
    }

    public void Setup()
    {
        
        for(int i=0;i<rewardList.Count;i++){
            rewardList[i].ReturnObject();
        }
        rewardList.Clear();

        // skillParent의 모든 자식 오브젝트 삭제
        foreach(Transform child in skillParent){
            Destroy(child.gameObject);
        }

        

        
        // strTime.StringReference.Arguments = new[]{new{value="99m"}};
        // strTime.StringReference.SetReference("UI","resultTime");
        // strTime.RefreshString();

        float gameTime = Mathf.FloorToInt(Time.time - UI.Game.GameStartTime);
        int minutes = (int)(gameTime / 60);
        int seconds = (int)(gameTime % 60);
        string timeStr = $"{seconds}s";
        if(minutes>0) timeStr = $"{minutes}m {timeStr}";

        strTime.text = Util.LocalStr("UI","resultTime",new[]{new{value=timeStr}});

        strKill.text = Util.LocalStr("UI","resultKill",new[]{new{value=UI.Game.Manager.KillCount}});

        strLevel.text = Util.LocalStr("UI","resultLevel",new[]{new{value=UI.Game.Manager.Level}});

        //strSilver.text = Util.LocalStr("UI","strSilver",new[]{new{value=UI.Game.Manager.Level}});

        
        for(int i=0;i<UI.Game.Player.Active.Count;i++){
            ActiveData data = UI.Game.Player.Active[i];

            Assets.CreateAsset("Assets/Game/Prefab/UI/SimpleSkillIcon.prefab", (obj) => {
                SimpleSkillIcon icon = obj.GetComponent<SimpleSkillIcon>();
                icon.type = SkillType.Active;
                icon.level = data.level;
                icon.InfoKey = data.infoKey;
            },skillParent);
        }

        for(int i=0;i<UI.Game.Player.Passive.Count;i++){
            PassiveData data = UI.Game.Player.Passive[i];

            Assets.CreateAsset("Assets/Game/Prefab/UI/SimpleSkillIcon.prefab", (obj) => {
                SimpleSkillIcon icon = obj.GetComponent<SimpleSkillIcon>();
                icon.type = SkillType.Passive;
                icon.level = data.level;
                icon.InfoKey = data.infoKey;
            },skillParent);
        }

        
        


        
        
        on = false;

        Sequence seq = DOTween.Sequence();

        body.anchoredPosition = new Vector2(0f, -600f);

        Transform titleTransform = defeatTitleTransform;
        if(clear) {
            titleTransform = clearTitleTransform;
            Data.StageClear(Data.DungeonName);
        }
        else{
            Sound.PlaySFX("Defeat");
        }

        titleTransform.gameObject.SetActive(true);
        titleTransform.localScale = new Vector3(0, 1, 1);
        touch.alpha = 0;

        seq.Append(body.DOAnchorPosY(0, 0.5f).SetEase(Ease.OutBack));
        seq.Append(titleTransform.DOScale(1, 0.5f).SetEase(Ease.OutBack).OnComplete(()=>{
            SetReward();
            if(clear) Effect.Play("Power_Burst_V9",titleTransform.position,UI.I.transform);
        }));
        seq.Append(touch.DOFade(1, 0.5f));

        seq.OnComplete(()=>{
            on = true;
        });
    }

    void SetReward(){
        for(int i=0;i<rewards.Count;i++){
            
            StuffButton button = Pool.GetObject<StuffButton>("StuffButton");
            button.transform.SetParent(rewardParent);
            button.transform.localScale = Vector3.one;
            button.Stuff = rewards[i];
            button.InitObject();
            button.Show();
            rewardList.Add(button);
            
        }
    }

    public void OnClick(){
        if(!on) return;
        foreach(Transform child in rewardParent){
            StuffButton button = child.GetComponent<StuffButton>();
            if(button != null){
                button.ReturnObject();
            }
        }

        defeatTitleTransform.gameObject.SetActive(false);
        clearTitleTransform.gameObject.SetActive(false);

        gameObject.SetActive(false);
        UI.Game.GoMain();
    }


}
