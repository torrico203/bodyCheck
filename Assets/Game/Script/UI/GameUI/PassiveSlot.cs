using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class PassiveSlot : MonoBehaviour
{   
    

    [SerializeField]
    private Image  icon;
    [SerializeField]
    private TextMeshProUGUI levelText;
    [SerializeField]
    private Image border;

    
    private PassiveData data;
    public PassiveData Data{
        get{return data;}
        set{
            data = value;
            if(data == null){
                icon.gameObject.SetActive(false);
                levelText.gameObject.SetActive(false);
                border.color = Color.white;
            }
            else{
                icon.gameObject.SetActive(true);
                icon.sprite = Info.Passive[data.infoKey].icon;
                levelText.gameObject.SetActive(true);
                levelText.text = "Lv."+data.level.ToString();
                border.color = Info.GradeColor[(int)Info.Passive[data.infoKey].grade];
            }
        }
    }


}