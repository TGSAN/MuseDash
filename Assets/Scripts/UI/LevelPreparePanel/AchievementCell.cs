//using UnityEngine;
//using System.Collections;
//using FormulaBase;
//
//public class AchievementCell : MonoBehaviour {
//
//	GameObject m_Finish;	//完成
//	GameObject m_UnFinish;	//未完成
//	UILabel m_GoldLabel;
//
//	private ushort stageId;
//	void Onalble()
//	{
//		
//	}
//	void Awake()
//	{
//		m_GoldLabel=transform.FindChild("Label").GetComponent<UILabel>();
//		m_Finish=transform.FindChild("fIshbg").gameObject;
//		m_UnFinish=transform.FindChild("unfIshbg").gameObject;
//	}
//
//	void SetFinish()
//	{
//		m_Finish.SetActive(true);
//		m_UnFinish.SetActive(false);
//	}
//	void SetUnfinish()
//	{
//		m_Finish.SetActive(false);
//		m_UnFinish.SetActive(true);
//	}
//	public void SetData(ushort _id,TaskStageTarget.Target _Goal,bool fished=false)
//	{
//		stageId = _id;
//		if (_Goal.id == 0) {
//			this.gameObject.SetActive (false);
//			return;
//		}
//		if (fished)
//			SetFinish ();
//		else 
//			SetUnfinish ();
//		this.gameObject.SetActive (true);
//
//		LitJson.JsonData jStr = ConfigPool.Instance.GetConfigValue ("Level_Goal", _Goal.id.ToString (), "goal");
//		if (jStr == null) {
//			Debug.Log ("Level_Goal " + _Goal.id + " has no config.");
//			return;
//		}
//		
//		string t_str = jStr.ToString ();
//		if (_Goal.strValue != null && t_str.Contains ("Y")) {
//			t_str = t_str.Replace ("Y", _Goal.strValue);
//		}
//		
//		if (t_str.Contains ("X")) {
//			t_str = t_str.Replace ("X", _Goal.value.ToString ());
//		}
//		
//		m_GoldLabel.text = t_str;
//		/*
//		if (_Goal.id == 1011) {	
//			string [] temp = TargetManager.Instance.GetTarget1011Strings ();
//			string t_str = (string)ConfigPool.Instance.GetConfigByName ("Level_Goal") [_Goal.id.ToString ()] ["goal"];
//
//			t_str = t_str.Replace ("X", temp [0]);
//			t_str = t_str.Replace ("Y", temp [1]);
//			m_GoldLabel.text = t_str;
//			//1000 20001
//		} else if (_Goal.id == 1002) {
//			string [] temp = TargetManager.Instance.GetTarget1011Strings ();
//			string t_str = (string)ConfigPool.Instance.GetConfigByName ("Level_Goal") [_Goal.id.ToString ()] ["goal"];
//
//			switch (_Goal.value) {
//			case 0:
//				t_str = t_str.Replace ("X", "C");
//				break;
//			case 1:
//				t_str = t_str.Replace ("X", "B");
//				break;
//			case 2:
//				t_str = t_str.Replace ("X", "A");
//				break;
//			case 3:
//				t_str = t_str.Replace ("X", "S");
//				break;
//			}
//			m_GoldLabel.text = t_str;
//				
//		} else {
//			string t_str = (string)ConfigPool.Instance.GetConfigByName ("Level_Goal") [_Goal.id.ToString ()] ["goal"];
//			t_str = t_str.Replace ("X", _Goal.value.ToString ());
//			m_GoldLabel.text = t_str;
//		}
//		*/
//	}
//	// Use this for initialization
//	void Start () {
//
//	
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//}
