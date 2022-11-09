using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Orazum.Math;

public class Polyline
{
    /// <summary>
    /// Should not happen a case when substractaion is from the polyline consisting of two nodes.
    /// It is avoided by always connecting two nodes polyline to something, and not disconnecting from it.
    ///
    /// Additionally, Node Connection should happen in relation to the ones already included in the polyline;
    /// </summary>

    public Node Start { get; private set; }
    public Node End { get; private set; }

    public ClockOrderType ClockOrder { get; private set; }
    public bool IsClockOrderValid { get; private set; }
    public int NodeCount { get; private set; }

    public WebOfRopesMaker Maker { get; set; }

    public Polyline(Node start, Node end, Edge edge)
    {
        this.Start = start;
        this.End = end;
        edge._containingPolyline = this;
        edge.ConnectNodes(start, end, this);
        NodeCount = 2;
    }

    public Node GetPrevEnd()
    {
        return End.FromEdge.Start;
    }

    public void ToStart(Node node, Edge edge)
    {
        edge._containingPolyline = this;
        edge.ConnectNodes(node, Start, this);
        Start = node;
        NodeCount++;
    }

    public void ToEnd(Node node, Edge edge)
    {
        edge._containingPolyline = this;
        edge.ConnectNodes(End, node, this);
        End = node;
        NodeCount++;
    }

    public void AssignClockOrder(ClockOrderType clockOrder)
    {
        ClockOrder = clockOrder;
        IsClockOrderValid = true;
    }

    public void OnPlayerEntered(Edge edge)
    {
        Maker.OnPlayerEnteredEdge(edge);
    }
}
