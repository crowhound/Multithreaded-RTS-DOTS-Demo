using System;

using SF.EntitiesModule;

using UnityEngine;
using UnityEngine.UI;

namespace SF.UI
{
    public class UnitSelectionManagerUI : MonoBehaviour
    {
        /// <summary>
        /// This is the Rect transform of the image component itself. 
        /// Make sure this is the correct UI object with the image component on it.
        /// </summary>
        [SerializeField] private RectTransform _selectionAreaRectTransform;

        private void Start()
        {
            UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
            UnitSelectionManager.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;

            // Make sure we are getting the correct RectTransform. If not, log an error.
            if(_selectionAreaRectTransform == null || !_selectionAreaRectTransform.TryGetComponent(out Image selectionImage))
            {
                Debug.LogError("The RectTransform is either null or the _selectionAreaRectTransform is assigned to it doesn't have an image component. The UnitSelectionManagerUI functionality expects the _selectionAreaRectTransform to have an image component.", gameObject);
                return;
            }

            // Make the selection UI disabled by default.
            _selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            if(_selectionAreaRectTransform != null && _selectionAreaRectTransform.gameObject.activeSelf)
            {
                UpdateVisual();
            }

        }
        private void UnitSelectionManager_OnSelectionAreaStart(object sender, EventArgs e)
        {
            if(_selectionAreaRectTransform == null)
                return;

            // Enable the visual UI selector image when starting a drag selection.
           _selectionAreaRectTransform.gameObject.SetActive(true);
            UpdateVisual();
        }

        private void UnitSelectionManager_OnSelectionAreaEnd(object sender, EventArgs e)
        {
            if(_selectionAreaRectTransform == null)
                return;

            // Disable the visual UI selector image when ending a drag selection.
            _selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void UpdateVisual()
        {
            if(_selectionAreaRectTransform == null)
                return;

            Rect selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();

            _selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y);
            _selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height);
        }
    }
}
