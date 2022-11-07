using System.Collections.Generic;

public abstract class CanditateForConnecting
{
    public abstract void ProcessChoosing(DetachedNodesList detachedRef, List<Rope> ropesRef);
    public bool IsValid;
    public CanditateForConnecting()
    {
        IsValid = false;
    }
}
