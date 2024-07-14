using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusManager : MonoBehaviour
{
    [SerializeField]
    float blinkSpeed = 0.1f;
    [SerializeField]
    int blinkCount = 10;
    int currentblinkCount = 0;
    //Hp감소 애니메이션 변수 선언
    bool isBlink=false;

    bool isDead = false;

    int maxHp = 3;
    int currentHp = 3;

    int maxShield = 3;
    int currentShield = 0;

    [SerializeField]
    Image[] hpImage = null;
    [SerializeField]
    Image[] shieldImage = null;

    [SerializeField]
    int shieldIncreaseCombo = 5;
    //5콤보가 찰때마다 부터 쉴드 증가
    int currentShieldCombo = 0;
    [SerializeField]
    Image ShieldGauge = null;

    Result result;
    NoteManager noteManager;
    [SerializeField]
    MeshRenderer playerMesh=null;

    void Start()
    {
        result=FindObjectOfType<Result>();
        noteManager = FindObjectOfType<NoteManager>();
    }

    public void Initialized()
    {//초기화
        currentHp = maxHp;
        currentShield = 0;
        currentShieldCombo = 0;
        ShieldGauge.fillAmount = 0;

        isDead= false;
        SettingHpImage();
        SettingShieldImage();
    }
    public void CheckShield()
    {
        currentShieldCombo++;

        if (currentShieldCombo >= shieldIncreaseCombo)
        {
            currentShieldCombo= 0;
            IncreaseShiled();
        }

        ShieldGauge.fillAmount =(float) currentShieldCombo / shieldIncreaseCombo;
    }
    public void ResetShieldCombo()
    {
        currentShieldCombo= 0;
        ShieldGauge.fillAmount = (float)currentShieldCombo / shieldIncreaseCombo;
    }

    public void IncreaseShiled()
    {
        currentShield++;

        if (currentShield >= maxShield)
        {//계속 늘어남 방지
            currentShield = maxShield;
        }
        SettingShieldImage();
    }
    public void DecreaseShield(int p_num)
    {
        currentShield -= p_num;

        if (currentShield <= 0)
        {
            currentShield = 0;
        }

        SettingShieldImage();
    }
    void SettingShieldImage()
    {//현재 체력 보여줌
        for (int i = 0; i < shieldImage.Length; i++)
        {
            if (i < currentShield)
            {
                shieldImage[i].gameObject.SetActive(true);
            }
            else
                shieldImage[i].gameObject.SetActive(false);
        }
    }
    public void DecreaseHp(int p_num)
    {
        if (!isBlink)
        {
            if (currentShield > 0)
            {
                DecreaseShield(p_num);
            }
            else
            {
                currentHp -= p_num;

                if (currentHp <= 0)
                {
                    isDead = true;
                    result.ShowResult();
                    noteManager.RemoveNote();
                }
                else
                    StartCoroutine(BlinkCo());


                SettingHpImage();
            }
        }
    }

    void SettingHpImage()
    {//현재 체력 보여줌
        for(int i=0;i<hpImage.Length;i++)
        {
            if(i<currentHp)
            {
                hpImage[i].gameObject.SetActive(true);
            }
            else
                hpImage[i].gameObject.SetActive(false);
        }
    }
    public bool IsDead()
    {
        return isDead;
    }
    IEnumerator BlinkCo()
    {//hp감소 모션
        isBlink = true;
        while(currentblinkCount<=blinkCount)
        {
            playerMesh.enabled=!playerMesh.enabled;
            //반전
            yield return new WaitForSeconds(blinkSpeed);
            currentblinkCount++;
        }

        playerMesh.enabled=true;
        currentblinkCount=0;
        isBlink = false;
    }
}
