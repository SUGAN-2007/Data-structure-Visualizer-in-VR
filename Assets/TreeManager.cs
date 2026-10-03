using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TreeManager : MonoBehaviour
{
    private class Node
    {
        public int value;
        public Node left, right, parent;
    }

    [Header("Visuals")]
    public GameObject[] cubeVisuals;
    public LineRenderer[] connectors;
    public StatusMessageDisplay statusMessageDisplay;

    [Header("Layout")]
    public Vector3 basePosition = new Vector3(0f, 0.6f, 0f);
    public float xSpacing = 0.45f;
    public float levelHeight = 0.4f;

    [Header("Cursor")]
    public bool isActive = false;
    public int maxNodes = 10;

    private Node root;

    private List<Node> displayOrder = new List<Node>();
    private Dictionary<Node, int> nodeToCardIndex = new Dictionary<Node, int>();
    private Dictionary<Node, Vector3> nodePosition = new Dictionary<Node, Vector3>();
    private int inOrderCounter;

    private void Awake()
    {

    }

    private void Start()
    {
        ActivateMode();
    }

    public void ActivateMode()
    {
        isActive = true;
        ClearTree();
    }

    public void DeactivateMode()
    {
        isActive = false;

    }

    public void ClearTree()
    {
        root = null;
        UpdateVisuals();
    }

    public void BuildFromList(List<int> values)
    {
        root = null;

        int count = 0;
        foreach (int value in values)
        {
            if (count >= maxNodes)
            {
                statusMessageDisplay?.ShowMessage("Overflow! Only first " + maxNodes + " values used.");
                break;
            }
            InsertBST(value);
            count++;
        }

        UpdateVisuals();
    }

    private void InsertBST(int value)
    {
        if (root == null)
        {
            root = new Node { value = value };
            return;
        }

        Node current = root;
        while (true)
        {
            if (value < current.value)
            {
                if (current.left == null) { current.left = new Node { value = value, parent = current }; return; }
                current = current.left;
            }
            else
            {
                if (current.right == null) { current.right = new Node { value = value, parent = current }; return; }
                current = current.right;
            }
        }
    }

    public void DeleteNode(int cardIndex)
    {
        if (cardIndex >= displayOrder.Count) return;
        Node target = displayOrder[cardIndex];

        if (target.left != null || target.right != null)
        {
            statusMessageDisplay?.ShowMessage("Delete children first (leaf nodes only).");
            return;
        }

        Node parent = target.parent;
        if (parent == null) root = null;
        else if (parent.left == target) parent.left = null;
        else if (parent.right == target) parent.right = null;

        UpdateVisuals();
    }

private void AssignPositions(Node node, int depth)
{
    if (node == null) return;

    int totalNodes = CountNodes(root);
    float scaledXSpacing = GetScaledSpacing(xSpacing, totalNodes);
    float scaledLevelHeight = GetScaledSpacing(levelHeight, totalNodes);

    AssignPositions(node.left, depth + 1);

    displayOrder.Add(node);
    float x = (inOrderCounter - (totalNodes - 1) / 2f) * scaledXSpacing;
    float y = -depth * scaledLevelHeight;
    nodePosition[node] = basePosition + new Vector3(x, y, 0);
    inOrderCounter++;

    AssignPositions(node.right, depth + 1);
}
    private int CountNodes(Node node)
    {
        if (node == null) return 0;
        return 1 + CountNodes(node.left) + CountNodes(node.right);
    }

    public void UpdateVisuals()
    {
        if (!isActive) return;

        displayOrder.Clear();
        nodeToCardIndex.Clear();
        nodePosition.Clear();
        inOrderCounter = 0;

        if (root != null)
        {
            AssignPositions(root, 0);

            float rootX = nodePosition[root].x - basePosition.x;
            var keys = new List<Node>(nodePosition.Keys);
            foreach (var key in keys)
            {
                Vector3 p = nodePosition[key];
                p.x -= rootX;
                nodePosition[key] = p;
            }
        }

        for (int i = 0; i < cubeVisuals.Length; i++)
        {
            if (cubeVisuals[i] == null) continue;

            if (i < displayOrder.Count)
            {
                Node node = displayOrder[i];
                nodeToCardIndex[node] = i;

                cubeVisuals[i].SetActive(true);
                cubeVisuals[i].transform.localPosition = nodePosition[node];

                var label = cubeVisuals[i].GetComponentInChildren<TextMeshPro>(true);
                if (label != null)
                {
                    string text = node.value.ToString();
                    if (node == root) text += "\n(ROOT)";
                    label.text = text;
                }


            }
            else
            {
                cubeVisuals[i].SetActive(false);
            }
        }

        UpdateConnectors();
    }

    private void UpdateConnectors()
    {
        if (connectors == null) return;

        int connectorIndex = 0;
        foreach (var node in displayOrder)
        {
            if (node.parent == null) continue;
            if (connectorIndex >= connectors.Length) break;

            var connector = connectors[connectorIndex];
            if (connector == null) { connectorIndex++; continue; }

            bool parentHasCard = nodeToCardIndex.ContainsKey(node.parent);
            bool selfHasCard = nodeToCardIndex.ContainsKey(node);

            if (parentHasCard && selfHasCard)
            {
                connector.gameObject.SetActive(true);
                connector.SetPosition(0, cubeVisuals[nodeToCardIndex[node.parent]].transform.position);
                connector.SetPosition(1, cubeVisuals[nodeToCardIndex[node]].transform.position);
            }
            else
            {
                connector.gameObject.SetActive(false);
            }
            connectorIndex++;
        }

        for (int i = connectorIndex; i < connectors.Length; i++)
        {
            if (connectors[i] != null) connectors[i].gameObject.SetActive(false);
        }
    }
    private float GetScaledSpacing(float baseValue, int nodeCount)
{
    if (nodeCount <= 3) return baseValue;
    float scale = 3f / nodeCount; // shrinks as more nodes exist
    return Mathf.Max(baseValue * scale, baseValue * 0.35f); // don't shrink below 35%
}
private int GetMaxDepth(Node node, int depth = 0)
{
    if (node == null) return depth - 1;
    return Mathf.Max(GetMaxDepth(node.left, depth + 1), GetMaxDepth(node.right, depth + 1));
}
}