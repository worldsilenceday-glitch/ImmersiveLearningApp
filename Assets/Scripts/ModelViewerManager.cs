using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Manages the 3D model viewer scene with interactive elements
    /// </summary>
    public class ModelViewerManager : MonoBehaviour
    {
        [Header("Model References")]
        [SerializeField] private GameObject[] models;
        [SerializeField] private Transform modelsParent;
        
        [Header("Camera Controller")]
        [SerializeField] private ModelCameraController cameraController;
        
        [Header("UI References")]
        [SerializeField] private GameObject modelSelectorPanel;
        
        private int currentModelIndex = 0;
        
        private void Start()
        {
            if (models.Length > 0)
            {
                ShowModel(0);
            }
            
            // Initialize camera controller if not set
            if (cameraController == null)
            {
                cameraController = FindObjectOfType<ModelCameraController>();
            }
        }
        
        /// <summary>
        /// Shows the specified model
        /// </summary>
        public void ShowModel(int index)
        {
            if (index < 0 || index >= models.Length) return;
            
            // Hide all models
            foreach (GameObject model in models)
            {
                if (model != null)
                {
                    model.SetActive(false);
                }
            }
            
            // Show the selected model
            if (models[index] != null)
            {
                models[index].SetActive(true);
                
                // Update camera to focus on this model
                if (cameraController != null)
                {
                    cameraController.SetTarget(models[index].transform);
                }
                
                currentModelIndex = index;
            }
        }
        
        /// <summary>
        /// Shows the next model in the list
        /// </summary>
        public void NextModel()
        {
            int nextIndex = (currentModelIndex + 1) % models.Length;
            ShowModel(nextIndex);
        }
        
        /// <summary>
        /// Shows the previous model in the list
        /// </summary>
        public void PreviousModel()
        {
            int prevIndex = (currentModelIndex - 1 + models.Length) % models.Length;
            ShowModel(prevIndex);
        }
        
        /// <summary>
        /// Resets the camera position
        /// </summary>
        public void ResetCamera()
        {
            if (cameraController != null)
            {
                cameraController.ResetCamera();
            }
        }
        
        /// <summary>
        /// Toggles the model selector panel
        /// </summary>
        public void ToggleModelSelector()
        {
            if (modelSelectorPanel != null)
            {
                modelSelectorPanel.SetActive(!modelSelectorPanel.activeSelf);
            }
        }
    }
}