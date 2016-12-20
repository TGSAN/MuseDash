////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IOSProductTemplate  {

	//Editor Only
	public bool IsOpen = true;


	[SerializeField]
	private bool _IsAvaliable = false;

	[SerializeField]
	private string _Id = string.Empty;

	[SerializeField]
	private string _DisplayName =  "New Product";

	[SerializeField]
	private string _Description;

	[SerializeField]
	private float _Price = 0.99f;

	[SerializeField]
	private string _LocalizedPrice = string.Empty;

	[SerializeField]
	private string _CurrencySymbol = "$";

	[SerializeField]
	private string _CurrencyCode = "USD";

	[SerializeField]
	private Texture2D _Texture;

	[SerializeField]
	private ISN_InAppType _ProductType = ISN_InAppType.Consumable;

	[SerializeField]
	private ISN_InAppPriceTier _PriceTier = ISN_InAppPriceTier.Tier1;



	public void UpdatePriceByTier() {
		int tierint = (int) _PriceTier;

		float FloatPrice = tierint + 1f - 0.01f;
		_Price = FloatPrice;
	}
	

	public string Id {
		get {
			return _Id;
		}

		set {
			_Id = value;
		}
	}

	[System.Obsolete("Title is deprecated, please use DisplayName instead.")]
	public string Title {
		get {
			return _DisplayName;
		}

		set {
			_DisplayName = value;
		}
	}

	public string DisplayName {
		get {
			return _DisplayName;
		}
		
		set {
			_DisplayName = value;
		}
	}




	public string Description {
		get {
			return _Description;
		}

		set {
			_Description = value;
		}
	}

	public ISN_InAppType ProductType {
		get {
			return _ProductType;
		}

		set {
			_ProductType =  value;
		}
	}

	public float Price {
		get {
			return _Price;
		} 

		set {
			_Price = value;
		}
	}

	public int PriceInMicros {
		get {
			return System.Convert.ToInt32(_Price * 1000000f);
		} 

	}

	public string LocalizedPrice {
		get {
			if(_LocalizedPrice.Equals(string.Empty)) {
				return Price + " " + _CurrencySymbol;
			} else {
				return _LocalizedPrice;
			}

		}

		set {
			_LocalizedPrice = value;
		}
	}

	public string CurrencySymbol {
		get {
			return _CurrencySymbol;
		} 

		set {
			_CurrencySymbol = value;
		}
	}

	public string CurrencyCode {
		get {
			return _CurrencyCode;
		}

		set {
			_CurrencyCode = value;
		}
	}
	
	public Texture2D Texture {
		get {
			return _Texture;
		}

		set {
			_Texture = value;
		}
	}

	public ISN_InAppPriceTier PriceTier {
		get {
			return _PriceTier;
		}

		set {
			_PriceTier = value;
		}
	}

	public bool IsAvaliable {
		get {
			return _IsAvaliable;
		}

		set {
			_IsAvaliable = value;
		}
	}
}
