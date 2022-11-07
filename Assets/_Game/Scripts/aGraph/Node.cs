using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RopeTail
{
    Start,
    End
}

public class Node : MonoBehaviour
{
    /// <summary>
    /// In Encirclements serves as a gameobject with name without any methods.
    /// 
    /// In Ropes serves as a whole class.
    /// 
    /// Node Connection/Disconnection happens in relative to those already included in the rope
    /// (may not be true to change in case of additional types of structures)
    /// </summary>

    [SerializeField]
    GameObject linePrefab;

    public int Index { get; set; }
    public Node ConnectedTo { get; private set; }
    public Node ConnectedFrom { get; private set; }

    private GameObject rope;

    public void Connect(Node node, RopeTail ropeTail = RopeTail.End)
    {
        if (ropeTail == RopeTail.Start)
        {
            ConnectedFrom = node;
            node.ConnectedTo = this;
        }
        else
        {
            ConnectedTo = node;
            node.ConnectedFrom = this;
        }
        InstantiateRopeModel();
    }

    public void Disconnect(RopeTail ropeTail)
    {
        if (ropeTail == RopeTail.Start)
        {
            ConnectedTo.ConnectedFrom = null;
            ConnectedTo = null;
        }
        else
        {
            ConnectedFrom.ConnectedTo = null;
            ConnectedFrom = null;
        }
        rope.SetActive(false); // TODO Rope tearing
    }

    private void InstantiateRopeModel()
    {
        rope = Instantiate(linePrefab, transform);
        float delta = (transform.position - ConnectedTo.transform.position).magnitude;
        Vector3 newScale = rope.transform.localScale;
        newScale.z = delta / 10;
        rope.transform.localScale = newScale;

        Vector3 direction = ConnectedTo.transform.position - transform.position;
        rope.transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    public void Highlight()
    {
        StartCoroutine(Highlighting());
    }

    private IEnumerator Highlighting()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        rend.material.color = Color.red;
        yield return new WaitForSeconds(5);
        rend.material.color = Color.white;
    }
}
