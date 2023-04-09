using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace YawVR {
    /// <summary>
    /// Cancels the camera's rotation based on IMU data
    /// </summary>
    public class CameraIMUCancellation : MonoBehaviour {
        [SerializeField]
        private Transform cameraOffsetTransform;
        [SerializeField]
        private YawController yawController;



        
        private Vector3 IMU;

        private Vector3 offset;

        private Vector3 rot;
        private void Awake() {  
            rot = transform.eulerAngles;
        }
        public void UpdateOffset() {
            offset.y = IMU.y;
        }
        private void Update() {
            if (YawController.Instance().State == ControllerState.Started ||
                 YawController.Instance().State == ControllerState.Connected) {
                IMU.y = -yawController.Device.ActualPosition.yaw;

                if (cameraOffsetTransform != null) {
                    cameraOffsetTransform.localEulerAngles = IMU - offset;   
                }
            }
        }
    }
}
