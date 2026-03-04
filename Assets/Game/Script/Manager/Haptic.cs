//BOMIN
using UnityEngine;
using Lofelt.NiceVibrations;

public static class Haptic
{
    public static void Play(HapticPatterns.PresetType presetType){
        if(presetType != HapticPatterns.PresetType.None)
            HapticPatterns.PlayPreset(presetType);
    }

}