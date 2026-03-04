//BOMIN
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class Effect
{
    public static EffectObject Play(string name, Vector3 pos, Transform parent = null, float fitch = 1f){
        EffectObject effect = Pool.GetObject<EffectObject>(name);
        effect.transform.SetParent(parent);
        //pos.z = -10;
        effect.Fitch = fitch;
        // if(effect.RT != null)
        //     effect.RT.anchoredPosition = pos;
        // else
        //     effect.transform.position = pos;
        effect.transform.position = pos;
        effect.transform.localScale = Vector3.one;
        //effect.gameObject.SetActive(true);
        effect.InitObject();
        return effect;
    }

    // public static void Wealth(WealthType type, Vector3 targetPos, Action callback = null){
    //     WealthEffect effect = Pool.GetObject<WealthEffect>("WealthEffect");
    //     effect.transform.SetParent(UI.I.transform);
    //     effect.transform.position = targetPos;
    //     effect.TargetPos = UI.Game.WealthViewers[(int)type].transform.position;
    //     effect.WealthType = type;
    //     effect.callback = callback;
    //     //effect.gameObject.SetActive(true);
    //     effect.InitObject();
    // }
}