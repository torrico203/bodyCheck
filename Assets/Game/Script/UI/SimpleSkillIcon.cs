using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Dependencies.NCalc;


public class SimpleSkillIcon : MonoBehaviour
{
    [SerializeField]
    private Image icon,border;

    [SerializeField]
    private TextMeshProUGUI levelText;

    public int level{
        set{
            levelText.text = $"lv.{value.ToString()}";
        }
    }

    public SkillType type;

    private string infoKey;
    public string InfoKey{
        set{
            infoKey = value;
            if(type == SkillType.Active){
                icon.sprite = Info.Active[infoKey].icon;
                border.color = Info.GradeColor[(int)Info.Active[infoKey].grade];
            }
            else if(type == SkillType.Passive){
                icon.sprite = Info.Passive[infoKey].icon;
                border.color = Info.GradeColor[(int)Info.Passive[infoKey].grade];
            }
        }
    }



}