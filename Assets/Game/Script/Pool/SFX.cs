//BOMIN
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Audio;

public class SFX : MonoBehaviour{

    [SerializeField]
    private List<AudioSource> list;

    [SerializeField]
    private List<AudioClip> clip;
    public List<AudioClip> Clip { get => clip; set => clip = value; }

    [SerializeField]
    private int count=1;

    [SerializeField]
    private bool random = false;

    [SerializeField]
    private AudioMixerGroup mixer;
    

    public void Initialize(){
        for(int i=0;i<count;i++)
        {
            for(int j=0;j<clip.Count;j++)
            {
                GameObject SFX = new GameObject(typeof(SFX).ToString());
                SFX.transform.SetParent(this.transform);
                AudioSource source = SFX.AddComponent<AudioSource>();

                source.clip = clip[j];
                source.outputAudioMixerGroup = mixer;
                this.list.Add(source);
            }
        }
    }

    public void Play(float fitch = 1f){
        int seed = 0;
        if(random) seed = Random.Range(0,list.Count);
        for(int i=0;i<list.Count;i++){
            int x = (i + seed)%list.Count;
            if(!list[x].isPlaying){
                list[x].pitch = fitch;
                list[x].Play();
                break;
            }
        }
    }   
}