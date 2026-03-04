using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Initialize : MonoBehaviour
{
    [SerializeField]
    private bool loaded = false;

    private bool UILoaded = false;

    private bool preLoaded = false;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        //LoadManager.Instance.LoadScene("TitleScene");

        //프리로드 체크 및 프리로드
        StartCoroutine(PreLoad());
    }

    IEnumerator PreLoad(){
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync("Preload");
        yield return getDownloadSize;

        Debug.Log("Download Size : "+getDownloadSize.Result);

        if(getDownloadSize.Result > 0){
            AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync("Preload", false);
            float progress = 0;

            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
                if (percentageComplete > progress * 1.1) // Report at most every 10% or so
                {
                    progress = percentageComplete; // More accurate %
                    Debug.Log("Preload : "+progress);
                    //UI.PreloadText.text = "Preload : "+progress;
                }
                yield return null;
            }
            //UI.PreloadText.text = "Preload Complete";
            Debug.Log("Preload Complete");
            Load();
            //Addressables.Release(downloadHandle); //Release the operation 
        }
        else{
            //UI.PreloadText.text = "Preload Already Complete";
            Debug.Log("Preload Already Complete");
            Load();
        }
    }

    void Load(){
        preLoaded = true;
        Pool.Load();
        Info.Load();
        Sound.Load();
    }

    // Update is called once per frame
    void Update()
    {
        if(preLoaded){
            if(Info.Loaded && Pool.Loaded && !UILoaded){
                UILoaded = true;
                
                Debug.Log("UI Load Start");
                UI.Load();
            }
            else if(Info.Loaded && Pool.Loaded && UI.Loaded){
                if(!loaded){
                    loaded = true;
                    Debug.Log("Game Load Start");
                    Loader.LoadScene("Title");
                }
            }
        }
        
    }
}
