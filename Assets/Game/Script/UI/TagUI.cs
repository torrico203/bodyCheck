using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TagUI : PoolingObject
{
    [SerializeField]
    private string tag;
    public string Tag { 
        get => tag; 
        set{
            tag = value;
            Setup();
        }
    }

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private Image background;

    void Setup(){
        if(tag == null || tag == "") return;
        if(Info.TagColor.ContainsKey(tag)){
            background.color = Info.TagColor[tag];
            text.text = Util.LocalStr("Tag",tag);
        }
    }

    void Awake(){
        Setup();
    }

    protected override void OnInit(){
        
    }
    protected override void OnReturn(){
        
    }


}
