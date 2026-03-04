using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EquipmentButton : MonoBehaviour
{   
    public EquipmentData data;
    public int idx = -1;

    [SerializeField]
    private GameObject equipChecker;
    public GameObject EquipChecker{
        get { return equipChecker; }
    }

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    private EquipmentIcon equipmentIcon;

    [SerializeField]
    private RectTransform rt, rtIcon;

    [SerializeField]
    private Image bg,border;

    [SerializeField]
    private bool forEquip = false;

    [SerializeField]
    private bool isSynthesis = false;
    public bool IsSynthesis{
        get { return isSynthesis; }
        set { isSynthesis = value; }
    }

    [SerializeField]
    private ParticleSystem particleSystem1,particleSystem2;

    public void Load(){
        //icon.sprite = Info.Equipment[data.infoKey].icon;
    
        equipmentIcon.SetIcon(Info.Equipment[data.infoKey].images,Info.Equipment[data.infoKey].flipY);

        isSynthesis = false;

        bg.color = Info.GradeColor[(int)data.grade];
        border.color = Info.GradeColor[(int)data.grade];
        if((int)data.grade > 2){
            particleSystem1.gameObject.SetActive(true);
            particleSystem1.startColor = Info.GradeColor[(int)data.grade];
        }
        else{
            particleSystem1.gameObject.SetActive(false);
        }
        if((int)data.grade > 5){
            particleSystem2.gameObject.SetActive(true);
            particleSystem2.startColor = Info.GradeColor[(int)data.grade];
        }
        else{
            particleSystem2.gameObject.SetActive(false);
        }
        

        levelText.text = "Lv "+data.level.ToString();


        //Info.GradeColor[(int)Info.Equipment[data.infoKey].grade]
        // RectTransform rt = GetComponent<RectTransform>();
        // RectTransform rtIcon = equipmentIcon.GetComponent<RectTransform>();
    }

    public void OnClick(){
        if(isSynthesis){
            UI.Main.Equip.Synth.AddOffering(idx);
        }
        else{
            EquipmentPopup popup = UI.OpenPopup<EquipmentPopup>("Equipment");
            popup.data = data;
            popup.idx = idx;
            popup.Load();
        }
        
    }

    // protected override void OnInit(){
    //     transform.localScale = Vector3.one;
    //     Load();
    // }
    // protected override void OnReturn(){
        
    // }

    public void SetEquipChecker(){
        if(!forEquip && !isSynthesis){
            if(idx != -1){
                if(Data.Player.equip[(int)Info.Equipment[data.infoKey].type] == idx)
                    equipChecker.SetActive(true);
                else
                    equipChecker.SetActive(false);
            }
        }
    }

    void Update(){
        SetEquipChecker();
        

        //if(!forEquip){
            equipmentIcon.transform.localScale = Vector3.one * (rt.rect.width/rtIcon.rect.width);
            particleSystem1.transform.localScale = Vector3.one * (rt.rect.width/rtIcon.rect.width);
            particleSystem2.transform.localScale = Vector3.one * (rt.rect.width/rtIcon.rect.width);
        //}
            
    }
    
        
}