using System.Collections.Generic;

public class ConnectTo : CanditateForConnecting
{
    private Polyline _polyline;
    private Node _node;

    public ConnectTo(Polyline polyline, Node node)
    {
        _polyline = polyline;
        _node = node;
        IsValid = true;
    }

    public ConnectTo() : base()
    { }

    public override void ProcessChoosing(List<Node> detachedNodes, List<Polyline> polylines, Edge connectionEdge)
    {
        detachedNodes.Remove(_node);
        _polyline.ToEnd(_node, connectionEdge);
    }
}
