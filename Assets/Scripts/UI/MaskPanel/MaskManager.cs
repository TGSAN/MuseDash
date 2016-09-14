using UnityEngine;
using System.Collections;

public class MaskManager 
{
	static MaskManager S_Instance = null;

	public static MaskManager Get()
	{
		if (S_Instance == null) 
		{
			S_Instance = new MaskManager ();
		}
		return S_Instance;
	}
	private GameObject m_UI=null;
	private MaskPanel.MaskPanel  m_HandleCs=null;

	public void RegistUI(GameObject _ui)
	{
		m_UI = _ui;
		m_HandleCs = m_UI.GetComponent<MaskPanel.MaskPanel> ();
	}


	public delegate void MaskAlphaCallBack();


	public void PlayUI(MaskAlphaCallBack _Call)
	{
		if(m_HandleCs!=null)
			m_HandleCs.PlayUI (_Call);
	}
}
