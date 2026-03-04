using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class StageSelectPopupButton : MonoBehaviour
{   

    public void OnClick(){
        UI.OpenPopupSimple("StageSelect");
        
    }

}