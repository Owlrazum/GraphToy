using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Orazum.Math;

public class WebOfRopesMaker : MonoBehaviour
{
    [Header("Transforms")]
    [Space]
    [SerializeField]
    private Transform playerTransform;

    [Header("ParametersOfRandomness")]
    [Space]
    [SerializeField]
    private float desiredMinDistanceFromPlayer = 4;

    [SerializeField]
    float radiusOfSpawnArea;

    [SerializeField]
    private GameObject nodePrefab;

    [SerializeField]
    private float timeUntilNextSpawn;

    private List<Node> nodes;

    private List<Rope> ropes;
    private DetachedNodesList detachedNodes;

    private bool isNodeSpawnProcessed;

    private void Start()
    {
        nodes = new List<Node>();
        detachedNodes = new DetachedNodesList();
        ropes = new List<Rope>();

        isNodeSpawnProcessed = true;
        StartCoroutine(Making());
    }

    private IEnumerator Making()
    {
        while (true)
        {
            yield return SpawnNode();
            yield return SpawnNode();

            Detach detachedCanditate = new Detach();
            ConnectTo connectToCanditate = new ConnectTo();
            RopesJoin ropesJoinCanditate = new RopesJoin();

            if (detachedNodes.GetCount() > 1 && ropes.Count < 5)
            {
                detachedCanditate = ComputeDetachedCanditate();
            }

            if (ropes.Count > 0 && detachedNodes.GetCount() > 0)
            {
                connectToCanditate = ComputeConnectToRopeCanditate();
            }

            if (detachedCanditate.IsValid && connectToCanditate.IsValid)
            {
                float rnd = Random.value;
                if (rnd < 0.5f)
                {
                    detachedCanditate.ProcessChoosing(detachedNodes, ropes);
                    //ProcessDetachedCanditateChoosing(detachedCanditate);
                }
                else //if (rnd >= 0.3f && rnd < 0.6f)
                {
                    connectToCanditate.ProcessChoosing(detachedNodes, ropes);
                    //ProcessConnectToCanditateChoosing(connectToCanditate);
                }
            }
            else if (detachedCanditate.IsValid)
            {
                detachedCanditate.ProcessChoosing(detachedNodes, ropes);
                //ProcessDetachedCanditateChoosing(detachedCanditate);
            }
            else if (connectToCanditate.IsValid)
            {
                connectToCanditate.ProcessChoosing(detachedNodes, ropes);
                //ProcessConnectToCanditateChoosing(connectToCanditate);
            }
        }
    }

    private IEnumerator SpawnNode()
    {
        GameObject nodeGb = Instantiate(nodePrefab);
        Node node = nodeGb.GetComponent<Node>();

        Vector2 pos2d = Random.insideUnitCircle * radiusOfSpawnArea;
        Vector3 spawnPos = new Vector3(pos2d.x, 0, pos2d.y);
        nodeGb.transform.position = spawnPos;

        node.gameObject.name = "Node " + nodes.Count;
        node.Index = nodes.Count;
        nodes.Add(node);
        //        Debug.Log(node.Index);
        detachedNodes.AddNode(node);
        isNodeSpawnProcessed = false;

        yield return new WaitForSeconds(timeUntilNextSpawn);
    }

    private Detach ComputeDetachedCanditate()
    {
        int d1, d2;
        if (detachedNodes.GetCount() == 2)
        {
            (d1, d2) = detachedNodes.GetTwoRandomNodes();
            return new Detach((nodes[d1], nodes[d2]));
        }
        int iterationCount = 1000;
        for (int i = 0; i < iterationCount; i++)
        {
            (d1, d2) = detachedNodes.GetTwoRandomNodes();
            if (IsTooSmallDistanceFromPlayer(nodes[d1], nodes[d2]))
            {
                continue;
            }
            return new Detach((nodes[d1], nodes[d2]));
        }
        Debug.Log("Ahhh, anyway, I am tired. Let the distance be less now...");
        return new Detach();
    }

