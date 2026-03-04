using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Loader : MonoBehaviour
{
    private static Loader i;
    public static Loader I
    {
        get
        {
            // if (i == null)
            // {
            //     i = FindObjectOfType<Loader>();
            //     if (i == null)
            //     {
            //         GameObject singleton = new GameObject(typeof(Loader).ToString());
            //         i = singleton.AddComponent<Loader>();
            //         DontDestroyOnLoad(singleton);
            //     }
            // }

            Debug.Log("Call Loader");
            return i;
        }
    }
    
    [SerializeField]
    private CanvasGroup canvasGroup;

    private string loadSceneName;
    
    [SerializeField]
    private UIGauge loadGauge;

    

    [SerializeField]
    private Sprite[] loadIllustration;

    [SerializeField]
    private Image loadImage;
    // Start is called before the first frame update


    private void Awake()
    {
        if(i == null)
        {
            Debug.Log("Loader Awake");
            DontDestroyOnLoad(gameObject);
            i = this;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }
    
    public static void LoadScene(string sceneName, Action callback = null)
    {
        

        I.gameObject.SetActive(true);
        SceneManager.sceneLoaded += I.LoadSceneEnd;
        I.loadSceneName = sceneName;

        I.loadGauge.Init();
        I.LoadSceneStart(sceneName, callback);

        //일러스트 
        I.loadImage.sprite = I.loadIllustration[UnityEngine.Random.Range(0, I.loadIllustration.Length)];
    }

    void LoadSceneStart(string name, Action callback = null){
        
        Debug.Log("Scene Load1 : " + name);
        loadGauge.Set(0, 1000, 0);
        Debug.Log("Scene Load2 : " + name);
        
        loadImage.DOFade(0f,0f);
        canvasGroup.DOFade(1f, 0.25f).OnComplete(()=>{
            loadImage.DOFade(1f, 0.25f).OnComplete(()=>{
                Debug.Log("Scene Load3 : " + name);
                StartCoroutine(SceneLoad(name));
                if(callback != null) callback();
            });
        });
        //StartCoroutine(Load(name));
    }

    IEnumerator SceneLoad(string name)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(name); 
        op.allowSceneActivation = false;  
        float timer = 0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f)
            {
                long progress = Mathf.CeilToInt(op.progress * 1000);
                loadGauge.Set(0, 1000, progress);
            }
            else
            {
                long progress = Mathf.CeilToInt(op.progress * 1000);
                loadGauge.Set(0, 1000, Mathf.CeilToInt(Mathf.Lerp(progress, 1000, (long)(timer*1000))));
                
                if (loadGauge.isFull)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
        
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.name == loadSceneName)
        {
            UI.SetScene(scene.name);
            Debug.Log("Scene Load End : " + scene.name);
            canvasGroup.DOFade(0f, 0.5f).OnComplete(()=>{
                gameObject.SetActive(false);
            });
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }
    
}