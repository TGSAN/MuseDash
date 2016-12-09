using FormulaBase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//获取物品奖励的界面
public class RewardPanel : MonoBehaviour
{
    public GameObject m_Particle1;  //4个粒子特效
    public GameObject m_Particle2;
    public GameObject m_Particle3;
    public GameObject m_Particle4;
    public UISprite m_QualitySprite;        //品质的文字

    public GameObject m_OkButton;
    public List<UISprite> m_ListSprite = new List<UISprite>();

    private List<string> m_IconName = new List<string>();//显示物品的名字
    private List<int> m_ListQuality = new List<int>();
    public GameObject m_mask;

    public TweenScale m_ShowItem;//显示物品
    public TweenScale m_ShowItemLabel;//显示物品品质的文字的动画
    public UISprite m_ShowSprite;

    public UIPlayTween m_Play;
    public UIPlayTween m_PlayAllItem;

    public TweenScale m_RewardPanelFinishAni;
    public TweenAlpha m_RewardPanelFinishAni2;

    private int Now_Index = 0;

    public GameObject m_RewardPanel;

    public void ClcikScreen(GameObject _ob)
    {
        if (Now_Index == m_IconName.Count - 1)
        {
            return;
        }
        FinishAni();
    }

    // Use this for initialization
    private void Start()
    {
        UIEventListener.Get(m_mask).onClick = ClcikScreen;
    }

