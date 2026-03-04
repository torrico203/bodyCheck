//BOMIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lofelt.NiceVibrations;

public class EffectObject : PoolingObject
{
    [SerializeField]
    protected float duration = 1.0f;

    protected float fitch = 1f;
    public float Fitch{ get => fitch; set => fitch = value; }

    [SerializeField]
    protected string sound = "";

    [SerializeField]
    HapticPatterns.PresetType haptic = HapticPatterns.PresetType.None;


    protected override void OnInit(){
        if(sound != ""){
            Sound.PlaySFX(sound,fitch);
        }
        StartCoroutine(End());
        
        
        Haptic.Play(haptic);
    }

    protected IEnumerator End(){
        yield return new WaitForSeconds(duration);
        //ReturnObject();
        //gameObject.SetActive(false);
        ReturnObject();
    }

    public void Stop(){
        StopAllCoroutines();
        ReturnObject();
    }
    protected override void OnReturn(){
        transform.localScale = Vector3.one;
        fitch = 1f;
    }
}
