using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class ActiveSimple : MonoBehaviour
{   
    

    [SerializeField]
    private Image icon;
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private Image border;

    private string infoKey;
    public string InfoKey{
        get{return infoKey;}
        set{
            infoKey = value;
            icon.sprite = Info.Active[infoKey].icon;
            //nameText.text = Util.LocalStr("Name",Info.Active[infoKey].name);
            Util.SetLocalizedText(nameText, "Name", Info.Active[infoKey].name);
            border.color = Info.GradeColor[(int)Info.Active[infoKey].grade];
        }
    }


}