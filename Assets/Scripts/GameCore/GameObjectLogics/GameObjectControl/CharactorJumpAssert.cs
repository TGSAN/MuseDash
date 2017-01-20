using UnityEngine;
using System.Collections;
using GameLogic;

public class CharactorJumpAssert : MonoBehaviour
{
    private Animator _animator;
    private Coroutine _coroutine;

    public float delay;
    /*
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}
	*/

    public void DoDelay()
    {
        if (this._animator == null)
        {
            this._animator = this.gameObject.GetComponent<Animator>();
        }

        if (this._animator == null)
        {
            return;
        }

        this._animator.speed = 0f;

        if (this._coroutine != null)
        {
            this.StopCoroutine(this._coroutine);
        }

        this._coroutine = this.StartCoroutine(this.__DoDelay());
    }

    private IEnumerator __DoDelay()
    {
        yield return new WaitForSeconds(this.delay);

        this._coroutine = null;
        this._animator.speed = GameGlobal.TIME_SCALE;
    }
}