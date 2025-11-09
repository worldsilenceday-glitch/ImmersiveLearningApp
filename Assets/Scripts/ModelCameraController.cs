using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Provides smooth camera controls for rotating and zooming 3D models
    /// </summary>
    public class ModelCameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform target; // The model to focus on
        [SerializeField] private float distance = 10.0f;
        [SerializeField] private float minDistance = 3.0f;
        [SerializeField] private float maxDistance = 20.0f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float xSpeed = 200.0f;
        [SerializeField] private float ySpeed = 200.0f;
        [SerializeField] private float zoomSpeed = 20.0f;
        
        [Header("Clamping Settings")]
        [SerializeField] private float yMinLimit = -80f;
        [SerializeField] private float yMaxLimit = 80f;
        
        [Header("Smoothing Settings")]
        [SerializeField] private float rotationSmoothTime = 0.1f;
        [SerializeField] private float zoomSmoothTime = 0.1f;
        
        private float x = 0.0f;
        private float y = 0.0f;
        private float currentDistance;
        private float velocityDistance;
        private Vector3 currentRotation;
        private Vector3 targetRotation;
        private Vector3 velocityRotation;
        
        [Header("Input Settings")]
        [SerializeField] private string horizontalAxis = "Mouse X";
        [SerializeField] private string verticalAxis = "Mouse Y";
        [SerializeField] private string scrollAxis = "Mouse ScrollWheel";
        
        private bool isDragging = false;
        
        private void Start()
        {
            if (target == null)
            {
                target = transform; // Use own transform if no target is set
            }
            
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
            currentDistance = distance;
            currentRotation = new Vector3(y, x, 0);
            targetRotation = currentRotation;
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            // Handle mouse input for rotation
            HandleRotationInput();
            
            // Handle mouse wheel for zooming
            HandleZoomInput();
            
            // Apply smooth transitions
            ApplySmoothTransform();
        }
        
        /// <summary>
        /// Handles mouse drag rotation input
        /// </summary>
        private void HandleRotationInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
            
            if (isDragging)
            {
                float mouseX = Input.GetAxis(horizontalAxis);
                float mouseY = Input.GetAxis(verticalAxis);
                
                x += mouseX * xSpeed * 0.02f;
                y -= mouseY * ySpeed * 0.02f;
                
                // Clamp the vertical rotation
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                
                targetRotation = new Vector3(y, x, 0);
            }
        }
        
        /// <summary>
        /// Handles mouse wheel zoom input
        /// </summary>
        private void HandleZoomInput()
        {
            float scroll = Input.GetAxis(scrollAxis);
            
            if (scroll != 0)
            {
                currentDistance -= scroll * zoomSpeed * 20f;
                currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
                
                // Update distance target for smooth zooming
                distance = currentDistance;
            }
        }
        
        /// <summary>
        /// Applies smooth transitions for rotation and zoom
        /// </summary>
        private void ApplySmoothTransform()
        {
            // Smooth rotation
            currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, 
                ref velocityRotation, rotationSmoothTime);
            
            // Apply rotation to camera
            transform.rotation = Quaternion.Euler(currentRotation);
            
            // Position camera at calculated distance from target
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistance);
            Vector3 position = transform.rotation * negDistance + target.position;
            transform.position = position;
        }
        
        /// <summary>
        /// Clamps angle to min/max values
        /// </summary>
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
        
        /// <summary>
        /// Resets camera to default position
        /// </summary>
        public void ResetCamera()
        {
            x = 0f;
            y = 0f;
            currentDistance = distance;
            targetRotation = new Vector3(y, x, 0);
            
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentDistance);
            Vector3 position = transform.rotation * negDistance + target.position;
            transform.position = position;
            transform.rotation = Quaternion.Euler(targetRotation);
        }
        
        /// <summary>
        /// Sets a new target for the camera to focus on
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}