using System.Collections.Generic;
using System.Text;
using UnityEngine;
using TMPro;

public class NumberPadManager : MonoBehaviour
{
    public TextMeshPro currentInputLabel; // shows what's being typed
    public TextMeshPro pendingListLabel;  // shows the list built so far
    public TreeManager treeManager;

    private StringBuilder currentInput = new StringBuilder();
    public List<int> pendingValues = new List<int>();
    public int maxListSize = 10; // matches your tree's max nodes

    public void PressDigit(int digit)
    {
        if (currentInput.Length >= 2) return; // optional: cap at 2 digits (0-99)
        currentInput.Append(digit);
        UpdateDisplay();
        Debug.Log($"Pressed: {digit}");
    }

    public void ClearInput()
    {
        currentInput.Clear();
        UpdateDisplay();
    }

    public void ConfirmNumber()
    {
        if (currentInput.Length == 0) return;
        if (pendingValues.Count >= maxListSize) return; // list full

        int value = int.Parse(currentInput.ToString());
        pendingValues.Add(value);
        currentInput.Clear();
        UpdateDisplay();
    }

    public void BuildTree()
    {
        if (pendingValues.Count == 0) return;
        treeManager.BuildFromList(pendingValues);
    }

    public void ResetAll()
    {
        currentInput.Clear();
        pendingValues.Clear();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (currentInputLabel != null)
            currentInputLabel.text = "Typing: " + currentInput.ToString();

        if (pendingListLabel != null)
            pendingListLabel.text = "List: [" + string.Join(", ", pendingValues) + "]";
    }
}