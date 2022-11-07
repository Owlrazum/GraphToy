using System.Collections;
using UnityEngine;

public class EncirclementMaker : MonoBehaviour
{
    [Header("Transforms")]
    [Space]
    [SerializeField]
    private Transform encirclementTransform;

    [Header("ParametersOfRandomMaking", order = 0)]
    [Space]
    [SerializeField]
    private float minDistOfSpawn;

    [SerializeField]
    private float maxDistOfSpawn;

    [SerializeField]
    private int segmentCount;

    [SerializeField]
    private float heightOffset;
    [Header("TimingParamteres", order = 1)]
    [Space]
    [SerializeField]
    private float rateOfEncirclement;

    [SerializeField, Tooltip("used after player escaped encirclement")]
    private float timeBeforeNewSpawn;

    [SerializeField]
    private float timeBeforeLose;

    [SerializeField]
    private float borderGrowthTime;

    [Header("Prefabs")]
    [Space]
    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private GameObject borderPrefab;

    private void Awake()
    {
    }

    private Encirclement current;

    // Start is called before the first frame update
    void Start()
    {
        current = new Encirclement(encirclementTransform.position, segmentCount);
        StartCoroutine(Making());
    }

    private IEnumerator Making()
    {
        while (true)
        {
            if (current.IsEscapedFrom())
            {
                yield break;
                current = new Encirclement(encirclementTransform.position + encirclementTransform.forward * 15, segmentCount);
                //current.DrawDebugLines(0);
                //TODO new encirclement initialization with existing nodes
            }
            else
            {
                if (current.IsCloseToEncircle())
                {
                    //                    Debug.Log("It is close!");

                    CircleSector lastSector = current.GetRandomSectorAndOccupyIt();
                    //current.DrawDebugLines(0);
                    SpawnNodeInSector(lastSector);
                    ConnectBasedOnEncircData(current.GetRelevantConnectionData());

                    yield return new WaitForSeconds(timeBeforeLose);

                    if (!current.IsEscapedFrom())
                    {
                        Debug.Log("Closing");
                        (CircleSector primary, CircleSector secondary) data = current.GetFinishEncirclementData();
                        Border border = InstantiateBorderModel(data.primary, data.secondary);
                        current.AddBorderToList(border);
                        //current = new Encirclement(current.Pos + new Vector3(75, 0, 0), segmentCount);
                        yield break;
                    }
                    yield return new WaitForSeconds(rateOfEncirclement);
                    continue;
                }
                CircleSector sector = current.GetRandomSectorAndOccupyIt();
                //current.DrawDebugLines(0);
                SpawnNodeInSector(sector);
                ConnectBasedOnEncircData(current.GetRelevantConnectionData());
 
            }
            yield return new WaitForSeconds(rateOfEncirclement);
        }
    }

    private void SpawnNodeInSector(CircleSector sector)
    {
        float rndAngle = Random.Range(sector.StartAngle, sector.EndAngle);
        float rndRadius = Random.Range(minDistOfSpawn, maxDistOfSpawn);
        Vector3 localPos = new Vector3(Mathf.Cos(rndAngle), 0, Mathf.Sin(rndAngle)) * rndRadius;
        Vector3 worldPos = current.Pos + localPos;

        GameObject node = Instantiate(nodePrefab, worldPos, Quaternion.identity);
        sector.OccupyingNode = node.transform;
    }


    private void ConnectBasedOnEncircData(
            (CircleSector newNodeSector,
            CircleSector primarySector,
            CircleSector secondarySector) data)

    {
        if (data.newNodeSector == null)
        {
            return;
        }
        (Border b1, Border b2) result = (null, null);
        result.b1 = InstantiateBorderModel(data.newNodeSector, data.primarySector);
        if (data.secondarySector!= null)
        {
            result.b2 = InstantiateBorderModel(data.newNodeSector, data.secondarySector);
        }

        if (result.b1 != null)
        {
            current.AddBorderToList(result.b1);
        }
        if (result.b2 != null)
        {
            current.AddBorderToList(result.b2);
        }
    }

    private Border InstantiateBorderModel(CircleSector startSector, CircleSector endSector)
    {
        CircleSector tempStart = startSector;
        startSector = startSector.GetIndex() < endSector.GetIndex() ? startSector : endSector;
        endSector = tempStart.GetIndex() < endSector.GetIndex() ? endSector : tempStart;

        Vector3 start = startSector.OccupyingNode.transform.position;
        start.y += heightOffset;
        Vector3 end = endSector.OccupyingNode.transform.position;
        end.y += heightOffset;

        Vector3 middlePos = (start + end) / 2;

        GameObject borderGb = Instantiate(borderPrefab, middlePos, Quaternion.identity);
        //StartCoroutine(BorderGrowthAnimation(borderGb, start, end));

        Border border = borderGb.GetComponentInChildren<Border>();
        border.Initialize(startSector, endSector, current);
        border.StartGrowth(borderGrowthTime, start, end);
        return border;
    }
}
