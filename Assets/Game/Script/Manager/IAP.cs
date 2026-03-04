//BOMIN
using UnityEngine;
using System;
using TMPro;

public class IAP : MonoBehaviour
{
    public static IAP I { get; private set; }

    [SerializeField]
    private TextMeshProUGUI productText;

    private Action callback = null;

    public static void Purchase(string productId, Action callback){
        I.productText.text = productId;
        I.callback = callback;
        I.gameObject.SetActive(true);
    }

    public void PurchaseComplete(){
        if(callback != null){
            callback();
            gameObject.SetActive(false);
            callback = null;
        }
    }

    public void Quit()
    {
        gameObject.SetActive(false);
    }

    void Awake(){
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
    }
}
