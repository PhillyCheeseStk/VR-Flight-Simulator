/********************************************************************************//**
\file      Grabber.cs
\brief     Basic sample to demonstrating adding grabbing to Avatar SDK or custom hands.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OVRTouchSample;

namespace OVRTouchSample
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(VelocityTracker))]
    public class Grabber : MonoBehaviour
    {
        // Grip trigger thresholds for picking up objects, with some hysteresis.
        public const float THRESH_GRAB_BEGIN = 0.55f;
        public const float THRESH_GRAB_END = 0.35f;
        // Velocity threshold to distinguish between a throw and a drop.
        public const float THRESH_THROW_SPEED = 1.0f;

        // Demonstrates a quick/experimental approach to attaching objects to the hand using a FixedJoint. 
        // Not satisfactory in this use case. Since the Hand is allowed inside the geometry and the
        // non-kinematic held object is not, this leads to jittery physics as it attempts to move the
        // held object into the geometry, unless we specify a trivially breakable break force.
        // Needs a more elaborate solution.
        [SerializeField]
        private bool m_useFixedJointForGrabbedObject = false;

        // Demonstrates parenting the held object to the hand's transform when grabbed.
        // When false, the grabbed object is moved every FixedUpdate using MovePosition. 
        // Note that MovePosition is required for proper physics simulation. If you set this to true, you can
        // easily observe broken physics simulation by, for example, moving the bottom cube of a stacked
        // tower and noting a complete loss of friction.
        [SerializeField]
        private bool m_parentHeldObject = false;

        // Child/attached transforms of the grabber, indicating where to snap held objects to (if you snap them).
        // Also used for ranking grab targets in case of multiple candidates.
        [SerializeField]
        private Transform m_gripTransform = null;
        // Child/attached Colliders to detect candidate grabbable objects.
        [SerializeField]
        private Collider[] m_grabVolumes = null;

        // Avatar to pull hand poses from.
        [SerializeField]
        private OvrAvatar m_avatar;
        // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
        [SerializeField]
        private OVRInput.Controller m_controller;

        private VelocityTracker m_velocityTracker = null;
        private bool m_grabVolumeEnabled = true;
        private Vector3 m_lastPos;
        private Quaternion m_lastRot;
        private Quaternion m_anchorOffsetRotation;
        private Vector3 m_anchorOffsetPosition;
        private float m_prevFlex;
        private Transform m_parentTransform;
        private Grabbable m_grabbedObj = null;
        private Dictionary<Grabbable, int> m_grabCandidates = new Dictionary<Grabbable, int>();

        public void ForceRelease(Grabbable grabbable)
        {
            bool canRelease = (
                (m_grabbedObj != null) &&
                (m_grabbedObj == grabbable)
            );
            if (canRelease)
            {
                GrabEnd();
            }
        }
        private void Awake()
        {
            m_anchorOffsetPosition = transform.localPosition;
            m_anchorOffsetRotation = transform.localRotation;
        }

        private void Start()
        {
            m_velocityTracker = this.GetComponent<VelocityTracker>();
            m_lastPos = transform.position;
            m_lastRot = transform.rotation;
            m_parentTransform = gameObject.transform.parent.transform;
        }

        private void Update()
        {
            if (m_avatar != null && m_avatar.Driver != null)
            {
                float prevFlex = m_prevFlex;

                OvrAvatarDriver.PoseFrame frame;
                m_avatar.Driver.GetCurrentPose(out frame);
                OvrAvatarDriver.ControllerPose pose = m_controller == OVRInput.Controller.LTouch ? frame.controllerLeftPose : frame.controllerRightPose;
                // Update values from inputs
                m_prevFlex = pose.handTrigger;

                CheckForGrabOrRelease(prevFlex);
            }
        }

        // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
        // This is done instead of parenting to achieve workable physics. If you don't require physics on 
        // your hands or held objects, you may wish to switch to parenting.
        //
        // BUG: currently (Unity 5.5.0f3.), there's an unavoidable cosmetic issue with
        // the hand. FixedUpdate must be used, or else physics behavior is wildly erratic.
        // However, FixedUpdate cannot be guaranteed to run every frame, even when at 90Hz.
        // On frames where FixedUpdate fails to run, the hand will fail to update its position, causing apparent
        // judder. A fix is in progress, but not fixable on the user side at this time.
        private void FixedUpdate()
        {
            Vector3 handPos = OVRInput.GetLocalControllerPosition(m_controller);
            Quaternion handRot = OVRInput.GetLocalControllerRotation(m_controller);
            GetComponent<Rigidbody>().MovePosition(m_anchorOffsetPosition + handPos + m_parentTransform.position);
            GetComponent<Rigidbody>().MoveRotation(handRot * m_anchorOffsetRotation * m_parentTransform.rotation);

            if (!m_useFixedJointForGrabbedObject && !m_parentHeldObject)
            {
                MoveGrabbedObject();
            }
            m_lastPos = transform.position;
            m_lastRot = transform.rotation;
        }

        private void OnDestroy()
        {
            if (m_grabbedObj != null)
            {
                GrabEnd();
            }
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            // Get the grab trigger
            Grabbable grabbable = otherCollider.GetComponent<Grabbable>() ?? otherCollider.GetComponentInParent<Grabbable>();
            if (grabbable == null) return;

            // Add the grabbable
            int refCount = 0;
            m_grabCandidates.TryGetValue(grabbable, out refCount);
            m_grabCandidates[grabbable] = refCount + 1;
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            Grabbable grabbable = otherCollider.GetComponent<Grabbable>() ?? otherCollider.GetComponentInParent<Grabbable>();
            if (grabbable == null) return;

            // Remove the grabbable
            int refCount = 0;
            bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
            if (!found)
            {
                return;
            }

            if (refCount > 1)
            {
                m_grabCandidates[grabbable] = refCount - 1;
            }
            else
            {
                m_grabCandidates.Remove(grabbable);
            }
        }

        private void CheckForGrabOrRelease(float prevFlex)
        {
            if ((m_prevFlex >= THRESH_GRAB_BEGIN) && (prevFlex < THRESH_GRAB_BEGIN))
            {
                GrabBegin();
            }
            else if ((m_prevFlex <= THRESH_GRAB_END) && (prevFlex > THRESH_GRAB_END))
            {
                GrabEnd();
            }
        }

        private void GrabBegin()
        {
            float closestMagSq = float.MaxValue;
            Grabbable closestGrabbable = null;
            Collider closestGrabbableCollider = null;

            // Iterate grab candidates and find the closest grabbable candidate
            foreach (Grabbable grabbable in m_grabCandidates.Keys)
            {
                bool canGrab = !(grabbable.IsGrabbed && !grabbable.AllowOffhandGrab);
                if (!canGrab)
                {
                    continue;
                }

                for (int j = 0; j < grabbable.GrabPoints.Length; ++j)
                {
                    Collider grabbableCollider = grabbable.GrabPoints[j];
                    // Store the closest grabbable
                    Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                    float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                    if (grabbableMagSq < closestMagSq)
                    {
                        closestMagSq = grabbableMagSq;
                        closestGrabbable = grabbable;
                        closestGrabbableCollider = grabbableCollider;
                    }
                }
            }

            // Disable grab volumes to prevent overlaps
            GrabVolumeEnable(false);

            if (closestGrabbable != null)
            {
                if (closestGrabbable.IsGrabbed)
                {
                    closestGrabbable.GrabbedBy.OffhandGrabbed(closestGrabbable);
                }

                m_grabbedObj = closestGrabbable;
                m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

                if(m_useFixedJointForGrabbedObject)
                {
                    FixedJoint fj = gameObject.GetComponent<FixedJoint>() ?? gameObject.AddComponent<FixedJoint>();
                    fj.connectedBody = m_grabbedObj.GetComponent<Rigidbody>();
                }
                else
                {
                    // Teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
                    // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
                    // is beyond the scope of this demo.
                    m_lastPos = transform.position;
                    m_lastRot = transform.rotation;
                    MoveGrabbedObject(true);
                    if(m_parentHeldObject)
                    {
                        m_grabbedObj.transform.parent = transform;
                    }
                }
            }
        }

        private void MoveGrabbedObject(bool forceTeleport = false)
        {
            if (m_grabbedObj == null)
            {
                return;
            }

            Vector3 handInitialPosition = m_lastPos;
            Quaternion handInitialRotation = m_lastRot;
            Vector3 handFinalPosition = transform.position;
            Quaternion handFinalRotation = transform.rotation;
            Quaternion handDeltaRotation = handFinalRotation * Quaternion.Inverse(handInitialRotation);

            bool snapPosition = m_grabbedObj.SnapPosition;
            bool snapRotation = m_grabbedObj.SnapOrientation;
            Vector3 snapOffset = Vector3.zero;
            Quaternion snapRot = Quaternion.identity;
            if (snapPosition && m_grabbedObj.SnapOffset)
            {
                snapOffset = m_grabbedObj.SnapOffset.position;
                if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                snapOffset = transform.rotation * snapOffset;
            }
            if (snapRotation && m_grabbedObj.SnapOffset)
            {
                snapRot = m_grabbedObj.SnapOffset.rotation;
            }

            Rigidbody grabbedRigidbody = m_grabbedObj.GrabbedRigidbody;
            Transform grabbedTransform = grabbedRigidbody.transform;
            // snap uses:   gripTransform.position, transform.position, m_lastpos
            // nosnap uses: m_lastPos, transform.position, grabbedTransform.position
            Vector3 grabbablePosition = snapPosition ?
                m_gripTransform.position + snapOffset + handDeltaRotation * (handFinalPosition - handInitialPosition) : 
                handFinalPosition + handDeltaRotation * (grabbedTransform.position - handInitialPosition);
            Quaternion grabbableRotation = snapRotation ? 
                handDeltaRotation * m_gripTransform.rotation :
                handDeltaRotation * grabbedTransform.rotation;

            if(snapRotation && m_grabbedObj.SnapOffset)
            {
                grabbableRotation *= m_grabbedObj.SnapOffset.rotation;
            }

            if (forceTeleport)
            {
                grabbedRigidbody.transform.position = grabbablePosition;
                grabbedRigidbody.transform.rotation = grabbableRotation;
            }
            else
            {
                grabbedRigidbody.MovePosition(grabbablePosition);
                grabbedRigidbody.MoveRotation(grabbableRotation);
            }
        }

        private void GrabEnd()
        {
            if (m_grabbedObj != null)
            {
                // Determine if the grabbable was thrown, compute appropriate velocities.
                bool wasThrown = m_velocityTracker.TrackedLinearVelocity.magnitude >= THRESH_THROW_SPEED;
                Vector3 linearVelocity = Vector3.zero;
                Vector3 angularVelocity = Vector3.zero;
                if (wasThrown)
                {
                    // Throw velocity
                    linearVelocity = m_velocityTracker.TrackedLinearVelocity;
                    angularVelocity = m_velocityTracker.TrackedAngularVelocity;
                }
                else
                {
                    // Drop velocity
                    linearVelocity = m_velocityTracker.FrameLinearVelocity;
                    angularVelocity = m_velocityTracker.FrameAngularVelocity;
                }

                GrabbableRelease(linearVelocity, angularVelocity);
            }

            // Re-enable grab volumes to allow overlap events
            GrabVolumeEnable(true);
        }

        private void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            Destroy(gameObject.GetComponent<FixedJoint>());
            m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
            if(m_parentHeldObject) m_grabbedObj.transform.parent = null;
            m_grabbedObj = null;
        }

        private void GrabVolumeEnable(bool enabled)
        {
            if (m_grabVolumeEnabled == enabled)
            {
                return;
            }

            m_grabVolumeEnabled = enabled;
            for (int i = 0; i < m_grabVolumes.Length; ++i)
            {
                Collider grabVolume = m_grabVolumes[i];
                grabVolume.enabled = m_grabVolumeEnabled;
            }

            if (!m_grabVolumeEnabled)
            {
                m_grabCandidates.Clear();
            }
        }

        private void OffhandGrabbed(Grabbable grabbable)
        {
            if (m_grabbedObj == grabbable)
            {
                GrabbableRelease(Vector3.zero, Vector3.zero);
            }
        }
    }
}
