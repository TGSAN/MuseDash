using UnityEngine;
using System.Collections;

namespace MaskPanel {
	public class MaskPanel : UIPanelBase 
	{
		public TweenAlpha _MaskTweenAlpha=null;
		// Use this for initialization
		void Start () 
		{
		
		}
		
		// Update is called once per frame
		void Update () 
		{
			
		}
		/// <summary>
		/// 播放UI动画
		/// </summary>
		public void PlayUI(MaskManager.MaskAlphaCallBack _Call)
		{
			if (_MaskTweenAlpha != null) 
			{
				_MaskTweenAlpha.ResetToBeginning ();
				_MaskTweenAlpha.PlayForward ();
				_MaskTweenAlpha.onFinished.Clear ();
				_MaskTweenAlpha.AddOnFinished (()=>
					{
						if(_Call!=null)
						{
							_Call();
						}
					}
				);
			}
		}
	}
}