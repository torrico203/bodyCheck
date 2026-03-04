using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using MoreMountains.Feedbacks;

public class LevelupSelectButton : MonoBehaviour
{   

    public SkillType type;

    private string infoKey;
    public string InfoKey{
        get{return infoKey;}
        set{infoKey = value;}
    }

    private string delKey = "";
    public string DelKey{
        get{return delKey;}
        set{delKey = value;}
    }

    private int level = 1;
    public int Level{
        get{return level;}
        set{level = value;}
    }

    private LevelupPopup popup;
    public LevelupPopup Popup{
        get{return popup;}
        set{popup = value;}
    }

    [SerializeField]
    private GameObject aura;
    public GameObject Aura{
        get{return aura;}
    }

    [SerializeField]
    private TagListUI tagList;

    [SerializeField]
    private SkillExplainUI explainUI;

    [SerializeField]
    private StarLevel starLevel;

    [SerializeField]
    private Transform relatedActiveParent;

    private List<GameObject> relatedActives = new List<GameObject>();
    [SerializeField]
    private GameObject relatedActive;
    [SerializeField]
    private TextMeshProUGUI relatedText;

    [SerializeField]
    private RectTransform explainTransform;

    void SetStarLevel(int level, int maxLevel){
        if(starLevel != null){
            if(maxLevel == 0) starLevel.gameObject.SetActive(false);
            else starLevel.gameObject.SetActive(true);
            starLevel.MaxLevel = maxLevel;
            starLevel.Level = level;
        }
    }
    public void Load(){

        explainUI.InfoKey = infoKey;
        explainUI.Level = level;
        explainUI.type = type;
        explainUI.Setup();

        tagList.Clear();

        tagList.Add(type.ToString());

        string weaponType = "Public";
        int maxLevel = 0;

        //관련 액티브 스킬 초기화
        for(int i=0;i<relatedActives.Count;i++)
            Destroy(relatedActives[i]);
        relatedActives.Clear();
        //foreach(Transform child in relatedActiveParent)
        //   Destroy(child.gameObject);
        
        if(type == SkillType.Active){
            //액티브 스킬인 경우
            if(Info.Active[infoKey].weaponType != WeaponType.None)
                weaponType = Info.Active[infoKey].weaponType.ToString();
            maxLevel = Info.Active[infoKey].maxLevel;

            relatedActive.SetActive(false);
            Vector2 offset = explainTransform.offsetMin;
            offset.y = 10f;
            explainTransform.offsetMin = offset;
        }
        else if(type == SkillType.Passive){
            //패시브 스킬인 경우


            if(Info.Passive[infoKey].weaponType != WeaponType.None)
                weaponType = Info.Passive[infoKey].weaponType.ToString();
            maxLevel = Info.Passive[infoKey].maxLevel;

            if(Info.Passive[infoKey].relatedActive.Length > 0){
                relatedActive.SetActive(true);
                relatedText.text = Util.LocalStr("UI","relateActive");
                Vector2 offset = explainTransform.offsetMin;
                offset.y = 100f;
                explainTransform.offsetMin = offset;

                for(int x = 0;x<Info.Passive[infoKey].relatedActive.Length;x++){
                    string relateInfoKey = Info.Passive[infoKey].relatedActive[x].name;
                    Assets.CreateAsset("Assets/Game/Prefab/UI/ActiveSimple.prefab", (activeSimple) => {
                        activeSimple.GetComponent<ActiveSimple>().InfoKey = relateInfoKey;
                        relatedActives.Add(activeSimple);
                    },relatedActiveParent);
                }
            }
            else {
                relatedActive.SetActive(false);
                Vector2 offset = explainTransform.offsetMin;
                offset.y = 10f;
                explainTransform.offsetMin = offset;
            }
        }
        else if(type == SkillType.Reward){
            //보상인 경우
            maxLevel = 0;

            relatedActive.SetActive(false);
            Vector2 offset = explainTransform.offsetMin;
            offset.y = 10f;
            explainTransform.offsetMin = offset;
        }

        if(maxLevel > 0) SetStarLevel(level, maxLevel);
        else SetStarLevel(0, 0);

    
        tagList.Add(weaponType);


        if(type == SkillType.Active){
            //액티브 스킬인 경우

            //레벨업인 경우
            if(level > 1) tagList.Add("LevelUp");
            
        

            if(delKey != ""){
                //승급인 경우
                tagList.Add("Upgrade");
                aura.SetActive(true);

                relatedActive.SetActive(true);
                relatedText.text = Util.LocalStr("UI","UpgradeActive");

                string relateInfoKey = delKey;
                Assets.CreateAsset("Assets/Game/Prefab/UI/ActiveSimple.prefab", (activeSimple) => {
                    activeSimple.GetComponent<ActiveSimple>().InfoKey = relateInfoKey;
                    relatedActives.Add(activeSimple);
                },relatedActiveParent);
                
            }
            else{
                //승급이 아닌 경우
                aura.SetActive(false);
            }

            if(Info.Active[infoKey].moving){
                //이동 스킬인 경우
                tagList.Add("Moving");
            }
        }
        else if(type == SkillType.Passive){
            //패시브 스킬인 경우

            //레벨업인경우
            if(level > 1) tagList.Add("LevelUp");
            

            aura.SetActive(false);
        }
        else if(type == SkillType.Reward){
            //보상인 경우
            aura.SetActive(false);
        }

        
    }

    public void OnClick(){
        if(popup.selected) return;
        popup.selected = true;
        
        if(type == SkillType.Active){
            if(delKey != ""){
                UI.Game.DelActive(delKey);
            }
            UI.Game.Manager.Player.AddActive(infoKey);
        }
        else if(type == SkillType.Passive){
            if(Info.Passive[infoKey].addition)
                UI.Game.Manager.Player.VStat += (Info.Passive[infoKey].stat);
            else{
                Stat value = (UI.Game.Manager.Player.NStat*(Info.Passive[infoKey].stat));
                for(int i=0;i<value.Count();i++)
                {
                    if(value[i]!=0){
                        
                        if(Info.IntStat[i]!=0) {
                            value[i] = Mathf.Round(value[i]);
                            if(value[i]<1) value[i] = 1;
                        }
                    }
                }
                UI.Game.Manager.Player.VStat += value;
            }
                
            UI.Game.Manager.AddPassive(infoKey);
        }
        else if(type == SkillType.Reward){
            List<Stuff> stuff = new List<Stuff>();
            foreach(Stuff s in Info.Reward[infoKey].stuff){
                stuff.Add(s);
            }
            Data.AddStuffs(stuff);
        }

        popup.Close();
    }

        
}