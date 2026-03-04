using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TagListUI : MonoBehaviour
{
    private List<TagUI> tags = new List<TagUI>();

    public void Clear(){
        foreach(var tag in tags){
            tag.ReturnObject();
        }
        tags.Clear();
    }

    public void Add(string tagName){
        TagUI tag = Pool.GetObject<TagUI>("Tag");
        tag.transform.SetParent(this.transform);
        tag.transform.localPosition = Vector3.zero;
        tag.transform.localScale = Vector3.one;
        tag.Tag = tagName;
        tags.Add(tag);
    }

}
