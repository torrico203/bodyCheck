//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class LevelupPopup : MonoBehaviour
{
    [SerializeField]
    private Popup popup;

    [SerializeField]
    private LevelupSelectButton[] buttons;

    [SerializeField]
    private Transform levelupTitle,skillBoxTitle;

    private List<ActiveInfo> activeList = new List<ActiveInfo>();

    private List<PassiveInfo> passiveList = new List<PassiveInfo>();

    [SerializeField]
    private List<string> rewardList = new List<string>();

    [SerializeField]
    private RectTransform listTransform,bodyTransform;

    public Action callback = null;

    [HideInInspector]
    public bool selected = false;
    
    
    private bool isSkillBox = false;
    public bool IsSkillBox{
        set{
            isSkillBox = value;
            levelupTitle.gameObject.SetActive(!isSkillBox);
            skillBoxTitle.gameObject.SetActive(isSkillBox);
            SetButtons();
        }
    }
    


    public void OnEnable(){
        if(!UI.Loaded) {
            gameObject.SetActive(false);
            return;
        }
        if(!Info.Loaded) {
            gameObject.SetActive(false);
            return;
        }

        selected = false;

        Time.timeScale = 0;

        
        
    }

    void SetButtons(){
        SetInfo();
        
        for(int i = 0; i < buttons.Length; i++){
            buttons[i].gameObject.SetActive(true);
            float rate = UnityEngine.Random.Range(0f,1f);
            bool related = false;
            if(rate < 0.33f || isSkillBox) related = true;

            rate = UnityEngine.Random.Range(0f,1f);
            if(rate < 0.5f){
                SetActiveButton(i,false,related);
            }
            else{
                SetPassiveButton(i,false,related);
            }

            buttons[i].transform.SetSiblingIndex(UnityEngine.Random.Range(0,100));

            
            Debug.Log("Load : "+buttons[i].InfoKey);
            Debug.Log("Type : "+buttons[i].type);

            buttons[i].Load();
            buttons[i].Popup = this;
        }

        RefreshVerticalLayoutGroup();
    }

    void SetInfo(){
        

        activeList.Clear();

        foreach(var active in Info.Active){
            //Debug.Log(active.Key);
            if(active.Value.user == Actor.Type.Player && (active.Value.weaponType == WeaponType.None || active.Value.weaponType == UI.Game.Manager.Player.WeaponType || active.Value.weaponType == UI.Game.Manager.Player.ShieldType)) //타입 검사
            {
                if(active.Value.level <= Data.Player.level)
                {
                    if(active.Value.forLevelup)
                        activeList.Add(active.Value);
                }
            }
        }

        
        passiveList.Clear();
         
        foreach(var passive in Info.Passive){
            if(passive.Value.user == Actor.Type.Player && (passive.Value.weaponType == WeaponType.None || passive.Value.weaponType == UI.Game.Manager.Player.WeaponType || passive.Value.weaponType == UI.Game.Manager.Player.ShieldType)) //타입 검사
            {
                if(passive.Value.level <= Data.Player.level)
                    passiveList.Add(passive.Value);
            }
        }
    }
    void SetRewardButton(int i){
        buttons[i].type = SkillType.Reward;
        buttons[i].DelKey = "";
        buttons[i].InfoKey = rewardList[UnityEngine.Random.Range(0,rewardList.Count)];
        buttons[i].Level = 1;

    }
    void SetPassiveButton(int i, bool isReward = false, bool relate = false){
        buttons[i].type = SkillType.Passive;
        buttons[i].DelKey = "";
        bool confirm = true;
        string infoKey = "";
        int tryCount = 0;
        int level = 1;
        do{
            confirm = true;
            infoKey = passiveList[UnityEngine.Random.Range(0,passiveList.Count)].name;
            level = UI.Game.Manager.GetPassiveLevel(infoKey) + 1;
            

            if(Info.Passive[infoKey].maxLevel > 0 && level > Info.Passive[infoKey].maxLevel){
                confirm = false;
            }
            
            if(level==1){//레벨이 1인 경우(최초 획득)
                if(UI.Game.GetRemainPassiveCount()<=0){
                    confirm = false;
                }
            }

            if(relate)
            {
                if(Info.Passive[infoKey].relatedActive.Length>0){
                    
                    ResultActive data = GetResultActive(Info.Passive[infoKey].relatedActive[0].name, 1);
                    if(data == null) confirm = false;
                    else{
                        if(data.beforeKey == "" && data.level == 1) //승급이 아니면서 레벨이 1인 경우 (즉, 새거)
                        {
                            confirm = false;
                        }
                    }
                    
                }
                else{
                    confirm = false;
                }
            }

            if(tryCount > 100){
                if(!relate){
                    if(isReward) SetRewardButton(i);
                    else SetActiveButton(i,true);
                }
                else{
                    SetPassiveButton(i,false,false);
                }
                return;
            }
            
            tryCount++;
        }while(!confirm);

        buttons[i].InfoKey = infoKey;
        buttons[i].Level = level;


        //중복 안뜨게 제거
        passiveList.RemoveAll(x => x.name == infoKey);
    }

    void SetActiveButton(int i, bool isReward = false, bool relate = false){
        buttons[i].type = SkillType.Active;
        bool confirm = true;
        string infoKey = "";
        int tryCount = 0;
        int level = 1;
        do{
            
            buttons[i].DelKey = "";
            confirm = true;
            infoKey = activeList[UnityEngine.Random.Range(0,activeList.Count)].name;
            

            level = 1;
            

            ResultActive data = GetResultActive(infoKey, level);
            if(data == null) confirm = false;
            else{
                if(data.level==1)//새스킬인데
                {
                    if(data.infoKey != infoKey){ //승급 대상인 경우
                        buttons[i].DelKey = data.beforeKey;
                    }
                }
                
                infoKey = data.infoKey;
                level = data.level;
            }
            

            if(buttons[i].DelKey == ""){    //승급이 아님에도도
                if(level == 1){     //새스킬이라면
                    if(!relate){
                        if(UI.Game.GetRemainActiveCount()<=0)//자리체크
                            confirm = false;
                    }
                    else{ //관계스킬일때는 그냥 실패
                            confirm = false;
                    }
                    
                }
            }
            
            //같은 스킬이 이미 나와있는 경우
            for(int x = 0;x<i;x++){
                if(buttons[x].InfoKey == infoKey){
                    confirm = false;
                    break;
                }
            }

            if(relate){
                if(level != 1){//승급된 스킬의 레벨업 금지
                    if(!Info.Active[infoKey].forLevelup) confirm = false;
                }
            }

            if(tryCount > 100){ //시도횟수가 100번 넘을 경우 넘긴다.
                if(!relate){
                    if(isReward) SetRewardButton(i);
                    else SetPassiveButton(i,true);
                }
                else{
                    //관계스킬 가져오기 실패했을 때는 그냥 새스킬 허용
                    SetActiveButton(i,false,false);
                }
                return;
            }
            tryCount++;
        }while(!confirm);
        
        buttons[i].InfoKey = infoKey;
        buttons[i].Level = level;
        
        // activeList에서 infoKey에 해당하는 ActiveInfo를 제거
        activeList.RemoveAll(x => x.name == infoKey);
    }

    [System.Serializable]
    public class ResultActive
    {
        public string infoKey;
        public int level;
        public string beforeKey;
    }

    //최종 액티브 스킬을 결정하는 함수
    public ResultActive GetResultActive(string infoKey, int level, string beforeKey = ""){
        
        ResultActive data = new ResultActive();
        data.infoKey = infoKey;
        data.level = level;
        data.beforeKey = beforeKey;

        //플레이어 스킬 레벨 체크
        for(int x = 0;x<UI.Game.Manager.Player.Active.Count;x++){
            if(UI.Game.Manager.Player.Active[x].infoKey == infoKey){
                data.level = UI.Game.Manager.Player.Active[x].level+1;
                break;
            }
        }

        //승급 레벨 체크
        if(Info.Active[infoKey].maxLevel>0){    
            if(data.level > Info.Active[infoKey].maxLevel)
            {//만렙이라면
                if(Info.Active[infoKey].nextActive.Length == 0){//승급 가능한 스킬이 없는 경우
                    return null;
                }
                else{//승급 가능한 스킬이 있는 경우
                    ActiveInfo[] nextActive = Info.Active[infoKey].nextActive;
                    List<string> enableNextActive = new List<string>();
                    
                    for(int x=0;x<nextActive.Length;x++){
                        if(nextActive[x].relatedPassive.Length > 0){//관련 패시브 스킬이 있는 경우
                            PassiveInfo[] relatedPassive = nextActive[x].relatedPassive;
                            for(int y=0;y<relatedPassive.Length;y++){//관련 패시브 스킬 레벨이 최소 0이상(즉, 플레이어가 패시브 스킬을 가지고 있는 경우)
                                if(UI.Game.Manager.GetPassiveLevel(relatedPassive[y].name)>0){
                                    enableNextActive.Add(nextActive[x].name);
                                    break;
                                }
                            }
                        }
                        else{//관련 패시브 스킬이 없는 경우
                            enableNextActive.Add(nextActive[x].name);
                        }
                    }
                    if(enableNextActive.Count > 0){//승급 가능한 스킬이 있는 경우
                        int seed = UnityEngine.Random.Range(0,enableNextActive.Count);
                        infoKey = enableNextActive[seed];
                        data = GetResultActive(infoKey, 1, data.infoKey);
                    }
                    else{//승급 가능한 스킬이 없는 경우
                        return null;
                    }
                }                    
            }
        }    

        return data;
    }

    public void Open(){

        //Effect.Play("Power_Up_v29",,skillBoxTitle.position,UI.I.transform);

        //RefreshVerticalLayoutGroup();

    }

    public void Load(){
        //icon.sprite = Info.Equipment[data.infoKey].icon;
    }

    public void Close(){

        // for(int i = 0; i < buttons.Length; i++){
            
        //     buttons[i].Aura.SetActive(true);
        // }

        //UI.Game.JoystickOnOff(true);
        popup.Close(()=>{
            gameObject.SetActive(false);
            if(callback != null) callback();
            Time.timeScale = 1;
        });
    }

    void RefreshVerticalLayoutGroup()
    {
        var layoutGroup = listTransform.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup != null){
            LayoutRebuilder.ForceRebuildLayoutImmediate(listTransform);
        }
        layoutGroup = bodyTransform.GetComponent<VerticalLayoutGroup>();
        if(layoutGroup != null){
            LayoutRebuilder.ForceRebuildLayoutImmediate(bodyTransform);
        }
        
    }

}
