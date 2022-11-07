using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The main reason of this class is to make easier register cases when on two occupation sector sequences combine.
/// Worthy to note, each potential sector stores a reference to the sectors of the sequence.
/// When a sector referenced sequence that was merged, in other words destroyed, its reference becomes invalid.
/// </summary>
public class OccupatorOfSectors
{

    private List<CircleSector> occupiedSectors;
    private (CircleSector primaryNeighbour, CircleSector secondaryNeighbour) victims;

    public OccupatorOfSectors()
    {
        occupiedSectors = new List<CircleSector>();
        victims.primaryNeighbour = null;
        victims.secondaryNeighbour = null;
    }

    public void Increase(CircleSector sector)
    {
        if (occupiedSectors.Contains(sector))
        {
            return;
        }
        occupiedSectors.Add(sector);
        sector.OwnerLord = this;
    }

    public void ModifyVictims(CircleSector primaryNeighbour, CircleSector secondaryNeighbour)
    {
        victims.primaryNeighbour = primaryNeighbour;
        victims.secondaryNeighbour = secondaryNeighbour;
    }

    public (CircleSector, CircleSector) GetVictims()
    {
        return victims;
    }

    /// <summary>
    /// In other words merge two sequences of occupied sectors. 
    /// </summary>
    /// <param name="other"></param>
    /// <param name="pointOfConquest">
    /// The point which was occupied so the conquest became possible. It should be between two lords
    /// </param>
    public void Conquer(OccupatorOfSectors other, CircleSector pointOfConquest)
    {
        foreach (CircleSector s in other.occupiedSectors)
        {
            s.OwnerLord = this;
        }
        occupiedSectors.AddRange(other.occupiedSectors);

        (CircleSector v1, CircleSector v2) = other.GetVictims();
        if (v1 == null)
        {
            Debug.LogError("v1 NULL " + pointOfConquest.GetIndex());
        }

        if (v2 == null)
        {
            Debug.LogError("v2 NULL" + pointOfConquest.GetIndex());
        }

        CircleSector newVictim = null;

        if (v1 == pointOfConquest)
        {
            v1.LordMerged(this, other);
            newVictim = v2;
        }
        else if (v2 == pointOfConquest)
        {
            v2.LordMerged(this, other);
            newVictim = v1;
        }
        else
        {
            Debug.Log(v1.GetIndex() + " " + v2.GetIndex() + " " + pointOfConquest.GetIndex());
            Debug.LogError("Incosistent state of occupator: no victim is equal to the occupied sector");
        }

        if (victims.primaryNeighbour == pointOfConquest)
        {
            ModifyVictims(newVictim, victims.secondaryNeighbour);
 //           Debug.Log(newVictim.GetIndex() + " " + victims.secondaryNeighbour.GetIndex());
        }
        else if (victims.secondaryNeighbour == pointOfConquest)
        {
            ModifyVictims(victims.primaryNeighbour, newVictim);
//            Debug.Log(victims.primaryNeighbour.GetIndex() + " " + newVictim.GetIndex());
        }
        else
        {
           // Debug.LogError("Failed to keep consistent state");
        }

    }

    public OccupatorOfSectors DivideByBorder(Border border)
    {
        CircleSector borderEnd = border.PrimaryEnd;

        ByIndexComparer comparer = new ByIndexComparer();

        occupiedSectors.Sort(comparer);
        int startIndexSecondary = occupiedSectors.BinarySearch(borderEnd, comparer);
        if (startIndexSecondary < 0)
        {
            Debug.LogError("No start index found, invalid borderEnd");
            return null;
        }

        int secondaryCount = occupiedSectors.Count - startIndexSecondary;
        Debug.Log("Overall Count: " + occupiedSectors.Count + "StartIndex: " + startIndexSecondary + " count: " + secondaryCount);
        CircleSector[] copyArray = new CircleSector[secondaryCount];

        /* 0 1 2 3 Count 4 start 3 secCount 1
         * 3
         * 
         
         */

        occupiedSectors.CopyTo(startIndexSecondary, copyArray, 0, secondaryCount);
        occupiedSectors.RemoveRange(startIndexSecondary, secondaryCount);

        OccupatorOfSectors secondaryLord = new OccupatorOfSectors();

        List<CircleSector> secondaryOccupiedSectors = new List<CircleSector>(copyArray);
        foreach(CircleSector s in secondaryOccupiedSectors)
        {
            Debug.Log(s?.GetIndex());
            secondaryLord.Increase(s);
        }

        return secondaryLord;
    }

    public List<CircleSector> GetDebugList()
    {
        return occupiedSectors;
    }

    public override string ToString()
    {
        string message = "Lord";
        foreach (CircleSector i in occupiedSectors)
        {
            message += i.GetIndex() + " ";
        }
        return message;
    }
}

public class ByIndexComparer : IComparer<CircleSector>
{
    public int Compare(CircleSector s1, CircleSector s2)
    {
        if (s1 == null)
        {
            Debug.LogError("S1 null");
        }
        if (s2 == null)
        {
            Debug.LogError("S2 null");
        }
        return s1.GetIndex().CompareTo(s2.GetIndex());
    }
}