//BOMIN
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class WealthViewer : MonoBehaviour
{

    [SerializeField]
    private Wealth.Type wealthType;

    [SerializeField]
    private TextMeshProUGUI icon,valueText;

    [SerializeField]
    private string valueFront = "";

    private Sequence seq;

    private int value = -1;

    void Awake(){
        //icon.sprite = Info.WealthIcon[(int)wealthType];
        if(icon != null)
            icon.text = "<sprite name="+wealthType.ToString()+">";
    }

    void Update(){
        if(this.value != Data.Player.wealth[(int)wealthType])
        {
            float diff = Mathf.Abs(Data.Player.wealth[(int)wealthType] - this.value);
            diff = Mathf.Clamp(diff/1000f,0.1f,3f);

            if(seq != null) seq.Kill();

            transform.localScale = Vector3.one * 0.9f;
            seq = DOTween.Sequence();
            seq.Append(valueText.DOCounter(this.value,Data.Player.wealth[(int)wealthType],diff));
            seq.Join(transform.DOScale(Vector3.one,0.3f).SetEase(Ease.OutBack));
            seq.AppendCallback(()=>{
                valueText.text = valueFront + valueText.text;
                seq = null;
            });
            this.value = Data.Player.wealth[(int)wealthType];
        }
        
    }

    
}
