using UnityEngine;
using System.Collections;
namespace AssetBundleDefine
{
	/// <summary>
	/// 资源的类型定义
	/// </summary>
	public enum AssetBundleType
	{
		RES_AUDIO,
		RES_GAMEOBJ,
		RES_OBJ
	}
	/// <summary>
	/// 资源的加载请求
	/// </summary>
	public class ResLoadReq
	{
		/// <summary>
		/// 资源类型
		/// </summary>
		public AssetBundleType _ResType = AssetBundleType.RES_OBJ;
		/// <summary>
		/// 资源路径
		/// </summary>
		public string _ResPath = string.Empty;
		/// <summary>
		/// 加载得到的资源
		/// </summary>
		public Object _Res = null;
	}
	/// <summary>
	/// 资源的路径配置文件
	/// </summary>
	public class AssetPathData
	{
		/// <summary>
		/// asb的路径信息
		/// </summary>
		public string AsbPath = string.Empty;
		/// <summary>
		/// 资源的ID
		/// </summary>
		public string AssetID = string.Empty;
		/// <summary>
		/// 资源的路径
		/// </summary>
		public string ResPath = string.Empty;
	}

	public delegate void wwwLoadCallBack<T>(T wwwLoad);
}
