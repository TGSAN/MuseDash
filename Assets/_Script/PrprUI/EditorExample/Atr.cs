using UnityEngine;
using System.Collections;

[HelpURL("http://www.baidu.com")]
public class Atr : MonoBehaviour
{
	public int id;
	public string Name;
	[Multiline(5)]
	public string BackStory;

	public float health;
	public float damage;

	public float weaponDamagel, weaponDamage2;
	public string shoeName;
	public int shoeSize;
	public string shoeType;

	[Space(100)]
	[Range(-2,5)]
	public int time;
	void Start ()
	{
		health = 50;
	}
}