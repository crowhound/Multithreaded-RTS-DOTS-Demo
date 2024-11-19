using System;

using SF.EntitiesModule;

using UnityEngine;

namespace SF.UI
{
    public class UnitSelectionManagerUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _selectionAreaRectTransform;

        private void Start()
        {
            UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
            UnitSelectionManager.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;
        }

        private void UnitSelectionManager_OnSelectionAreaStart(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UnitSelectionManager_OnSelectionAreaEnd(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
