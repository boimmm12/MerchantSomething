using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDE.GenericSelectionUI
{
    public class SelectionUI<T> : MonoBehaviour where T : ISelectableItem
    {
        List<T> items;
        protected int selectedItem = 0;
        float selectionTimer = 0;
        protected int? clickedIndex = null;
        protected bool confirmPressed = false;
        protected bool backPressed = false;

        public event Action<int> OnSelected;
        public event Action OnBack;
        public void SetItems(List<T> items)
        {
            this.items = items;
            items.ForEach(i => i.Init());
            UpdateSelectionInUI();
        }

        public void ClearItems()
        {
            items?.ForEach(i => i.Clear());
            this.items = null;
        }

        public virtual void HandleUpdate()
        {
            UpdateSelectionTimer();
            int prevSelection = selectedItem;

            if (clickedIndex != null)
            {
                selectedItem = clickedIndex.Value;
                clickedIndex = null;
                UpdateSelectionInUI();
            }

            if (confirmPressed)
            {
                confirmPressed = false;
                OnSelected?.Invoke(selectedItem);
            }

            if (backPressed || Input.GetKeyDown(KeyCode.Escape))
            {
                backPressed = false;
                OnBack?.Invoke();
            }
        }

        public virtual void UpdateSelectionInUI()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelectionChanged(i == selectedItem);
            }
        }

        void UpdateSelectionTimer()
        {
            if (selectionTimer > 0)
                selectionTimer = Mathf.Clamp(selectionTimer - Time.deltaTime, 0, selectionTimer);
        }

        public void OnItemClicked(int index)
        {
            clickedIndex = index;
        }

        public void OnConfirmButton()
        {
            confirmPressed = true;
        }

        public void OnBackButton()
        {
            backPressed = true;
        }
    }
}
