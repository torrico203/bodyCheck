using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StagePointIcon : MonoBehaviour
{

    [SerializeField]
    private Image icon;

    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private Color[] colors;

    private StagePoint.Type _type;
    public StagePoint.Type type{
        get{
            return _type;
        }
        set{
            _type = value;
            icon.sprite = sprites[(int)_type];
            icon.SetNativeSize();
            icon.color = colors[(int)_type];
        }
    }

    public Color color{
        set => icon.color = value;
    }
    

}