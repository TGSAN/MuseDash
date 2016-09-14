using UnityEngine;
using System.Collections;
using FormulaBase;
public class ChapterCell : MonoBehaviour {
	public int m_id;
	public UISprite m_Sprite;
	public UILabel TrophyNum;
	public GameObject m_LockedOb;
	public GameObject m_OpenedChpter;
	public UILabel m_ChapterName;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	/// <summary>
	/// 设置章节信息
	/// </summary>
	/// <param name="_index">Index.</param>
	public void SetData(int _index)
	{
		/*
		m_id=_index+1;
		ChapterManageComponent.Instance.SetChapterId(m_id);
		m_ChapterName.text=ChapterManageComponent.Instance.GetChapterName();
		if(m_id<=AccountManagerComponent.Instance.GetOpenedChapter())
		{
			m_LockedOb.gameObject.SetActive(false);
			m_OpenedChpter.gameObject.SetActive(true);	
			m_Sprite.spriteName=ChapterManageComponent.Instance.GetChapterSpriteName(m_id);
		}
		else 
		{
			m_LockedOb.gameObject.SetActive(true);
			m_OpenedChpter.gameObject.SetActive(false);	
			TrophyNum.text=ChapterManageComponent.Instance.GetChapterTrophyNum().ToString();
		}
*/
	}
}
