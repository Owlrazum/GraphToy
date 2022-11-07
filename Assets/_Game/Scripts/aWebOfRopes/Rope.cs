using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Orazum.Math;

public class Rope
{
    /// <summary>
    /// Should not happen a case when substractaion is from the rope consisting of two nodes.
    /// It is avoided by always connecting two nodes rope to something, and not disconnecting from it anything in RopeMaker
    ///
    /// Additionally, Node Connection should happen in relation to the ones already included in the rope;
    /// </summary>

    public Node Start { get; private set; }
    public Node End { get; private set; }

    public ClockOrderType ClockOrder { get; private set; }
    public bool IsClockOrderValid { get; private set; }
    public int NodeCount { get; private set; }

    public Rope(Node start, Node end)
    {
        this.Start = start;
        this.End = end;
        this.Start.Connect(this.End, RopeTail.End);
        NodeCount = 0;
    }

    public void ToStart(Node node)
    {
        ProcessNodeAddition(node);
        Start.Connect(node, RopeTail.Start);
        Start = node;
    }

    public void ToEnd(Node node)
    {
        ProcessNodeAddition(node);
        End.Connect(node, RopeTail.End);
        End = node;
    }

    private void ProcessNodeAddition(Node node)
    {
        NodeCount++;
        if (NodeCount == 3) 
        {
            Node first = Start;
            Node second = first.ConnectedTo;
            Node third = second.ConnectedTo;
            Vector3 lhs = Start.transform.position - second.transform.position;
            Vector3 rhs = second.transform.position - third.transform.position;
            float CrossProdSign = Mathf.Sign(Vector3.Cross(lhs, rhs).y);
            ClockOrder = MathUtilities.ConvertSignToClockOrder(CrossProdSign);
        }
    }

    public void AssignClockOrder(ClockOrderType clockOrder)
    {
        ClockOrder = clockOrder;
        IsClockOrderValid = true;
    }

    public void FromStart()
    {
        ProcessNodeSubstraction(Start);
        Node newStart = Start.ConnectedTo;
        Start.Disconnect(RopeTail.Start);
        Start = newStart;
    }

    public void FromEnd()
    {
        ProcessNodeSubstraction(End);
        Node newEnd = End.ConnectedFrom;
        End.Disconnect(RopeTail.End);
        End = newEnd;
    }

    private void ProcessNodeSubstraction(Node node)
    {
        NodeCount--;
        if (NodeCount == 2)
        {
            IsClockOrderValid = false;
        }
    }

    public bool CheckIsNodeSameClockOrder(Node node, RopeTail ropeTail)
    {
        if (!IsClockOrderValid)
        {
            Debug.LogError("No ClockOrder for this rope!");
            return false;
        }
        Vector3 first, second, third; 
        if (ropeTail == RopeTail.Start)
        {
            first  = node.transform.position;
            second = Start.transform.position;
            third  = Start.ConnectedTo.transform.position;
        }
        else
        {
            first  = End.ConnectedFrom.transform.position;
            second = End.transform.position;
            third = node.transform.position;
        }
        Vector3 firstVec = second - first;
        Vector3 secondVec = third - second;
        float checkSign = Mathf.Sign(Vector3.Cross(firstVec, secondVec).y);
        ClockOrderType checkClockOrder = MathUtilities.ConvertSignToClockOrder(checkSign); 
        return checkClockOrder == ClockOrder;
    }
}
