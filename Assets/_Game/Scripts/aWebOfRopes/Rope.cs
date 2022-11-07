using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RopeNamespace;

public class Rope
{
    /// <summary>
    /// Should not happen a case when substractaion is from the rope consisting of two nodes.
    /// It is avoided by always connecting two nodes rope to something, and not disconnecting from it anything in RopeMaker
    ///
    /// Additionally, Node Connection should happen in relation to the ones already included in the rope;
    /// </summary>

    public Node start { get; private set; }
    public Node end { get; private set; }

    public ClockOrderEnum ClockOrder { get; private set; }
    public int nodeCount { get; private set; }

    public Rope(Node start, Node end)
    {
        this.start = start;
        this.end = end;
        this.start.Connect(this.end, RopeTail.End);
        ClockOrder = ClockOrderEnum.None;
        nodeCount = 0;
    }

    public void ToStart(Node node)
    {
        ProcessNodeAddition(node);
        start.Connect(node, RopeTail.Start);
        start = node;
    }

    public void ToEnd(Node node)
    {
        ProcessNodeAddition(node);
        end.Connect(node, RopeTail.End);
        end = node;
    }

    private void ProcessNodeAddition(Node node)
    {
        nodeCount++;
        if (nodeCount == 3) 
        {
            Node first = start;
            Node second = first.ConnectedTo;
            Node third = second.ConnectedTo;
            Vector3 lhs = start.transform.position - second.transform.position;
            Vector3 rhs = second.transform.position - third.transform.position;
            float CrossProdSign = Mathf.Sign(Vector3.Cross(lhs, rhs).y);
            ClockOrder = EnumConverter.ConvertSignToClockOrder(CrossProdSign);
        }
    }

    public void FromStart()
    {
        ProcessNodeSubstraction(start);
        Node newStart = start.ConnectedTo;
        start.Disconnect(RopeTail.Start);
        start = newStart;
    }

    public void FromEnd()
    {
        ProcessNodeSubstraction(end);
        Node newEnd = end.ConnectedFrom;
        end.Disconnect(RopeTail.End);
        end = newEnd;
    }

    private void ProcessNodeSubstraction(Node node)
    {
        nodeCount--;
        if (nodeCount == 2)
        {
            ClockOrder = ClockOrderEnum.None;
        }
    }

    public bool CheckIsNodeSameClockOrder(Node node, RopeTail ropeTail)
    {
        if (ClockOrder == ClockOrderEnum.None)
        {
            Debug.LogError("No ClockOrder for this rope!");
            return false;
        }
        Vector3 first, second, third; 
        if (ropeTail == RopeTail.Start)
        {
            first  = node.transform.position;
            second = start.transform.position;
            third  = start.ConnectedTo.transform.position;
        }
        else
        {
            first  = end.ConnectedFrom.transform.position;
            second = end.transform.position;
            third = node.transform.position;
        }
        Vector3 firstVec = second - first;
        Vector3 secondVec = third - second;
        float checkSign = Mathf.Sign(Vector3.Cross(firstVec, secondVec).y);
        ClockOrderEnum checkClockOrder = EnumConverter.ConvertSignToClockOrder(checkSign); 
        return checkClockOrder == ClockOrder;
    }
}
