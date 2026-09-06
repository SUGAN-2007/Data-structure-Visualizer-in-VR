using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [Header("Stack Settings")]
    [SerializeField] private int maxSize = 5;
    [SerializeField] private GameObject[] cubeVisuals; // drag your 3 (or 5) cubes here, bottom to top

    private Stack<int> stack = new Stack<int>();

    void Start()
    {
        // hide all cubes at start
        foreach (var cube in cubeVisuals)
            cube.SetActive(false);
    }

    public void Push(int value)
    {
        if (stack.Count >= maxSize)
        {
            Debug.Log("Stack Overflow! Stack is full.");
            return;
        }

        stack.Push(value);
        UpdateVisuals();
        Debug.Log("Pushed: " + value);
    }

    public void Pop()
    {
        if (stack.Count == 0)
        {
            Debug.Log("Stack Underflow! Stack is empty.");
            return;
        }

        int value = stack.Pop();
        UpdateVisuals();
        Debug.Log("Popped: " + value);
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            cubeVisuals[i].SetActive(i < stack.Count);
        }
    }
    public void ResetStack()
{
    stack.Clear();
    foreach (var cube in cubeVisuals)
        cube.SetActive(false);
}
}
