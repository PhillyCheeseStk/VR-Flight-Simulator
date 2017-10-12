/********************************************************************************//**
\file      VelocityTracker.cs
\brief     Tracks velocity of an object during a window of time.
\copyright Copyright 2015 Oculus VR, LLC All Rights reserved.
************************************************************************************/

using UnityEngine;

namespace OVRTouchSample
{
    public class VelocityTracker : MonoBehaviour
    {
        public const float WINDOW_TIME = 1.0f / 90.0f;
        public const float WINDOW_EPSILON = 0.0001f;
        public const float LINEAR_SPEED_WINDOW = WINDOW_TIME * 8.0f;
        public const float LINEAR_VELOCITY_WINDOW = WINDOW_TIME * 4.0f;
        public const float ANGULAR_VELOCITY_WINDOW = WINDOW_TIME * 2.0f;
        public const int MAX_SAMPLES = 45;

        private struct Sample
        {
            public float Time;
            public float SquaredLinearSpeed;
            public Vector3 LinearVelocity;
            public Vector3 AngularVelocity;
        }

        [SerializeField]
        private bool m_showGizmos = true;

        private int m_index = -1;
        private int m_count = 0;
        private Sample[] m_samples = new Sample[MAX_SAMPLES];

        private Vector3 m_position = Vector3.zero;
        private Quaternion m_rotation = Quaternion.identity;

        private Vector3 m_frameLinearVelocity = Vector3.zero;
        private Vector3 m_frameAngularVelocity = Vector3.zero;
        private Vector3 m_trackedLinearVelocity = Vector3.zero;
        private Vector3 m_trackedAngularVelocity = Vector3.zero;

        public Vector3 FrameAngularVelocity
        {
            get { return m_frameAngularVelocity; }
        }

        public Vector3 FrameLinearVelocity
        {
            get { return m_frameLinearVelocity; }
        }

        public Vector3 TrackedAngularVelocity
        {
            get { return m_trackedAngularVelocity; }
        }

        public Vector3 TrackedLinearVelocity
        {
            get { return m_trackedLinearVelocity; }
        }

        private void Awake()
        {
            m_position = this.transform.position;
            m_rotation = this.transform.rotation;
        }

        private void FixedUpdate()
        {
            // Compute delta position
            Vector3 finalPosition = this.transform.position;
            Vector3 deltaPosition = finalPosition - m_position;
            m_position = finalPosition;

            // Compute delta rotation
            Quaternion finalRotation = this.transform.rotation;
            Vector3 deltaRotation = DeltaRotation(finalRotation, m_rotation) * Mathf.Deg2Rad;
            m_rotation = finalRotation;

            // Add the sample
            AddSample(deltaPosition, deltaRotation);

            // Update tracked velocities
            m_frameLinearVelocity = m_samples[m_index].LinearVelocity;
            m_frameAngularVelocity = m_samples[m_index].AngularVelocity;
            m_trackedLinearVelocity = ComputeAverageLinearVelocity().normalized * ComputeMaxLinearSpeed();
            m_trackedAngularVelocity = ComputeAverageAngularVelocity();
        }

        private void OnDrawGizmos()
        {
            if (!m_showGizmos)
            {
                return;
            }

            Gizmos.color = Color.red;
            Gizmos.DrawRay(this.transform.position, TrackedLinearVelocity);
        }

        private Vector3 DeltaRotation(Quaternion final, Quaternion initial)
        {
            Vector3 finalEuler = final.eulerAngles;
            Vector3 initialEuler = initial.eulerAngles;
            Vector3 deltaRotation = new Vector3(
                Mathf.DeltaAngle(initialEuler.x, finalEuler.x),
                Mathf.DeltaAngle(initialEuler.y, finalEuler.y),
                Mathf.DeltaAngle(initialEuler.z, finalEuler.z)
            );
            return deltaRotation;
        }

        private void AddSample(Vector3 deltaPosition, Vector3 deltaRotation)
        {
            // Compute the next index and count
            m_index = (m_index + 1) % m_samples.Length;
            m_count = Mathf.Min(m_count + 1, m_samples.Length);

            // Compute sample values
            float sampleTime = Time.time;
            Vector3 sampleLinearVelocity = deltaPosition / Time.deltaTime;
            Vector3 sampleAngularVelocity = deltaRotation / Time.deltaTime;

            // Add the sample
            m_samples[m_index] = new Sample
            {
                Time = sampleTime,
                LinearVelocity = sampleLinearVelocity,
                AngularVelocity = sampleAngularVelocity,
            };
            m_samples[m_index].SquaredLinearSpeed = ComputeAverageLinearVelocity().sqrMagnitude;
        }

        private int Count()
        {
            return Mathf.Min(m_count, m_samples.Length);
        }

        private int IndexPrev(int index)
        {
            return (index == 0) ? m_count - 1 : index - 1;
        }

        private bool IsSampleValid(int index, float windowSize)
        {
            float dt = Time.time - m_samples[index].Time;
            bool isSampleValid = (
                (windowSize - dt >= WINDOW_EPSILON) || // Determine if delta time falls within the time window size
                (index == m_index)                          // Use at least one sample regardless of how much time has elapsed
            );
            return isSampleValid;
        }

        private Vector3 ComputeAverageAngularVelocity()
        {
            int index = m_index;
            int count = Count();

            int velocityCount = 0;
            Vector3 angularVelocity = Vector3.zero;
            for (int i = 0; i < count; ++i)
            {
                // Determine if the sample is valid
                if (!IsSampleValid(index, ANGULAR_VELOCITY_WINDOW))
                {
                    break;
                }

                // Store the velocity
                velocityCount += 1;
                angularVelocity += m_samples[index].AngularVelocity;
                index = IndexPrev(index);
            }

            if (velocityCount > 1)
            {
                // Average the velocity
                angularVelocity /= (float)velocityCount;
            }

            return angularVelocity;
        }

        private Vector3 ComputeAverageLinearVelocity()
        {
            int index = m_index;
            int count = Count();

            int velocityCount = 0;
            Vector3 linearVelocity = Vector3.zero;
            for (int i = 0; i < count; ++i)
            {
                // Determine if the sample is valid
                if (!IsSampleValid(index, LINEAR_VELOCITY_WINDOW))
                {
                    break;
                }

                // Store the velocity
                velocityCount += 1;
                linearVelocity += m_samples[index].LinearVelocity;
                index = IndexPrev(index);
            }

            if (velocityCount > 1)
            {
                // Average the velocity
                linearVelocity /= (float)velocityCount;
            }

            return linearVelocity;
        }

        private float ComputeMaxLinearSpeed()
        {
            int index = m_index;
            int count = Count();

            float maxSpeed = 0.0f;
            for (int i = 0; i < count; ++i)
            {
                if (!IsSampleValid(index, LINEAR_SPEED_WINDOW))
                {
                    break;
                }

                maxSpeed = Mathf.Max(maxSpeed, m_samples[index].SquaredLinearSpeed);
                index = IndexPrev(index);
            }

            return maxSpeed > Mathf.Epsilon ? Mathf.Sqrt(maxSpeed) : 0.0f;
        }
    }
}
