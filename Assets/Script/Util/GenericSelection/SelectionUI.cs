using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GDE.GenericSelectionUI
{
    public enum SelectionType { List, Grid, SpecialGrid }
    public class SelectionUI<T> : MonoBehaviour where T : ISelectableItem
    {
        List<T> items;
        protected int selectedItem = 0;
        float selectionTimer = 0;
        protected int? clickedIndex = null;
        protected bool confirmPressed = false;
        protected bool backPressed = false;
        SelectionType selectionType;
        int gridWidth = 2;
        const float selectionSpeed = 5;
        public event Action<int> OnSelected;
        public event Action OnBack;
        public event Func<int, int, bool> OnSpecialVertical;
        public void SetSelectionSettings(SelectionType selectionType, int gridWidth)
        {
            this.selectionType = selectionType;
            this.gridWidth = gridWidth;
        }
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
            if (items == null || items.Count == 0)
                return;

            UpdateSelectionTimer();
            int prevSelection = selectedItem;

            if (clickedIndex != null)
            {
                selectedItem = Mathf.Clamp(clickedIndex.Value, 0, items.Count - 1);
                clickedIndex = null;
            }

            if (selectionType == SelectionType.List)
                HandleListSelection();
            else if (selectionType == SelectionType.SpecialGrid)
                HandleSpecialGridSelection();
            else
                HandleGridSelection();

            selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);

            if (selectedItem != prevSelection)
                UpdateSelectionInUI();

            if (Input.GetKeyDown(KeyCode.E))
                OnSelected?.Invoke(selectedItem);

            if (Input.GetKeyDown(KeyCode.Escape))
                OnBack?.Invoke();
        }
        void HandleListSelection()
        {
            float v = Input.GetAxisRaw("Vertical");

            if (selectionTimer == 0 && Mathf.Abs(v) > 0.2f)
            {
                selectedItem += -(int)Mathf.Sign(v);

                selectionTimer = 1 / selectionSpeed;
            }
        }

        void HandleGridSelection()
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");

            if (selectionTimer == 0 && (Mathf.Abs(v) > 0.2f || Mathf.Abs(h) > 0.2f))
            {
                if (Mathf.Abs(h) > Mathf.Abs(v))
                    selectedItem += (int)Mathf.Sign(h);
                else
                    selectedItem += -(int)Mathf.Sign(v) * gridWidth;


                selectionTimer = 1 / selectionSpeed;
            }
        }
        void HandleSpecialGridSelection()
        {
            float v = Input.GetAxisRaw("Vertical");
            float h = Input.GetAxisRaw("Horizontal");
            if (selectionTimer != 0) return;

            if (items == null || items.Count == 0) return;

            int count = items.Count;
            int curRow = selectedItem / gridWidth;
            int curCol = selectedItem % gridWidth;

            // --- Vertical first: coba biarkan pemakai "consume"
            if (Mathf.Abs(v) > 0.2f)
            {
                int dir = (int)Mathf.Sign(v); // up=+1, down=-1
                bool consumed = OnSpecialVertical?.Invoke(selectedItem, dir) ?? false;

                if (!consumed)
                {
                    // pindah di DALAM baris (±1), jaga agar tetap di row yang sama
                    int rowStart = curRow * gridWidth;
                    int rowEndExclusive = Mathf.Min(rowStart + gridWidth, count);

                    int next = selectedItem + (dir > 0 ? -1 : +1); // up -> ke kiri (index-1), down -> ke kanan (index+1)
                    next = Mathf.Clamp(next, rowStart, rowEndExclusive - 1);
                    selectedItem = next;
                }

                selectionTimer = 1 / selectionSpeed;
                return;
            }

            // --- Horizontal: pindah antar ROW (±gridWidth) sambil mempertahankan kolom bila ada
            if (Mathf.Abs(h) > 0.2f)
            {
                int rows = Mathf.CeilToInt(count / (float)gridWidth);
                int nextRow = Mathf.Clamp(curRow + (int)Mathf.Sign(h), 0, Mathf.Max(0, rows - 1));

                int rowStart = nextRow * gridWidth;
                int rowEndExclusive = Mathf.Min(rowStart + gridWidth, count);
                int maxColInRow = Mathf.Max(0, rowEndExclusive - rowStart - 1);

                int nextCol = Mathf.Clamp(curCol, 0, maxColInRow);
                selectedItem = rowStart + nextCol;

                selectionTimer = 1 / selectionSpeed;
            }
        }
        public virtual void UpdateSelectionInUI()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].OnSelectionChanged(i == selectedItem);
            }
        }

        int WrapIndex(int i, int count)
        {
            if (count <= 0) return 0;
            i %= count;
            if (i < 0) i += count;
            return i;
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
