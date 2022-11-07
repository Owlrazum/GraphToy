using System.Collections.Generic;
using UnityEngine;

public class Encirclement
{
    #region SeemsIrrelevant

    public Vector3 Pos { get; private set; }
    private int segmentCount;

    private List<CircleSector> sectors;

    private List<int> freeSectors;
    private List<int> potentialSectors;
    private List<OccupatorOfSectors> occupators;
    private List<Border> borders;

    (bool isRelevant, int OccupSector, int adjOccupSector, int adjOccupSectorSecond) lastOccupationData;
    (int primarySector, int seconarySector) finishEncirclementData;

    public Encirclement(Vector3 posArg, int segmentCountArg, float checkRadius = -1)
    {
        Pos = posArg;
        segmentCount = segmentCountArg;

        sectors = new List<CircleSector>();
        freeSectors = new List<int>();
        potentialSectors = new List<int>();
        occupators = new List<OccupatorOfSectors>();
        borders = new List<Border>();
        lastOccupationData = (false, -1, -1, -1);

        if (checkRadius > 0)
        {
            InitializeWithExistingBorders(checkRadius);
        }
        else
        {
            const float PI = Mathf.PI;
            float angleDelta = 2 * PI / segmentCount;
            for (int i = 0; i < segmentCount; i++)
            {
                float startAngle = i * angleDelta;
                float endAngle = (i + 1) * angleDelta;
                CircleSector sector = new CircleSector(startAngle, endAngle);

                sector.SetIndex(i);
                sectors.Add(sector);
                freeSectors.Add(sector.GetIndex());
            }
        }


    }

    private void InitializeWithExistingBorders(float checkRadius)
    {
        Collider[] nodeColliders = Physics.OverlapSphere(Pos, checkRadius, LayerMask.GetMask("Borders"), QueryTriggerInteraction.Collide);
        if (nodeColliders.Length > 0)
        {
            List<(float, float, Border)> bordersAngles = new List<(float, float, Border)>();
            for (int i = 0; i < nodeColliders.Length; i++)
            {
                Border border = nodeColliders[i].GetComponent<Border>();
                float startAngle = ComputeNodeAngle(border.PrimaryStart.OccupyingNode);
                float endAngle = ComputeNodeAngle(border.PrimaryEnd.OccupyingNode);
                bordersAngles.Add((startAngle, endAngle, border));
            }
            bordersAngles.Sort(CompareByDistance);

            for (int i = 0; i < bordersAngles.Count; i++)
            {
                (float startAngle, float endAndgle, Border border) tBorder = bordersAngles[i];

            }

        }
    }

    private bool IsInsideSectorAngles(float angle, CircleSector sector)
    {
        return angle >= sector.StartAngle && angle < sector.EndAngle;
    }


    public float ComputeNodeAngle(Node node)
    {
        Vector3 localNodePos = node.transform.position - Pos;
        Vector3 fromVector = Vector3.right;
        float angle = Vector3.SignedAngle(fromVector, localNodePos, -Vector3.up) * Mathf.Deg2Rad;
        if (angle < 0)
        {
            angle = Vector3.SignedAngle(new Vector3(-1, 0, 0), localNodePos, -Vector3.up) * Mathf.Deg2Rad;
            angle += Mathf.PI;
        }
        return angle;
    }

    public int CompareByDistance((float, float, Border border) b1, (float, float, Border border) b2)
    {
        Vector3 borderStart = b1.border.PrimaryStart.OccupyingNode.transform.position;
        Vector3 borderEnd = b1.border.PrimaryEnd.OccupyingNode.transform.position;
        float distFirst = CustomMath.GetPointLineSegmentDistance(Pos, borderStart, borderEnd);

        borderStart = b2.border.PrimaryStart.OccupyingNode.transform.position;
        borderEnd = b2.border.PrimaryEnd.OccupyingNode.transform.position;
        float distSecond = CustomMath.GetPointLineSegmentDistance(Pos, borderStart, borderEnd);

        return distFirst.CompareTo(distSecond);
    }

    private bool isEscapedFrom;

    public bool IsEscapedFrom()
    {
        return isEscapedFrom;
    }

    public void SetEscapedFrom(bool arg)
    {
        isEscapedFrom = arg;
    }

    public CircleSector GetRandomSectorAndOccupyIt()
    {
        if (freeSectors.Count > 0 && potentialSectors.Count > 0)
        {
            float rnd = UnityEngine.Random.value;
            if (rnd < 0.5f)
            {
                return GetRandomFreeSectorAndOccupyIt();
            }
            else
            {
                return GetRandomPotentialSectorAndOccupyIt();
            }
        }
        if (freeSectors.Count > 0) // Free Sector
        {
            return GetRandomFreeSectorAndOccupyIt();
        }
        else
        {
            return GetRandomPotentialSectorAndOccupyIt();
        }
    }

