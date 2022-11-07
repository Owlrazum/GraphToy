using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Principality
{
    None,
    Primary,
    Secondary
}

/// <summary>
/// Not exactly circle sector from geometry.
/// In addition, the sector is formed between two cicles
/// with various radiuses with the same center.
/// </summary>
public class CircleSector
{

    public float StartAngle { get; private set; }
    public float EndAngle { get; private set; }

    public CircleSector(float startAngleArg, float endAngleArg)
    {
        StartAngle = startAngleArg;
        EndAngle = endAngleArg;

        VassalityData = (false, null, null);
    }

    /// <summary>
    /// Only relevant in local space of encirlement
    /// </summary>
    private int index;
    public int GetIndex()
    {
        return index;
    }
    public void SetIndex(int indexArg)
    {
        index = indexArg;
    }

    #region Vassality
    /// <summary>
    /// Reason: it is needed for sector to know its adjacent lords. The vassal can be only non occupied sector.
    /// After the sector is occupied, it is not vassal anymore. Vassal is subject to be occupied by the nearbyLord.
    /// Non-vassal still can be occupied.
    /// </summary>
    public
        (bool isVassal,
        OccupatorOfSectors primaryLord,
        OccupatorOfSectors secondaryLord)
        VassalityData
    { get; private set; }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="isVassal"></param>
    /// <param name="primaryLord">It is almost, but not sufficient to know about index of the row in the Encirclement.Occupied</param>
    /// <param name="secondaryLord">There is a case when potential sector is located between two occupied</param>
    public void SetVassality
        (bool isVassal,
        OccupatorOfSectors primaryLord,
        OccupatorOfSectors secondaryLord = null)
    {
        if (isVassal)
        {
            VassalityData = (isVassal, primaryLord, secondaryLord);
        }
        else
        {
            VassalityData = (false, null, null);
        }
    }



    public void ClearPotential()
    {
        SetVassality(false, null);
    }

    public void LordMerged(OccupatorOfSectors newLord, OccupatorOfSectors consumedLord)
    {
        if (VassalityData.primaryLord == consumedLord)
        {
            SetVassality(true, newLord, VassalityData.secondaryLord);
        }
        else if (VassalityData.secondaryLord == consumedLord)
        {
            SetVassality(true, VassalityData.primaryLord, newLord);
        }
        else
        {
            Debug.LogError("Incosistent state of the sector. Merged Lord is not present in its state");
        }
    }

    public Principality GetPrincipalityOfLord(OccupatorOfSectors lord)
    {
        if (VassalityData.primaryLord == lord)
        {
            return Principality.Primary;
        }
        else if (VassalityData.secondaryLord == lord)
        {
            return Principality.Secondary;
        }
        else
        {
            return Principality.None;
        }
    }


    #endregion

    public OccupatorOfSectors OwnerLord { get; set; }

    private Transform occupyingNode;
    public Transform OccupyingNode
    {
        get { return occupyingNode; }
        set { occupyingNode = value; }
    }

    public void DrawDebugLines(Vector3 pos, Color color, float time = 2, int mode = 0)
    {
        if (color == Color.black)
        {
            color = OccupyingNode == null ? Color.red : Color.white;
        }

        if (mode == 0)
        {
            Vector3 endPos = new Vector3(Mathf.Cos(StartAngle), 0, Mathf.Sin(StartAngle)) * 10 + pos;
            Debug.DrawLine(pos, endPos, color, time);
            endPos = new Vector3(Mathf.Cos(EndAngle), 0, Mathf.Sin(EndAngle)) * 10 + pos;
            Debug.DrawLine(pos, endPos, color, time);
        }
        else if (mode == 1)
        {
            float midAngle = (StartAngle + EndAngle) / 2;
            Vector3 endPos = new Vector3(Mathf.Cos(midAngle), 0, Mathf.Sin(midAngle)) * 10 + pos;
            Debug.DrawLine(pos, endPos, color, time);
        }
    }
}
