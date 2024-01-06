using MyARPG.Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthInfoBox : MonoBehaviour
{
    private Image HpImage;
    private Image StrengthImage;
    private Text HpText;
    private Text StrengthText;

    private CharacterHealthBase info;

    private void Awake()
    {
        HpImage = transform.Find("Hp/Image").GetComponent<Image>();
        StrengthImage = transform.Find("Strength/Image").GetComponent<Image>();
        HpText = transform.Find("Hp/Text").GetComponent<Text>();
        StrengthText = transform.Find("Strength/Text").GetComponent<Text>();
    }

    private void Start()
    {
        GameEventManager.Instance.AddEventListening<CharacterHealthBase>("UpdateHealthInfo", _UpdateHealthInfo);
    }

    private void OnDestroy()
    {
        GameEventManager.Instance.RemoveEvent<CharacterHealthBase>("UpdateHealthInfo", _UpdateHealthInfo);
    }

    /// <summary>
    /// 设置当前ui显示的对象
    /// </summary>
    public void SetHealthInfo(CharacterHealthBase info)
    {
        this.info = info;
        _UpdateHealthInfo(info);
    }

    /// <summary>
    /// 事件驱动更新
    /// </summary>
    public void _UpdateHealthInfo(CharacterHealthBase info)
    {
        if (this.info != info) return;

        //update image
        HpImage.fillAmount = info.CharacterHealthData.CurrentHP / info.CharacterHealthData.MaxHP;
        StrengthImage.fillAmount = info.CharacterHealthData.CurrentStrength / info.CharacterHealthData.MaxStrength;
        //update text
        HpText.text = string.Format("{0}/{1}", info.CharacterHealthData.CurrentHP, info.CharacterHealthData.MaxHP);
        StrengthText.text = string.Format("{0}/{1}", info.CharacterHealthData.CurrentStrength, info.CharacterHealthData.MaxStrength);

    }


}
