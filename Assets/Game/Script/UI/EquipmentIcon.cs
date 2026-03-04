using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EquipmentIcon : MonoBehaviour
{

    private Dictionary<string, Image> icons = new Dictionary<string, Image>();

    
    
    void Awake()
    {
        foreach (Transform child in transform)
        {
            Image icon = child.GetComponent<Image>();
            if (icon != null)
            {
                icons.Add(child.name, icon);
            }
        }
    }

    public void SetIcon(EquipmentInfo.EquipSprite[] sprites,bool flipY = false)
    {
        foreach(var icon in icons)
        {
            icon.Value.sprite = null;
            icon.Value.gameObject.SetActive(false);
        }

        foreach (var sprite in sprites)
        {
            if (icons.ContainsKey(sprite.key))
            {
                if(sprite.sprite != null)
                {
                    icons[sprite.key].gameObject.SetActive(true);
                    icons[sprite.key].sprite = sprite.sprite;
                    icons[sprite.key].SetNativeSize();
                    if(flipY) icons[sprite.key].transform.localRotation = Quaternion.Euler(0f,0f,180f);
                    else icons[sprite.key].transform.localRotation = Quaternion.Euler(0f,0f,0f);
                }
            }
        }
    }
    

}