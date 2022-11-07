using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopesJoin : CanditateForConnecting
{
    private (Rope, Rope) data;
    public RopesJoin((Rope, Rope) dataArg)
    {
        data = dataArg;
    }

    public RopesJoin() : base()
    { }

    public override void ProcessChoosing(DetachedNodesList detachedRef, List<Rope> ropesRef)
    {
    }
}
