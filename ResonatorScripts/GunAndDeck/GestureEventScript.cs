using UnityEngine;
using System.Collections;

namespace Edwon.VR.Gesture
{
    public class GestureEventScript : MonoBehaviour
    {
        [HideInInspector]
        public enum GestureState
        {
            NONE,
            CIRCLE,
            ARC,
            SQUIGGLE_Z
        }
        public GestureState gestureState;

        void OnEnable()
        {
            gestureState = GestureState.NONE;

            VRGestureManager.GestureDetectedEvent += OnGestureDetected;
            VRGestureManager.GestureRejectedEvent += OnGestureRejected;
        }

        void OnDisable()
        {
            gestureState = GestureState.NONE;

            VRGestureManager.GestureDetectedEvent -= OnGestureDetected;
            VRGestureManager.GestureRejectedEvent -= OnGestureRejected;
        }

        void OnGestureDetected(string gestureName, double confidence)
        {
            switch (gestureName)
            {
                case "Circle":
                    gestureState = GestureState.CIRCLE;
                    //play audio for circle
                    Debug.Log("did Circle gesture");
                    break;
                case "Arc":
                    gestureState = GestureState.ARC;
                    //play audio for arc
                    Debug.Log("did Arc gesture");
                    break;
                case "Squiggle Z":
                    gestureState = GestureState.SQUIGGLE_Z;
                    //play audio for squiggle z
                    Debug.Log("did Squiggle Z gesture");
                    break;
            }
        }

        void OnGestureRejected(string error, string gestureName = null, double confidenceValue = 0)
        {
            // log some info about why it was null Debug.Log(nullDebugInfo)
        }
    }

}
