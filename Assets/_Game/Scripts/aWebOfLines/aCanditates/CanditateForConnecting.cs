using System.Collections.Generic;

public abstract class CanditateForConnecting
{
    public abstract void ProcessChoosing(List<Node> detachedNodes, List<Polyline> polylines, Edge connectionEdge);
    public bool IsValid;
    public CanditateForConnecting()
    {
        IsValid = false;
    }
}
