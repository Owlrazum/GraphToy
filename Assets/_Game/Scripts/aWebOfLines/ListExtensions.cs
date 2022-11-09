using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static (Node, Node) GetTwoRandom(this List<Node> list)
    {
        if (list.Count < 2)
        {
            Debug.LogError("There are no two detached nodes!");
            return (null, null);
        }
        if (list.Count == 2)
        {
            return (list[0], list[1]);
        }
            
        Node first, second;

        int rnd = Random.Range(2, list.Count);
        first = list[rnd];
        list[rnd] = list[0];
        list[0] = first;

        rnd = Random.Range(2, list.Count);
        second = list[rnd];
        list[rnd] = list[0];
        list[0] = second;

        return (first, second);
    }

    /// <summary>
    /// Swaps the elements in random index result and startIndex,
    /// to allow random "enumeration". In other words, each element in this.nodes should be
    /// returned once in the loop invoking this method.
    /// </summary>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    public static Node GetRandomWithSwap(this List<Node> nodes, int startIndex)
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
        Node result = nodes[rnd];
        nodes[rnd] = nodes[startIndex];
        nodes[startIndex] = result;
        return result;
    }
}
