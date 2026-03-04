//BOMIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Audio;
using DG.Tweening;

public class Sound : MonoBehaviour
{
    public static Sound I { get; private set; }

    public AudioSource musicSource;
    public AudioClip[] musicClips;

    [SerializeField]
    private Transform SFXParent,BGMParent;

    [SerializeField]
    private AudioMixer mixer;

    private Dictionary<string, SFX> SFXlist = new Dictionary<string, SFX>();
    private Dictionary<string, AudioSource> BGMlist = new Dictionary<string, AudioSource>();

    [SerializeField]
    private string nowBGM = "",nextBGM = "";

    private bool loaded = false;
    public static bool Loaded { get => I.loaded; }

    private Sequence s = null;

 
    void Awake()
    {
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start(){
        mixer.SetFloat("Master",PlayerPrefs.GetFloat("MasterVolume",0f));
        mixer.SetFloat("BGM",PlayerPrefs.GetFloat("BGMVolume",0f));
        mixer.SetFloat("SFX",PlayerPrefs.GetFloat("SFXVolume",0f));
    }

    void Update(){
        if(nextBGM != ""){
            if(s == null){
                if(this.BGMlist.ContainsKey(nextBGM)){
                    s = DOTween.Sequence();
                    if(this.nowBGM != ""){
                        s.Append(this.BGMlist[this.nowBGM].DOFade(0,1f).OnComplete(()=>{
                            this.BGMlist[this.nowBGM].Stop();
                            this.BGMlist[nextBGM].Play();
                        }));
                    }
                    else{
                        this.BGMlist[nextBGM].Play();
                    }
                    
                    s.Append(this.BGMlist[nextBGM].DOFade(1,1f));

                    s.OnComplete(()=>{
                        this.nowBGM = nextBGM;
                        this.nextBGM = "";
                        s = null;
                    });
                }
            }
            
        }
    }

    public static void Load(){   
        // Initialize dictionaries

        for (int i = 0; i < I.SFXParent.childCount; i++)
        {
            SFX pool = I.SFXParent.GetChild(i).GetComponent<SFX>();
            if(pool == null) continue;
            I.SFXlist.Add(pool.gameObject.name, pool);
        }

        for(int i=0;i<I.BGMParent.childCount;i++){
            AudioSource source = I.BGMParent.GetChild(i).GetComponent<AudioSource>();
            if(source == null) continue;
            I.BGMlist.Add(source.gameObject.name,source);
        }

        I.StartCoroutine(I.LoadCoroutine());
    }

    IEnumerator LoadCoroutine(){
        AsyncOperationHandle<IList<AudioClip>> handle =  Addressables.LoadAssetsAsync<AudioClip>("SFX", clip =>
        {
            //Gets called for every loaded asset
            string key = clip.name.Split('_')[0];
            if(I.SFXlist.ContainsKey(key)){
                Debug.Log("SFX : "+clip.name);
                I.SFXlist[key].Clip.Add(clip);
            }

        });
        yield return handle;
        foreach(var row in I.SFXlist) row.Value.Initialize();
        Debug.Log("SFX Loaded");
        //Addressables.Release(handle);
        
        handle = Addressables.LoadAssetsAsync<AudioClip>("BGM", clip =>
        {
            //Gets called for every loaded asset
            if(I.BGMlist.ContainsKey(clip.name)){
                Debug.Log("BGM : "+clip.name);
                I.BGMlist[clip.name].clip = clip;
            }
        });
        
        //Addressables.Release(handle);

        I.loaded = true;
    }

    public static void SetVolume(Type type,float volume){
        I.mixer.SetFloat(type.ToString(),volume);
        PlayerPrefs.SetFloat(type+"Volume",volume);
    }
    public static float GetVolume(Type type){
        float volume = PlayerPrefs.GetFloat(type.ToString()+"Volume",0f);
        return volume;
    }

    public static void PlayBGM(string name)
    {
        I.nextBGM = name;
    }

    public static void PlaySFX(string name,float fitch = 1f)
    {
        if(I.SFXlist.ContainsKey(name))
            I.SFXlist[name].Play(fitch);
    }


    public enum Type{
        Master = 0,
        BGM,
        SFX
    }
}