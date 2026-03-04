//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class EquipmentPopup : MonoBehaviour
{
    [SerializeField]
    private Popup popup;
    public EquipmentData data;
    public int idx = -1;

    [SerializeField]
    private RectTransform body;

    [SerializeField]
    private Image bg, border;

    [SerializeField]
    private GameObject statRowPrefab,skillRowPrefab;

    [SerializeField]
    private Transform statParent,activeParent;

    private List<StatRow> statRows = new List<StatRow>();
    private List<SkillExplainUI> skillRows = new List<SkillExplainUI>();


    [SerializeField]
    private EquipmentIcon equipmentIcon;

    [SerializeField]
    private TextMeshProUGUI nameText,quantityRateText;

    [SerializeField]
    private Button equipButton,unequipButton;

    [SerializeField]
    private GameObject buttonParent;


    [SerializeField]
    private TagListUI tagList;

    [SerializeField]
    private QualityGauge qualityGauge;

    [SerializeField]
    private PriceLabel levelupPrice,qualityPrice;

    [SerializeField]
    private Image light;


    public void Open(){
        //Debug.Log(data.infoKey);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(statParent.GetComponent<RectTransform>());
    }

    public void Load(){
        //icon.sprite = Info.Equipment[data.infoKey].icon;
        string name = Util.LocalStr("Name",data.infoKey);
        name = Util.LocalStr("Name",data.grade.ToString(),new[]{new{name=name}});
        nameText.text = "Lv. "+data.level+"\n"+name;

        equipmentIcon.SetIcon(Info.Equipment[data.infoKey].images,Info.Equipment[data.infoKey].flipY);
        bg.color = Info.GradeColor[(int)data.grade];
        border.color = Info.GradeColor[(int)data.grade];

        //태깅
        //gradeTag.Tag = data.grade.ToString();
        tagList.Clear();
        tagList.Add(Info.Equipment[data.infoKey].type.ToString());
        if(Info.Equipment[data.infoKey].weaponType != WeaponType.None)
            tagList.Add(Info.Equipment[data.infoKey].weaponType.ToString());
        // equipmentTag.Tag = Info.Equipment[data.infoKey].type.ToString();
        // if(Info.Equipment[data.infoKey].weaponType != WeaponType.None){
        //     weaponTypeTag.gameObject.SetActive(true);
        //     weaponTypeTag.Tag = Info.Equipment[data.infoKey].weaponType.ToString();
        // }
        // else
        //     weaponTypeTag.gameObject.SetActive(false);


        //품질
        qualityGauge.Set(0,100,Mathf.FloorToInt(data.quality*100));

        //레벨업 가격
        levelupPrice.Price = data.level*10;

        //품질업 확률
        quantityRateText.text = Util.LocalStr("UI","QualityUpRate",new[]{new{rate=Mathf.Floor(Util.QualityChance((int)(data.quality*100f))*10000f)/100f}});


        Stat stat = new Stat();

        if(idx != -1)
            stat = Data.EquipmentStat(idx);
        else
            stat = Data.EquipmentStat(data);

        Stat compare = Data.EquipmentStat(Data.Player.equip[(int)Info.Equipment[data.infoKey].type]);
        

        int i=0;
        for(;i<stat.Count();i++){
            if(stat[i]==0f && compare[i]==0f) {
                if(i < statRows.Count)
                    statRows[i].gameObject.SetActive(false);
                continue;
            }
            StatRow row = null;
            if(i < statRows.Count){
                row = statRows[i];
                row.gameObject.SetActive(true);
            }
            else{
                GameObject obj = Instantiate(statRowPrefab, statParent);
                row = obj.GetComponent<StatRow>();
                statRows.Add(row);
            }
            row.StatType = (Stat.Type)i;
            //row.SetType = (StatRow.Type)1;
            row.Stat = stat;
            row.Compare = compare;
        }

        for(;i<statRows.Count;i++){
            statRows[i].gameObject.SetActive(false);
        }

        for(i=0;i<Info.Equipment[data.infoKey].active.Length;i++){
            SkillExplainUI row = null;
            if(i < skillRows.Count){
                row = skillRows[i];
                row.gameObject.SetActive(true);
            }
            else{
                GameObject obj = Instantiate(skillRowPrefab, activeParent);
                row = obj.GetComponent<SkillExplainUI>();
                skillRows.Add(row);
            }
            row.Set(Info.Equipment[data.infoKey].active[i]);
        }
        for(;i<skillRows.Count;i++){
            skillRows[i].gameObject.SetActive(false);
        }

        if(idx == -1){
            buttonParent.SetActive(false);
        }
        else{
            buttonParent.SetActive(true);
            if(Data.Player.equip[(int)Info.Equipment[data.infoKey].type]== idx){
                unequipButton.gameObject.SetActive(true);
                equipButton.gameObject.SetActive(false);
            }
            else{
                unequipButton.gameObject.SetActive(false);
                equipButton.gameObject.SetActive(true);
            }
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(body);

    }

    public void Close(){
        popup.Close(()=>{
            
            gameObject.SetActive(false);
        });
    }
    public void Unequip(){
        //Data.Player.equip[(int)Info.Equipment[data.infoKey].type] = -1;
        
        Data.Unequip(Info.Equipment[data.infoKey].type);
        UI.Main.Equip.PlayerEquipRefresh();
        Close();
    }
    public void Equip(){
        // if(idx != -1){
        //     Data.Player.equip[(int)Info.Equipment[data.infoKey].type] = idx;
        // }
        Data.Equip(idx);
        UI.Main.Equip.PlayerEquipRefresh();
        Close();
    }

    public void Levelup(){

        if(Data.UseWealth(levelupPrice.Type,levelupPrice.Price)){
            Data.EquipmentLevelup(idx);
            Effect.Play("Power_Burst_V8",equipmentIcon.transform.position,UI.I.transform);
            light.DOFade(1,0.05f).SetEase(Ease.OutSine).OnComplete(() => {
                light.DOFade(0,0.1f).SetEase(Ease.InSine);
                Load();
                UI.Main.Equip.Load();
            });
            
        }
    }

    public void QualityUp(){
        if(Data.UseWealth(qualityPrice.Type,qualityPrice.Price)){
            
            if(Data.EquipmentQualityUp(idx)){
                Effect.Play("Power_Burst_V8",equipmentIcon.transform.position,UI.I.transform);
                light.DOFade(1,0.05f).SetEase(Ease.OutSine).OnComplete(() => {
                    light.DOFade(0,0.1f).SetEase(Ease.InSine);
                    Load();
                    UI.Main.Equip.Load();
                });

                UI.SmallNotice("SuccessQualityUp");
            }
            else{
                Effect.Play("Debris_GravityUp_V3",equipmentIcon.transform.position,UI.I.transform);
                UI.SmallNotice("FailQualityUp");
            }
            
        }
    }
}
