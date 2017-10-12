using UnityEngine;
using System.Collections;


namespace NewtonVR
{
	public class grabRalpha : NVRInteractableRotator
	{

		Vector3 InitialPosition;
	

		public override void BeginInteraction(NVRHand hand)
		{
			this.Rigidbody.isKinematic = false;

			base.BeginInteraction(hand);
		}
			
	}
}