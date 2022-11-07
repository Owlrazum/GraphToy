using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RopeNamespace;

public abstract class CanditateForConnecting
{
    public abstract void ProcessChoosing(DetachedNodesList detachedRef, List<Rope> ropesRef);
    public bool IsValid;
    public CanditateForConnecting()
    {
        IsValid = false;
    }
}
