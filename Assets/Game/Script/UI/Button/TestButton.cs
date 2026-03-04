using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TestButton : MonoBehaviour
{   
    
    public void OnClick(string name){
        if(name == "effect"){
            Effect.Play("Power_Up_v29",Vector3.zero,UI.I.transform);

        }
    }


    
}