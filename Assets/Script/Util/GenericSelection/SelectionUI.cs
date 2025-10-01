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
        int _sgLastRow = -1;
        bool _sgAllowNextVertical = false;
        int specialGridSnapColumnIndex = 1;
        bool specialGridSnapToColumnOnRowChange = true;
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
            if (items == null || items.Count == 0) return;

            int count = items.Count;
            int curRow = selectedItem / gridWidth;
            int curCol = selectedItem % gridWidth;

            // Deteksi pindah baris (termasuk akibat klik di frame sebelumnya)
            if (_sgLastRow == -1) _sgLastRow = curRow;
            if (curRow != _sgLastRow)
            {
                _sgLastRow = curRow;
                _sgAllowNextVertical = true;                 // ← beri bypass 1x
                OnSpecialVertical?.Invoke(selectedItem, 0);  // ← optional: sinyal reset ke listener
            }

            // --- VERTICAL: pindah dalam baris (±1) KECUALI kalau listener "consume"
            bool wantVertical = Mathf.Abs(v) > 0.2f;
            bool canVertical = (selectionTimer == 0) || _sgAllowNextVertical;

            if (wantVertical && canVertical)
            {
                int dir = (v > 0f) ? +1 : -1; // up=+1, down=-1
                bool consumed = OnSpecialVertical?.Invoke(selectedItem, dir) ?? false;

                if (!consumed)
                {
                    int rowStart = curRow * gridWidth;
                    int rowEndEx = Mathf.Min(rowStart + gridWidth, count);

                    // up → kiri (index-1), down → kanan (index+1)
                    int next = selectedItem + (dir > 0 ? -1 : +1);
                    next = Mathf.Clamp(next, rowStart, rowEndEx - 1);
                    if (next != selectedItem)
                    {
                        selectedItem = next;
                        UpdateSelectionInUI();
                    }
                }

                _sgAllowNextVertical = false;       // ← habiskan bypass
                selectionTimer = 1f / selectionSpeed;
                return;
            }

            // --- HORIZONTAL: pindah antar row (±gridWidth) dan aktifkan bypass vertical
            // --- HORIZONTAL: pindah antar row (±gridWidth) dan aktifkan bypass vertical
            if (selectionTimer == 0 && Mathf.Abs(h) > 0.2f)
            {
                int rows = Mathf.CeilToInt(count / (float)gridWidth);
                int nextRow = Mathf.Clamp(curRow + (h > 0 ? +1 : -1), 0, Mathf.Max(0, rows - 1));

                int rowStart = nextRow * gridWidth;
                int rowEndEx = Mathf.Min(rowStart + gridWidth, count);
                int maxCol = Mathf.Max(0, rowEndEx - rowStart - 1);

                // 👇 snap ke kolom harga ketika ganti row
                int nextCol;
                if (specialGridSnapToColumnOnRowChange)
                {
                    int preferred = Mathf.Clamp(specialGridSnapColumnIndex, 0, maxCol);
                    nextCol = preferred;
                }
                else
                {
                    nextCol = Mathf.Clamp(curCol, 0, maxCol);
                }

                int next = rowStart + nextCol;
                if (next != selectedItem)
                {
                    selectedItem = next;
                    _sgLastRow = nextRow;
                    _sgAllowNextVertical = true;                 // tetap: “Up” langsung bisa
                    OnSpecialVertical?.Invoke(selectedItem, 0);  // reset listener (opsional)
                    UpdateSelectionInUI();
                }

                selectionTimer = 1f / selectionSpeed;
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