    private void OnEnable()
    {
        m_OkButton.SetActive(false);
        m_RewardPanel.SetActive(false);
        this.transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// 播放显示物品的动画
    /// </summary>
    /// <param name="_name">Name.</param>
    public void PlaySHowItem()
    {
        m_ShowSprite.spriteName = m_IconName[Now_Index];
        HideAllParticle();
        switch (m_ListQuality[Now_Index])
        {
            case 1:
                m_Particle1.SetActive(true);
                m_QualitySprite.spriteName = "common";
                break;

            case 2:
                m_Particle2.SetActive(true);
                m_QualitySprite.spriteName = "rare";
                break;

            case 3:
                m_Particle3.SetActive(true);
                m_QualitySprite.spriteName = "epic";
                break;

            case 4:
                m_Particle4.SetActive(true);
                m_QualitySprite.spriteName = "legendary";
                break;
        }
        m_ShowItemLabel.gameObject.SetActive(true);
        m_Play.Play(true);
    }

    private void HideAllParticle()
    {
        m_Particle1.SetActive(false);
        m_Particle2.SetActive(false);
        m_Particle3.SetActive(false);
        m_Particle4.SetActive(false);
    }

    public void ShowAllGet()
    {
        for (int i = 0; i < m_ListSprite.Count; i++)
        {
            if (i < m_IconName.Count)
            {
                m_ListSprite[i].gameObject.SetActive(true);
                switch (m_ListQuality[i])
                {
                    case 1:
                        m_ListSprite[i].transform.FindChild("Quality").GetComponent<UISprite>().spriteName = "groove_1";
                        break;

                    case 2:
                        m_ListSprite[i].transform.FindChild("Quality").GetComponent<UISprite>().spriteName = "groove_2";
                        break;

                    case 3:
                        m_ListSprite[i].transform.FindChild("Quality").GetComponent<UISprite>().spriteName = "groove_3";
                        break;

                    case 4:
                        m_ListSprite[i].transform.FindChild("Quality").GetComponent<UISprite>().spriteName = "groove_4";
                        break;
                }

                //	m_ListSprite[i].transform.FindChild("Quality").GetComponent<UISprite>().spriteName=""
            }
            else
            {
                m_ListSprite[i].gameObject.SetActive(false);
            }
        }
        m_PlayAllItem.Play(true);
    }

    public void FinishAni()
    {
        if (Now_Index == m_IconName.Count - 1)
        {
            Debug.Log("播放完了");
            m_ShowItem.gameObject.SetActive(false);
            m_ShowItemLabel.gameObject.SetActive(false);
            m_ListSprite[Now_Index].spriteName = m_ShowSprite.spriteName;
            HideAllParticle();
            ShowAllGet();
            StartCoroutine("ShowOKButotn");
        }
        else
        {
            m_ListSprite[Now_Index].spriteName = m_ShowSprite.spriteName;
            Now_Index++;
            PlaySHowItem();
        }
    }

    private IEnumerator ShowOKButotn()
    {
        yield return new WaitForSeconds(m_ListQuality.Count * 0.12f);
        m_OkButton.SetActive(true);
    }

    public FormulaHost m_nowChest;

    /// <summary>
    ///
    /// </summary>
    /// <param name="_index">Index.</param>
    public void ShowGetItem(FormulaHost _Chesthost)
    {
        m_RewardPanelFinishAni2.GetComponent<UIWidget>().alpha = 1;//ResetToBeginning();
                                                                   //m_RewardPanelFinishAni.Play(true);

        Now_Index = 0;
        m_ShowItem.gameObject.SetActive(true);
        m_IconName.Clear();
        m_ShowItem.gameObject.SetActive(true);
        for (int i = 0; i < m_ListSprite.Count; i++)
        {
            m_ListSprite[i].gameObject.SetActive(false);
        }
        m_nowChest = _Chesthost;
        this.gameObject.SetActive(true);
        m_IconName = RandItem(_Chesthost);

        PlaySHowItem();
    }

    public void DeleteChest()
    {
        ChestManageComponent.Instance.ReomoveChest(m_nowChest);//.Remove();

        //	Messenger.Broadcast(bagPanel2.BroadcastBagItem);
    }

    public void ClickOkButton()
    {
        //		m_RewardPanelFinishAni.ResetToBeginning();
        //		m_RewardPanelFinishAni.Play(true);
        m_RewardPanelFinishAni2.ResetToBeginning();
        m_RewardPanelFinishAni2.Play(true);
    }

    public void ClickOkAniFinish()
    {
        //CommonPanel.GetInstance().CloseBlur(this.gameObject);//(CommonPanel.GetInstance().GetComponent<UIPanel>());
        DeleteChest();
        ItemManageComponent.Instance.AddItemList(t_AddItem);
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// 保底设置产出
    /// </summary>
    /// <returns>The rand quality get.</returns>
    /// <param name="_Probability">Probability.</param>
    /// <param name="_start">Start.</param>
    public int LitmitRandQualityGet(float[] _Probability, int _start)
    {
        float allpro = 0f;
        for (int i = _start; i < 4; i++)
        {
            allpro += _Probability[i];
        }
        float temp = Random.Range(0, allpro);
        for (int j = _start; j < 4; j++)        //随机品质
        {
            temp -= _Probability[j];
            if (temp <= 0)
            {
                return j + 1;
            }
        }
        Debug.Log("错误的随机种子");//错误 return 0；
        return 0;
    }

    private List<FormulaHost> t_AddItem = new List<FormulaHost>();          //服务器获取的添加的物品

    /// <summary>
    /// 获取奖励物品的图片名字
    /// </summary>
    /// <returns>The item.</returns>
    private List<string> RandItem(FormulaHost _Chesthost)
    {
        m_ListQuality.Clear();
        //		int Count=(int)_Chesthost.Result(FormulaKeys.FORMULA_95);
        List<string> tListSprtieName = new List<string>();
        float[] Probability = new float[4];
        Probability[0] = SundryManageComponent.Instance.GetVaule(1);    //4品质 绿
        Probability[1] = SundryManageComponent.Instance.GetVaule(2);    //蓝
        Probability[2] = SundryManageComponent.Instance.GetVaule(3);    //紫
        Probability[3] = SundryManageComponent.Instance.GetVaule(4);    //黄
        float[] tmep = new float[7];
        tmep[0] = TrophyManageComponent.Instance.GetNormalFoodProbability();//7概率
        tmep[1] = TrophyManageComponent.Instance.GetPetFoodProbability();
        tmep[2] = TrophyManageComponent.Instance.GetRoleUpStarsProbability();
        tmep[3] = TrophyManageComponent.Instance.GetEquipUpStarsProbability();
        tmep[4] = TrophyManageComponent.Instance.GetWeaponProbablity();
        tmep[5] = TrophyManageComponent.Instance.GetAccessoriesProbality();
        //		//tmep[6]=TrophyManageComponent.Instance.GetPetPatchProbality();
        //		Debug.Log("这地方有点问题 日了 狗");
        tmep[6] = 0.25f;

        #region 最低产出

        int[] RewardQuality = { 0, 0, 0, 0 };
        //		RewardQuality[0]=(int)_Chesthost.Result(FormulaKeys.FORMULA_96);
        //		RewardQuality[1]=(int)_Chesthost.Result(FormulaKeys.FORMULA_97);
        //		RewardQuality[2]=(int)_Chesthost.Result(FormulaKeys.FORMULA_98);
        //		RewardQuality[3]=(int)_Chesthost.Result(FormulaKeys.FORMULA_99);

        //int allLimit=RewardQuality[0]+RewardQuality[1]+RewardQuality[2]+RewardQuality[3];
        //Debug.Log("获取装备的概率"+RewardQuality[0]+"sss"+RewardQuality[1]+"sss"+RewardQuality[2]+"RewardQuality3"+RewardQuality[3]+"RewardQuality4");

        #endregion 最低产出

        //获取类型
        float t_Probality = Random.Range(0f, 1f);//
        float t_Quality = Random.Range(0f, 1f);
        int Type = 0;
        int Quality = 0;
        int temp_id = 0;
        List<RewardData> m_RandItem = new List<RewardData>();
        t_AddItem.Clear();
        //	List<FormulaHost> t_AddItem=new List<FormulaHost>();			//服务器获取的添加的物品
        for (int i = 0; i < 1; i++)
        {
            t_Probality = Random.Range(0f, 1f);
            t_Quality = Random.Range(0f, 1f);
            for (int j = 0; j < 7; j++)     //随机类型
            {
                t_Probality -= tmep[j];
                if (t_Probality <= 0)
                {
                    Type = j;
                    break;
                }
            }
            if (0 < RewardQuality[0] + RewardQuality[1] + RewardQuality[2] + RewardQuality[3])
            {
                for (int k = 0; k < 4; k++)
                {
                    if (RewardQuality[k] != 0)
                    {
                        switch (k)
                        {
                            case 0:
                                Debug.Log("保底绿色？");
                                Quality = LitmitRandQualityGet(Probability, 0);
                                break;//绿
                            case 1:
                                Quality = LitmitRandQualityGet(Probability, 1);
                                break;//蓝
                            case 2:
                                Quality = LitmitRandQualityGet(Probability, 2);
                                break;//紫
                            case 3:
                                Quality = LitmitRandQualityGet(Probability, 3);
                                break;//橙
                        }
                        RewardQuality[k]--;
                    }
                }
            }
            else
            {
                for (int j = 0; j < 4; j++)     //随机品质
                {
                    t_Quality -= Probability[j];
                    if (t_Quality <= 0)
                    {
                        Quality = j + 1;
                        break;
                    }
                }
            }
            switch (Type)
            {
                case 0://		tmep[0]=TrophyManageComponent.Instance.GetNormalFoodProbability();//7概率
                    m_RandItem = MaterialManageComponent.Instance.GetLimitItem(Quality, 1);
                    if (m_RandItem.Count != 0)
                    {
                        temp_id = m_RandItem[Random.Range(0, m_RandItem.Count)].id; //获取随机的ID
                        t_AddItem.Add(MaterialManageComponent.Instance.CreateItem(temp_id));        //增加获取的ID
                        tListSprtieName.Add(temp_id.ToString());
                    }
                    break;

                case 1://		tmep[1]=TrophyManageComponent.Instance.GetPetFoodProbability();
                    m_RandItem = MaterialManageComponent.Instance.GetLimitItem(Quality, 2);
                    if (m_RandItem.Count != 0)
                    {
                        temp_id = m_RandItem[Random.Range(0, m_RandItem.Count)].id; //获取随机的ID
                        t_AddItem.Add(MaterialManageComponent.Instance.CreateItem(temp_id));        //增加获取的ID
                        tListSprtieName.Add(temp_id.ToString());
                    }
                    break;

                case 2://		tmep[2]=TrophyManageComponent.Instance.GetRoleUpStarsProbability();
                    Debug.LogWarning("这地方角色升级素材的品质 有点问题");
                    m_RandItem = MaterialManageComponent.Instance.GetLimitItem(3, 3);
                    if (m_RandItem.Count != 0)
                    {
                        temp_id = m_RandItem[Random.Range(0, m_RandItem.Count)].id;                 //获取随机的ID
                        t_AddItem.Add(MaterialManageComponent.Instance.CreateItem(temp_id));        //增加获取的ID
                        tListSprtieName.Add(temp_id.ToString());
                    }
                    break;

                case 3://材料//		tmep[3]=TrophyManageComponent.Instance.GetEquipUpStarsProbability();
                    m_RandItem = MaterialManageComponent.Instance.GetLimitItem(Quality, 4);
                    if (m_RandItem.Count != 0)
                    {
                        temp_id = m_RandItem[Random.Range(0, m_RandItem.Count)].id; //获取随机的ID
                        t_AddItem.Add(MaterialManageComponent.Instance.CreateItem(temp_id));        //增加获取的ID
                        tListSprtieName.Add(temp_id.ToString());
                    }
                    break;

                case 4: //装备
                    m_RandItem = EquipManageComponent.Instance.GetLimitItem(Quality, 1);
                    if (m_RandItem.Count != 0)
                    {
                        temp_id = m_RandItem[Random.Range(0, m_RandItem.Count)].id; //获取随机的ID
                        t_AddItem.Add(EquipManageComponent.Instance.CreateItem(temp_id));//增加获取的ID
                        tListSprtieName.Add(temp_id.ToString());
                    }
                    break;

                case 5://
                    m_RandItem = EquipManageComponent.Instance.GetLimitItem(Quality, 2);
                    if (m_RandItem.Count != 0)
                    {
                        temp_id = m_RandItem[Random.Range(0, m_RandItem.Count)].id; //获取随机的ID
                        t_AddItem.Add(EquipManageComponent.Instance.CreateItem(temp_id));//增加获取的ID
                        tListSprtieName.Add(temp_id.ToString());
                    }
                    break;

                case 6:
                    m_RandItem = PetManageComponent.Instance.GetLimitItem(Quality, 6);
                    if (m_RandItem.Count != 0)
                    {
                        temp_id = m_RandItem[Random.Range(0, m_RandItem.Count)].id; //获取随机的ID
                        t_AddItem.Add(PetManageComponent.Instance.CreateItem(temp_id));//增加获取的ID
                        tListSprtieName.Add(temp_id.ToString());
                    }
                    break;
            }
            m_ListQuality.Add(Quality);
        }
        //Debug.Log("Count::"+tListSprtieName.Count);

        for (int i = 0, max = tListSprtieName.Count; i < max; i++)
        {
            string temp = tListSprtieName[i];
            int randnumber = Random.Range(0, max);
            tListSprtieName[i] = tListSprtieName[randnumber];
            tListSprtieName[randnumber] = temp;

            int tquality = m_ListQuality[i];
            m_ListQuality[i] = m_ListQuality[randnumber];
            m_ListQuality[randnumber] = tquality;
        }
        return tListSprtieName;
    }

    // Update is called once per frame
    private void Update()
    {
    }
}