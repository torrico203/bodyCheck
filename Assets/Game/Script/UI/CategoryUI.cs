using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class CategoryUI : MonoBehaviour
{
    [SerializeField]
    private int idx = -1; // -1: All
    public int Idx{
        get{
            return idx;
        }
        set{
            idx = value;
        }
    }

    [SerializeField]
    private UnityEvent onClick;

    [SerializeField]
    private Transform buttonParent;

    private List<Button> buttons = new List<Button>();

    public void Set(int index){
        idx = index;
        onClick.Invoke();
        ButtonSet();
    }

    void ButtonSet(){
        for(int i = 0; i < buttons.Count; i++){
            if(buttons[i].gameObject.name == idx.ToString()){
                //if(focusing!=1f) buttons[i].transform.localScale = Vector3.one * focusing;
                buttons[i].Disable = true;
            }else{
                //if(focusing!=1f) buttons[i].transform.localScale = Vector3.one;
                buttons[i].Disable = false;
            }
        }
    }

    void Awake(){
        foreach(Transform child in buttonParent){
            Button btn = child.GetComponent<Button>();
            if(btn != null){
                buttons.Add(btn);
            }
        }
    }

    void Start(){
        ButtonSet();
    }

    void Update(){
        
    }

    

}