    private ConnectTo ComputeConnectToRopeCanditate()
    {
        for (int i = 0; i < ropes.Count; i++)
        {
            Rope rndRope = GetRandomRope(i);
            for (int j = 0; j < detachedNodes.GetCount(); j++)
            {
                Node rndNode = nodes[detachedNodes.GetRandomNode(j)];
                if (!IsNodeHelpTrapPlayer(rndNode, rndRope))
                {
                    //TODO: probability of trapping;
                    continue;
                }
                if (IsTooSmallDistanceFromPlayer(rndNode, rndRope.End))
                {
                    continue;
                }
                Debug.Log("Computed");
                return new ConnectTo((rndRope, rndNode));
                //rope.CheckIsNodeSameClockOrder(rndNode, ropeTail); TODO: decide whether or not it is needed
            }
        }
        //Debug.LogError("NoConnectToCanditateFound");
        return new ConnectTo();
    }

    private RopesJoin ComputeRopesJoinCanditate()
    {
        Rope r1, r2;
        if (ropes.Count > 2)
        {
            int rndIndex = Random.Range(2, ropes.Count);
            r1 = ropes[rndIndex];
            ropes[rndIndex] = ropes[0];
            ropes[0] = r1;

            rndIndex = Random.Range(2, ropes.Count);
            r2 = ropes[rndIndex];
            ropes[rndIndex] = ropes[1];
            ropes[1] = r2;
        }
        else
        {
            r1 = ropes[0];
            r2 = ropes[1];
        }

        if (r1.ClockOrder == r2.ClockOrder)
        {
            if (IsNodeHelpTrapPlayer(r2.Start, r1))
            {
                if (!IsTooSmallDistanceFromPlayer(r1.End, r2.Start))
                {
                    //return new RopesJoin((r1.end, r2.start));
                }
            }

            if (IsNodeHelpTrapPlayer(r1.Start, r2))
            {
                if (!IsTooSmallDistanceFromPlayer(r1.Start, r2.End))
                {
                    //return (r1.start, r2.end);
                }
            }
        }
        else
        {

        }
        return new RopesJoin();
    }

    /// <summary>
    /// Using clock order of the rope,
    /// determine whether added node
    /// will result in invalid clockOrder for the rope  
    /// </summary>
    /// <param name="node"> </param>
    /// <param name="rope"> Checking as if node was added to the end of the rope </param>
    /// <returns></returns>
    private bool IsNodeHelpTrapPlayer(Node node, Rope rope)
    {
        Vector3 nodeVector = node.transform.position - rope.End.transform.position;
        Vector3 playerVector = playerTransform.position - node.transform.position;
        float crossProdSign = MathUtilities.GetCrossProdSign(nodeVector, playerVector);
        ClockOrderType checkClockOrder = MathUtilities.ConvertSignToClockOrder(crossProdSign);
        ClockOrderType ropeClockOrder = rope.ClockOrder;
        if (!rope.IsClockOrderValid)
        {
            Vector3 ropeVector = rope.End.transform.position - rope.End.ConnectedFrom.transform.position;
            crossProdSign = Mathf.Sign(Vector3.Cross(ropeVector, nodeVector).y);
            ropeClockOrder = MathUtilities.ConvertSignToClockOrder(crossProdSign); // not real
            rope.AssignClockOrder(ropeClockOrder);
        }
        if (checkClockOrder == ropeClockOrder)
        {
            string log = checkClockOrder + " " + ropeClockOrder + " " + rope.End.Index + " " + node.Index;
            //            Debug.Log(log);
            return true;
        }
        return false;
    }

    private bool IsTooSmallDistanceFromPlayer(Node n1, Node n2)
    {
        Vector3 p0 = playerTransform.position;
        Vector3 p1 = n1.transform.position;
        Vector3 p2 = n2.transform.position;
        float dist = MathUtilities.GetPointLineSegmentDistance(p0, p1, p2);
        if (dist < desiredMinDistanceFromPlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Rope GetRandomRope(int startIndex)
    {
        if (ropes.Count == 0)
        {
            Debug.LogError("There are no detached nodes!");
        }
        if (startIndex < 0 && startIndex >= ropes.Count)
        {
            Debug.LogError("Not Valid startIndex for detached nodes!");
        }
        int rnd = Random.Range(startIndex, ropes.Count);
        Rope result = ropes[rnd];
        ropes[rnd] = ropes[startIndex];
        ropes[startIndex] = result;
        return result;
    }
}
