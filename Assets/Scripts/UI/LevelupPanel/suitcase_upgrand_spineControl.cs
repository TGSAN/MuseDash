using UnityEngine;
using System.Collections;

public class suitcase_upgrand_spineControl : MonoBehaviour {

	public EquipAndPetLevelUpPanel m_EquipAndPetLevelUpPanel;
	public void AnimationFinish()
	{
		m_EquipAndPetLevelUpPanel.FinishUseItemAnimation();
	}
}
