using UnityEngine;

public class VicToryBehaviour : StateMachineBehaviour
{
//	public GameObject particle;
//	public float radius;
//	public float power;
//
//	protected GameObject clone;

//	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		clone = Instantiate(particle, animator.rootPosition, Quaternion.identity) as GameObject;
//		var rb = clone.GetComponent<Rigidbody>();
//		rb.AddExplosionForce(power, animator.rootPosition, radius, 3.0f);
//	}
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
//		animator.gameObject.SetActive(false);
//
//		animator.GetComponent<RechargePanel2>().FinishAni();
		//particle.SetActive(false);
		//Destroy(particle);
	}
//	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		Debug.Log("On Attack Update ");
//	}
//	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		Debug.Log("On Attack Move ");
//	}
//	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//	{
//		Debug.Log("On Attack IK ");
//	}
}