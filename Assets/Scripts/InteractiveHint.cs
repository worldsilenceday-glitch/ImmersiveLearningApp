using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveLearningApp
{
    /// <summary>
    /// Component that adds interactive hints to 3D models
    /// </summary>
    public class InteractiveHint : MonoBehaviour
    {
        [Header("Hint Information")]
        [TextArea(3, 10)]
        [SerializeField] private List<string> hints = new List<string>();
        
        [Header("Visual Settings")]
        [SerializeField] private bool highlightOnHover = true;
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField] private float highlightIntensity = 1.5f;
        
        private List<Renderer> renderers;
        private Color[] originalColors;
        
        private void Start()
        {
            // Cache all renderers on this object and children
            renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
            
            // Store original material colors
            originalColors = new Color[renderers.Count];
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                {
                    originalColors[i] = renderers[i].material.color;
                }
            }
        }
        
        /// <summary>
        /// Gets the list of hints for this object
        /// </summary>
        public List<string> GetHints()
        {
            return hints;
        }
        
        /// <summary>
        /// Highlights the model when hovered over
        /// </summary>
        public void Highlight()
        {
            if (!highlightOnHover) return;
            
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.HasProperty("_Color"))
                {
                    renderers[i].material.color = highlightColor * highlightIntensity;
                }
            }
        }
        
        /// <summary>
        /// Removes highlight effect
        /// </summary>
        public void RemoveHighlight()
        {
            if (!highlightOnHover) return;
            
            for (int i = 0; i < renderers.Count; i++)
            {
                if (i < originalColors.Length && renderers[i].material.HasProperty("_Color"))
                {
                    renderers[i].material.color = originalColors[i];
                }
            }
        }
        
        /// <summary>
        /// Adds a new hint to this object
        /// </summary>
        public void AddHint(string hint)
        {
            hints.Add(hint);
        }
        
        /// <summary>
        /// Sets all hints for this object
        /// </summary>
        public void SetHints(List<string> newHints)
        {
            hints = newHints;
        }
        
        #if UNITY_EDITOR
        private void OnValidate()
        {
            // Ensure we have a default hint if the list is empty
            if (hints.Count == 0)
            {
                hints.Add("Default hint for " + gameObject.name);
            }
        }
        #endif
    }
}