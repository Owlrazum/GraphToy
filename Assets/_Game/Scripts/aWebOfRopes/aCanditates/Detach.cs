using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detach : CanditateForConnecting
{
    private (Node, Node) data;
    public Detach((Node, Node) dataArg)
    {
        data = dataArg;
        IsValid = true;
    }

    public Detach() : base()
    { }

    public override void ProcessChoosing(DetachedNodesList detachedRef, List<Rope> ropesRef)
    {
        (Node n1, Node n2) = data;
        Rope rope = new Rope(n1, n2);
        ropesRef.Add(rope);
        detachedRef.RemoveNode(n1);
        detachedRef.RemoveNode(n2);
        n1.Connect(n2);
    }
}