    private CircleSector GetRandomFreeSectorAndOccupyIt()
    {
        int rnd = UnityEngine.Random.Range(0, freeSectors.Count);
        CircleSector result = sectors[freeSectors[rnd]];

        OccupyFreeSector(rnd);

        lastOccupationData = (false, -1, -1, -1);
        return result;
    }

    private void OccupyFreeSector(int index)
    {
        CircleSector sector = sectors[freeSectors[index]];

        freeSectors.RemoveAt(index);
        OccupatorOfSectors newLord = new OccupatorOfSectors();
        newLord.Increase(sector);
        occupators.Add(newLord);

        UpdateAdjacentSectorsAfterOccupation(sector.GetIndex(), newLord);
    }

    private CircleSector GetRandomPotentialSectorAndOccupyIt()
    {
        int rnd = UnityEngine.Random.Range(0, potentialSectors.Count);
        CircleSector result = sectors[potentialSectors[rnd]];

        OccupyPotentialSector(rnd);

        return result;
    }

    private void OccupyPotentialSector(int index)
    {
        CircleSector sector = sectors[potentialSectors[index]];

        (_, OccupatorOfSectors primaryLord,
            OccupatorOfSectors secondaryLord) = sector.VassalityData;

        potentialSectors.RemoveAt(index);
        primaryLord.Increase(sector);

        if (secondaryLord == null)
        {
            int adjOccupSectorIndex = UpdateAdjacentSectorsAfterOccupation(sector.GetIndex(), primaryLord);
            lastOccupationData = (true, sector.GetIndex(), adjOccupSectorIndex, -1);
        }
        else
        {
            if (primaryLord != secondaryLord)
            {
                primaryLord.Conquer(secondaryLord, sector);
                occupators.Remove(secondaryLord);
            }
            else
            {
                Debug.Log("Assumption true");
            }

            sector.ClearPotential();
            lastOccupationData = (true, sector.GetIndex(), GetNextIndex(sector.GetIndex()), GetPreviousIndex(sector.GetIndex()));
        }
    }

    private int UpdateAdjacentSectorsAfterOccupation
        (int occupSectorIndex,
         OccupatorOfSectors lord)
    {
        int nextSectorIndex = GetNextIndex(occupSectorIndex);
        int prevSectorIndex = GetPreviousIndex(occupSectorIndex);
        int adjOccupSectorIndex = -1;

        int checksum = 0;

        if (potentialSectors.Contains(nextSectorIndex))
        {
            AddPotentialData(nextSectorIndex, lord);
            adjOccupSectorIndex = prevSectorIndex;
            ModifyVictimsOfLord(lord, sectors[nextSectorIndex]);
            checksum++;
        }
        if (potentialSectors.Contains(prevSectorIndex))
        {
            AddPotentialData(prevSectorIndex, lord);
            adjOccupSectorIndex = nextSectorIndex;
            ModifyVictimsOfLord(lord, sectors[prevSectorIndex]);
            checksum++;
        }
        if (freeSectors.Contains(nextSectorIndex))
        {
            ConvertFreeToPotential(nextSectorIndex, lord);
            adjOccupSectorIndex = prevSectorIndex;
            ModifyVictimsOfLord(lord, sectors[nextSectorIndex]);
            checksum++;
        }
        if (freeSectors.Contains(prevSectorIndex))
        {
            ConvertFreeToPotential(prevSectorIndex, lord);
            adjOccupSectorIndex = nextSectorIndex;
            ModifyVictimsOfLord(lord, sectors[prevSectorIndex]);
            checksum++;
        }

        if (checksum > 2)
        {
            Debug.LogError("Checksum is greater than two. Is everything allright?");
        }
        return adjOccupSectorIndex;
    }

    private void ModifyVictimsOfLord(OccupatorOfSectors lord, CircleSector newVictim)
    {
        (CircleSector v1, CircleSector v2) = lord.GetVictims();
        if (v1 == null)
        {
            lord.ModifyVictims(newVictim, v2);
            //Debug.Log(newVictim.GetIndex() + " " + v2?.GetIndex() + " " + lord.ToString());
        }
        else if (v2 == null)
        {
            lord.ModifyVictims(v1, newVictim);
            //Debug.Log(v1.GetIndex() + " " + newVictim.GetIndex() + " " + lord.ToString());
        }
        else
        {
            if (Mathf.Abs(newVictim.GetIndex() - v1.GetIndex()) < 2)
            {
                lord.ModifyVictims(newVictim, v2);
                //Debug.Log(newVictim.GetIndex() + " " + v2?.GetIndex() + " " + lord.ToString());
            }
            else
            {
                lord.ModifyVictims(v1, newVictim);
                //                Debug.Log(v1.GetIndex() + " " + newVictim.GetIndex() + " " + lord.ToString());
            }
        }

    }

