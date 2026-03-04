using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WealthButton : MonoBehaviour
{   

    private Wealth.Type type;
    public Wealth.Type Type{
        get{
            return type;
        }
        set{
            type = value;
            WealthIcon.text = "<sprite name="+type.ToString()+">";
        }
    }
    private int val = 0;
    public int Val{
        get{
            return val;
        }
        set{
            val = value;
            valText.text = val.ToString("N0");
        }
    }

    [SerializeField]
    private TextMeshProUGUI valText,WealthIcon;


    
    public void OnClick(){
        // EquipmentPopup popup = UI.OpenPopup<EquipmentPopup>("Equipment");
        // popup.data = data;
        // popup.idx = idx;
        // popup.Load();
    }

    // protected override void OnInit(){
    //     transform.localScale = Vector3.one;
    // }
    // protected override void OnReturn(){
        
    // }

    
        
}