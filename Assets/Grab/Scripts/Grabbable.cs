/********************************************************************************//**
\file      Grabbable.cs
\brief     Component that allows objects to be grabbed by the hand.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using System;
using UnityEngine;

namespace OVRTouchSample
{
    // Which object had its grabbed state changed, and whether the grab is starting or ending.
    public class Grabbable : MonoBehaviour
    {
        [SerializeField]
        private bool m_allowOffhandGrab = true;
        [SerializeField]
        private bool m_snapPosition = false;
        [SerializeField]
        private bool m_snapOrientation = false;
        [SerializeField]
        private Transform m_snapOffset;
        [SerializeField]
        private Collider[] m_grabPoints = null;

        private bool m_grabbedKinematic = false;
        private Collider m_grabbedCollider = null;
        private Grabber m_grabbedBy = null;

        // Color-changing support for demo purposes.
        // Can be easily removed with no loss of functionality (aside from coloring the cubes and balls, of course).
        // (Search this file for "color-changing support.")
        public static readonly Color COLOR_GRAB = new Color(1.0f, 0.5f, 0.0f, 1.0f);
        private Color m_color = Color.black;
        private MeshRenderer[] m_meshRenderers = null;

        public bool AllowOffhandGrab
        {
            get { return m_allowOffhandGrab; }
        }

        public bool IsGrabbed
        {
            get { return m_grabbedBy != null; }
        }
        public bool SnapPosition
        {
            get { return m_snapPosition; }
        }
        public bool SnapOrientation
        {
            get { return m_snapOrientation; }
        }
        public Transform SnapOffset
        {
            get { return m_snapOffset; }
        }

        public Grabber GrabbedBy
        {
            get { return m_grabbedBy; }
        }

        public Transform GrabbedTransform
        {
            get { return m_grabbedCollider.transform; }
        }

        public Rigidbody GrabbedRigidbody
        {
            get { return m_grabbedCollider.attachedRigidbody; }
        }

        public Collider[] GrabPoints
        {
            get { return m_grabPoints; }
        }

        virtual public void GrabBegin(Grabber hand, Collider grabPoint)
        {
            m_grabbedBy = hand;
            m_grabbedCollider = grabPoint;
            //gameObject.GetComponent<Rigidbody>().isKinematic = true;

            // Color-changing support.
            SetColor(COLOR_GRAB);
        }

        virtual public void GrabEnd(Vector3 linearVelocity, Vector3 angularVelocity)
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
            rb.isKinematic = m_grabbedKinematic;
            rb.velocity = linearVelocity;
            rb.angularVelocity = angularVelocity;
            m_grabbedBy = null;
            m_grabbedCollider = null;

            // Color-changing support.
            SetColor(m_color);
        }

        virtual protected void Awake()
        {
            if (m_grabPoints.Length == 0)
            {
                // Get the collider from the grabbable
                Collider collider = this.GetComponent<Collider>();
                if (collider == null)
                {
                    throw new ArgumentException("Grabbable: Grabbables cannot have zero grab points and no collider -- please add a grab point or collider.");
                }

                // Create a default grab point
                m_grabPoints = new Collider[1] { collider };
            }

            // Color-changing support.
            // Random color selection on awake.
            m_color = new Color(
                UnityEngine.Random.Range(0.1f, 0.95f),
                UnityEngine.Random.Range(0.1f, 0.95f),
                UnityEngine.Random.Range(0.1f, 0.95f),
                1.0f
            );
            m_meshRenderers = this.GetComponentsInChildren<MeshRenderer>();
            SetColor(m_color);
        }

        private void Start()
        {
            m_grabbedKinematic = GetComponent<Rigidbody>().isKinematic;
        }

        private void OnDestroy()
        {
            if (m_grabbedBy != null)
            {
                // Notify the hand to release destroyed grabbables
                m_grabbedBy.ForceRelease(this);
            }
        }

        private void SendMsg(string msgName, object msg)
        {
            this.transform.SendMessage(msgName, msg, SendMessageOptions.DontRequireReceiver);
        }

        // Color-changing support.
        private void SetColor(Color color)
        {
            for (int i = 0; i < m_meshRenderers.Length; ++i)
            {
                MeshRenderer meshRenderer = m_meshRenderers[i];
                for (int j = 0; j < meshRenderer.materials.Length; ++j)
                {
                    Material meshMaterial = meshRenderer.materials[j];
                    meshMaterial.color = color;
                }
            }
        }
    }
}