    public bool IsCloseToEncircle()
    {
        return borderCount >= segmentCount - 2;
        //return freeSectors.Count == 0 && potentialSectors.Count <= 1;
    }

    private float borderCount;


    public (CircleSector, CircleSector, CircleSector) GetRelevantConnectionData()
    {
        if (!lastOccupationData.isRelevant)
        {
            return (null, null, null);
        }
        CircleSector occup = sectors[lastOccupationData.OccupSector];
        CircleSector adjOccup = sectors[lastOccupationData.adjOccupSector];

        (
            CircleSector newNodeSector,
            CircleSector primarySector,
            CircleSector secondarySector
        )
        result = (null, null, null);

        result.newNodeSector = occup;
        result.primarySector = adjOccup;
        borderCount++;
        if (lastOccupationData.adjOccupSectorSecond != -1)
        {
            if (!IsCloseToEncircle())
            {
                adjOccup = sectors[lastOccupationData.adjOccupSectorSecond];
                result.secondarySector = adjOccup;
                borderCount++;
            }
            else
            {
                int primaryFinishSector = lastOccupationData.OccupSector;
                int secondaryFinishSector = lastOccupationData.adjOccupSectorSecond;

                finishEncirclementData = (primaryFinishSector, secondaryFinishSector);
            }
        }
        lastOccupationData = (false, -1, -1, -1);
        return result;
    }

    public (CircleSector, CircleSector) GetFinishEncirclementData()
    {
        CircleSector primary = sectors[finishEncirclementData.primarySector];
        CircleSector secondary = sectors[finishEncirclementData.seconarySector];
        return (primary, secondary);
    }

    public void AddBorderToList(Border border)
    {
        borders.Add(border);
    }

    private void ConvertFreeToPotential(int indexOfSector, OccupatorOfSectors primaryLord)
    {
        CircleSector sector = sectors[indexOfSector];

        (bool isSectorPotential, _, _) = sector.VassalityData;
        if (isSectorPotential)
        {
            Debug.LogError("not free sector!");
            return;
        }

        freeSectors.Remove(indexOfSector);
        potentialSectors.Add(indexOfSector);

        sector.SetVassality(true, primaryLord);
    }

    private void AddPotentialData(int indexOfSector, OccupatorOfSectors secondaryLord)
    {
        CircleSector sector = sectors[indexOfSector];

        (bool isSectorPotential,
            OccupatorOfSectors primaryLord, _) = sector.VassalityData;
        if (!isSectorPotential)
        {
            Debug.LogError("not potential sector!");
        }

        sector.SetVassality(true, primaryLord, secondaryLord);
        //        Debug.Log("Secondary vassality " + primaryLord.ToString() + " \n" + secondaryLord.ToString());
    }

    public void DrawDebugLines(int mode)
    {
        if (mode == 0)
        {
            foreach (int i in freeSectors)
            {
                sectors[i].DrawDebugLines(Pos, Color.green, 200, 0);
            }
            foreach (int i in potentialSectors)
            {
                sectors[i].DrawDebugLines(Pos, Color.white, 200, 0);
            }
            foreach (OccupatorOfSectors occ in occupators)
            {
                List<CircleSector> list = occ.GetDebugList();
                foreach (CircleSector s in list)
                {
                    s.DrawDebugLines(Pos, Color.red, 10, 0);
                }
            }
        }
        else if (mode == 1)
        {
            if (lastOccupationData.isRelevant)
            {
                //                Debug.Log("Drawing relevant");
                sectors[lastOccupationData.OccupSector].DrawDebugLines(Pos, Color.black, 10);
                sectors[lastOccupationData.adjOccupSector].DrawDebugLines(Pos, Color.black, 10);
                if (lastOccupationData.adjOccupSectorSecond >= 0)
                {
                    sectors[lastOccupationData.adjOccupSectorSecond].DrawDebugLines(Pos, Color.black, 10);
                }
            }
            else
            {
                DrawDebugLines(0);
            }
        }
    }
    private int GetNextIndex(int index)
    {
        return (index + 1 == segmentCount ? 0 : index + 1);
    }

    private int GetPreviousIndex(int index)
    {
        return (index - 1 == -1 ? segmentCount - 1 : index - 1);
    }


    #endregion
    public void ProcessBorderCross(Border border)
    {
        borderCount--;
        if (!isEscapedFrom)
        {

            isEscapedFrom = true;

            //OccupatorOfSectors lord = border.PrimaryStart.OwnerLord;
            //finishEncirclementData = (-1, -1);
            //OccupatorOfSectors newLord = lord.DivideByBorder(border);
            //occupators.Add(newLord);


            //foreach (Border b in borders)
            //{
            //    b.StartFadeOut();
            //}
        }
    }

    public void UpdatePos(Vector3 pos)
    {
        Pos = pos;
    }
}