using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachedNodesList
{
    List<int> nodes;

    public DetachedNodesList()
    {
        nodes = new List<int>();
    }

    public void RemoveNode(Node node)
    {
        nodes.Remove(node.Index);
    }

    public void AddNode(Node node)
    {
        nodes.Add(node.Index);
    }

    public int GetCount()
    {
        return nodes.Count;
    }

    public (int, int) GetTwoRandomNodes()
    {
        if (nodes.Count < 2)
        {
            Debug.LogError("There are no two detached nodes!");
            return (-1, -1);
        }
        if (nodes.Count == 2)
        {
            return (nodes[0], nodes[1]);
        }
            
        int first, second;

        int rnd = Random.Range(2, nodes.Count);
        first = nodes[rnd];
        nodes[rnd] = nodes[0];
        nodes[0] = first;

        rnd = Random.Range(2, nodes.Count);
        second = nodes[rnd];
        nodes[rnd] = nodes[0];
        nodes[0] = second;

        return (first, second);
    }

    /// <summary>
    /// Swaps the elements in random index result and startIndex,
    /// to allow random "enumeration". In other words, each element in this.nodes should be
    /// returned once in the loop invoking this method.
    /// </summary>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public int GetRandomNode(int startIndex)
    {
        if (nodes.Count == 0)
        {
            Debug.LogError("There are no detached nodes!");
        }
        if (startIndex < 0 && startIndex >= nodes.Count)
        {
            Debug.LogError("Not Valid startIndex for detached nodes!");
        }
        int rnd = Random.Range(startIndex, nodes.Count);
        int result = nodes[rnd];
        nodes[rnd] = nodes[startIndex];
        nodes[startIndex] = result;
        return result;
    }
}
