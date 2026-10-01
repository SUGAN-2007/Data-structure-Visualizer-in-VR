using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LinkedListManager : MonoBehaviour
{
    [Header("Data")]
    public List<int> list = new List<int>();
    public int maxSize = 5;

    [Header("Visuals")]
    public GameObject[] cubeVisuals;
    public LineRenderer[] connectors;
    public StatusMessageDisplay statusMessageDisplay;

    [Header("Layout")]
    public float spacing = 0.4f;
    public Vector3 basePosition = new Vector3(-0.8f, 0f, 0f);

    [Header("Cursor")]
    public Color normalColor = Color.blue;
    public Color cursorColor = Color.yellow;
    public GameObject contextMenu; // single shared panel: Insert Before / Insert After / Delete / Go Head / Go Tail
    private int cursorIndex = 0;
private Color[] originalColors;
    private int nextValue = 1;

 public bool isActive = false;


public void ActivateMode()
{
    isActive = true;
    ResetList();
}
public void ResetList()
{
    list.Clear();
    nextValue = 1;
    list.Add(nextValue++);
    list.Add(nextValue++);
    list.Add(nextValue++);
    cursorIndex = 0;
    UpdateVisuals();
}

  private void Awake()
    {
        // cache each cube's starting color once, at scene load
        originalColors = new Color[cubeVisuals.Length];
        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            var renderer = cubeVisuals[i]?.GetComponentInChildren<Renderer>();
            if (renderer != null) originalColors[i] = renderer.material.color;
        }
    }

    public void DeactivateMode()
    {
        isActive = false;
        if (contextMenu != null) contextMenu.SetActive(false);

        // restore every cube's original color so Stack/Queue look correct again
        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            var renderer = cubeVisuals[i]?.GetComponentInChildren<Renderer>();
            if (renderer != null) renderer.material.color = originalColors[i];
        }
    }
    public void MoveRight()
    {
        if (list.Count == 0) return;
        cursorIndex = Mathf.Min(cursorIndex + 1, list.Count - 1);
        UpdateVisuals();
    }

    public void MoveLeft()
    {
        if (list.Count == 0) return;
        cursorIndex = Mathf.Max(cursorIndex - 1, 0);
        UpdateVisuals();
    }

public void MakeHead()
{
    // remove all nodes BEFORE the cursor — cursor's node becomes new head
    if (cursorIndex == 0) return; // already head, nothing to do
    list.RemoveRange(0, cursorIndex);
    cursorIndex = 0;
    UpdateVisuals();
}

public void MakeTail()
{
    // remove all nodes AFTER the cursor — cursor's node becomes new tail
    if (cursorIndex == list.Count - 1) return; // already tail, nothing to do
    int removeCount = list.Count - (cursorIndex + 1);
    list.RemoveRange(cursorIndex + 1, removeCount);
    UpdateVisuals(); // cursorIndex stays same, now correctly at the new last position
}

    public void InsertBeforeCursor()
    {
        if (list.Count >= maxSize) { statusMessageDisplay?.ShowMessage("Overflow! List is full."); return; }
        list.Insert(cursorIndex, nextValue++);
        UpdateVisuals();
    }

    public void InsertAfterCursor()
    {
        if (list.Count >= maxSize) { statusMessageDisplay?.ShowMessage("Overflow! List is full."); return; }
        int insertAt = Mathf.Min(cursorIndex + 1, list.Count);
        list.Insert(insertAt, nextValue++);
        cursorIndex = insertAt;
        UpdateVisuals();
    }

    public void DeleteAtCursor()
    {
        if (list.Count == 0) { statusMessageDisplay?.ShowMessage("Underflow! List is empty."); return; }
        list.RemoveAt(cursorIndex);
        if (cursorIndex >= list.Count) cursorIndex = Mathf.Max(0, list.Count - 1);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
         if (!isActive) return;
        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            if (cubeVisuals[i] == null) continue;
            bool active = i < list.Count;
            cubeVisuals[i].SetActive(active);

            if (active)
            {
                cubeVisuals[i].transform.localPosition = basePosition + new Vector3(spacing * i, 0, 0);
var label = cubeVisuals[i].GetComponentInChildren<TextMeshPro>();
Debug.Log($"Cube {i}: label found = {label != null}");

if (label != null)
{
    string text = list[i].ToString(); // the actual value (1,2,3...)

    if (i == 0 && i == list.Count - 1)
        text += "\n(HEAD/TAIL)"; // single-node case
    else if (i == 0)
        text += "\n(HEAD)";
    else if (i == list.Count - 1)
        text += "\n(TAIL)";

    label.text = text;
}

                var renderer = cubeVisuals[i].GetComponentInChildren<Renderer>();
                if (renderer != null)
                    renderer.material.color = (i == cursorIndex) ? cursorColor : normalColor;
            }
        }
        UpdateConnectors();
        PositionContextMenuAtCursor();
    }

    private void UpdateConnectors()
    {
        if (connectors == null) return;
        for (int i = 0; i < connectors.Length; i++)
        {
            if (connectors[i] == null) continue;
            bool shouldShow = (i + 1) < list.Count;
            connectors[i].gameObject.SetActive(shouldShow);
            if (shouldShow)
            {
                connectors[i].SetPosition(0, cubeVisuals[i].transform.position);
                connectors[i].SetPosition(1, cubeVisuals[i + 1].transform.position);
            }
        }
    }

private void PositionContextMenuAtCursor()
{
    if (!isActive || contextMenu == null || list.Count == 0) return;
    contextMenu.SetActive(true);
    Vector3 cubeWorldPos = cubeVisuals[cursorIndex].transform.position;
    contextMenu.transform.position = cubeWorldPos + Vector3.up * 0.25f;
}
}