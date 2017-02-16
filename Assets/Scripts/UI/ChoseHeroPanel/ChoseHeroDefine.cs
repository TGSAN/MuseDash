using UnityEngine;
using System.Collections;
namespace ChoseHeroDefine
{
	public class PlayerData
	{
		public int   _PlayrIndex=0;

		public string _Name = string.Empty;

		public string _PlayerText = string.Empty;

		public int _DemondGold = 1000;

		public bool _IsGet=false;

		public int[] _CosIDs = null;

		public PlayerAttr[] _Attrs=null;
	}
	/// <summary>
	/// 玩家的属性枚举
	/// </summary>
	public enum PLAYER_ATTRITE
	{
		/// <summary>
		/// 等级的上限
		/// </summary>
		LEVEL_MAX=1,
		/// <summary>
		/// 暴击的概率
		/// </summary>
		BAOJI_P,
		/// <summary>
		/// 金币的获取
		/// </summary>
		GOLD_GET,
		/// <summary>
		/// 体力的上限
		/// </summary>
		TILI_MAX,
		NODEFINE
	}
	/// <summary>
	/// 角色属性的定义属性
	/// </summary>
	public struct PlayerAttr
	{
		/// <summary>
		/// 属性的名字
		/// </summary>
		public string _Name ;
		/// <summary>
		/// 属性的整数值
		/// </summary>
		public int _Value ;
		/// <summary>
		/// 属性的浮点值
		/// </summary>
		public float _value2;
		/// <summary>
		/// 属性的类型
		/// </summary>
		public PLAYER_ATTRITE _tYpe;
	}

	/// <summary>
	/// 角色的类型
	/// </summary>
	public enum PLAYR_TYPE
	{
		PLAYER_LUMI=0,
		PLAYER_MALYA,
		PLAYER_BURO,
		PLAYER_MAX
	}
	/// <summary>
	/// 角色的装备情况
	/// </summary>
	public enum RESULT_EQUIP
	{
		/// <summary>
		/// 已经购买，未装备
		/// </summary>
		BUY_GET,
		/// <summary>
		/// 已经装备，已经购买
		/// </summary>
		EQUIP_GET,
		/// <summary>
		/// 未装备未购买
		/// </summary>
		NO_GET,
		/// <summary>
		/// 未定义
		/// </summary>
		NO_DEFINE
	}

}