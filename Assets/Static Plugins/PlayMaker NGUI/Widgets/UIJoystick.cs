//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//
// original script: UIDragObject. Adapted by Jean Fabre, contact@fabrejean.net for PlayMaker NGUI port.
//----------------------------------------------

using UnityEngine;
using System.Collections;

/// <summary>
/// Allows dragging of an object by mouse or touch, like the typical joystick you find on mobile games.
/// The Pad input values are true to the pad visual position, so the spring back Speed affects the input value too.
/// The pad drag complies with NGUI in the sense that it truly drag within the local space, so you can have a pad in a 3d plane interface, and in any orientation.
/// </summary>

[AddComponentMenu("NGUI/Interaction/JoyStick")]
public class UIJoystick : MonoBehaviour
{

	/// <summary>
	/// Target object that will be dragged. Made private, I am not getting the purpose of making this public or actually different then 'this', 
	/// I would need to see an example to understand the use, cause I am sure it's here for a reason.
	/// </summary>

	Transform target;

	/// <summary>
	/// Scale value applied to the drag delta. Set X or Y to 0 to disallow dragging in that direction.
	/// </summary>

	public Vector3 scale = Vector3.one;

	/// <summary>
	/// The Range of the pad in local coordinates. the value is for one side, so the full range is actually twice the values.
	/// If sprite is pixel perfect, than range is express in pixels too.
	/// </summary>
	
	public Vector2 range = new Vector2(100f,100f);
	
	/// <summary>
	/// The Pad dead zone in local coordinates.
	/// </summary>
	public float deadZone = 4f;
	
	/// <summary>
	/// If true, the pad is constraint as a circle, not as a rectangle, and only the x value of the range is taken into account.
	/// </summary>
	/// 
	public bool circularPadConstraint = false;
	
	/// <summary>
	/// When released, how fast the pad comes back to center. It affects the Input values.
	/// </summary>
	public float springBackSpeed = 20f;
	
	
	/// <summary>
	/// the actual Pad Position, use this as the input source, range from -1 to 1 on both axis
	/// </summary>

	public Vector2 padPosition;

	/// <summary>
	/// The pad angle, 0° being the pad forward ( when x = 0 and y>0).
	/// </summary>
	public float padAngle;
	
	/// <summary>
	/// the pad input values in one variable. XY are the input values H and V, Z is the angle. Use this for network streaming; saves bandwith ( not measured, I assume it does...).
	/// </summary>
	public Vector3 padPositionAndAngle;
	
	
	Vector3 mStartPos;
	Vector3 mStartLocalPos;
	
	Plane mPlane;
	Vector3 mLastPos;
	
	bool mPressed = false;
	
	Vector3 totalOffset;
	Vector3 startOffset;
	Vector3 totalWorldOffset;
	
	bool mDragStarted;
	Vector2 mDragStartOffset;
	
	bool started;

	
	void Start()
	{
		target = this.transform;
	}

	/// <summary>
	/// Create a plane on which we will be performing the dragging.
	/// </summary>

	void OnPress (bool pressed)
	{

		if (enabled && NGUITools.GetActive(gameObject))
		{
			mPressed = pressed;

			if (pressed)
			{
				if (!started)
				{
					started = true;
					mStartPos = this.transform.position;
					mStartLocalPos = this.transform.localPosition;
				}
				
				this.transform.position = mStartPos;
				mDragStarted = false;
				totalOffset = Vector3.zero;
				
				// Create the plane to drag along
				Transform trans = UICamera.currentCamera.transform;
				mPlane = new Plane(trans.rotation * Vector3.back, mLastPos);
			}
		}
	}
	
	void Update()
	{
		if (!mPressed)
		{
			if (Vector3AlmostEquals(target.position,mStartPos,0.1f))
			{
				// spring back Pad to center
				target.position = Vector3.Lerp(target.position,mStartPos, Time.deltaTime*springBackSpeed);
			}
		}
	}
	
	 /// <summary>compares the square magniture of target - second to given float value</summary>
    static bool Vector3AlmostEquals(Vector3 target, Vector3 second, float sqrMagniturePrecision)
    {
        return (target - second).sqrMagnitude < sqrMagniturePrecision;
    }
	
	/// <summary>
	/// Drag the object along the plane.
	/// </summary>

	void OnDrag (Vector2 delta)
	{
		if (enabled && NGUITools.GetActive(gameObject))
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			
			// Prevents the drag "jump". Contributed by 'mixd' from the Tasharen forums.
			// modified for the purpose, but this is the same deal.
			if (!mDragStarted)
			{
				mDragStarted = true;
				// Remember the hit position
				mLastPos = UICamera.lastHit.point;
			}
			
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float dist = 0f;

			if (mPlane.Raycast(ray, out dist))
			{
				Vector3 currentPos = ray.GetPoint(dist);
				Vector3 offset = currentPos - mLastPos;
				mLastPos = currentPos;
				
				if (offset.x != 0f || offset.y != 0f)
				{
					offset = target.InverseTransformDirection(offset);
					offset.Scale(scale);
					offset = target.TransformDirection(offset);
				}
				
				totalOffset += offset;
				target.position = mStartPos + totalOffset;
			}
		}
	}
	
	/// <summary>
	/// Constraints the Pad and compute the input values
	/// </summary>
	
	void LateUpdate()
	{
		if (!started)
		{
			return;
		}
		
		Vector3 pos = target.transform.localPosition;
		
		if (!circularPadConstraint)
		{
			pos.x = Mathf.Clamp(pos.x,mStartLocalPos.x-range.x,mStartLocalPos.x+range.x);
			pos.y = Mathf.Clamp(pos.y,mStartLocalPos.y-range.y,mStartLocalPos.y+range.y);
		}else{
			pos = Vector3.ClampMagnitude(pos,range.x);
		}
		
		target.transform.localPosition = pos;
		
		// feed the feedback values.
		
		Vector3 offset = pos-mStartLocalPos;
		//deadzone
		if (offset.magnitude<=deadZone)
		{
			padPosition =  Vector2.zero;
			padAngle = 0f;
			padPositionAndAngle = Vector3.zero;
		}else{
		
			// get the pad input values;
			padPosition.x = offset.x/(range.x);
			padPosition.y = offset.y/(range.y);
			
			
			padAngle = Mathf.Atan2(padPosition.x,padPosition.y) * 180.0f / 3.14159f;
			
			
			padPositionAndAngle.x = padPosition.x;
			padPositionAndAngle.y = padPosition.y;
			padPositionAndAngle.z = padAngle;
		}
	}
	
}