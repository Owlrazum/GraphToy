using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectTo : CanditateForConnecting
{
    private (Rope, Node) data;

    public ConnectTo((Rope, Node) dataArg)
    {
        data = dataArg;
        IsValid = true;
    }

    public ConnectTo() : base()
    { }

    public override void ProcessChoosing(DetachedNodesList detachedRef, List<Rope> ropesRef)
    {
        (Rope rope, Node node) = data;
        detachedRef.RemoveNode(node);
        rope.ToEnd(node);
    }
}
