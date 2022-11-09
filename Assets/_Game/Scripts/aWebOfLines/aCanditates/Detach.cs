using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detach : CanditateForConnecting
{
    private Node _first;
    private Node _second;
    public Detach(Node first, Node second)
    {
        _first = first;
        _second = second;
        IsValid = true;
    }

    public Detach() : base()
    { }

    public override void ProcessChoosing(List<Node> detachedNodes, List<Polyline> polylines, Edge connectionEdge)
    {
        Polyline polyline = new Polyline(_first, _second, connectionEdge);
        polylines.Add(polyline);
        detachedNodes.Remove(_first);
        detachedNodes.Remove(_second);
    }
}
