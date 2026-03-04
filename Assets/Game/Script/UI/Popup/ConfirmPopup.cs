//BOMIN
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

public class ConfirmPopup : MonoBehaviour
{
    [SerializeField]
    private Popup popup;

    public TextMeshProUGUI title,content;
    public Button confirmButton,cancelButton;

    [System.Serializable]
    public class ActionDic : CustomDic.SerializableDictionary<string, Action>{}
    [SerializeField]
    public ActionDic actions;

    public void Callback(string type){
        if(actions.ContainsKey(type))
            actions[type]?.Invoke();
        Close();
    }
    public void Open(){

        //Effect.Play("Power_Up_v29",titleTransform.position,UI.I.transform);

    
    }

    public void Close(){
        popup.Close(()=>{
            gameObject.SetActive(false);
        });
    }

}
