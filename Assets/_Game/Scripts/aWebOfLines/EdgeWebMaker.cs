using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

using Orazum.Math;

[RequireComponent(typeof(PoolsController))]
public class EdgeWebMaker : MonoBehaviour
{
    public static Action<Edge> EventPlayerEnteredEdge;
    [Header("SpawnParams")]
    [Space]
    [SerializeField]
    private float _desiredMinDistanceFromPlayer = 4;

    [SerializeField]
    private float _radiusOfSpawnArea = 10;

    [SerializeField]
    [Tooltip("Per second")]
    private float _nodeSpawnRate = 100;

    private float _timeFromLastNodeSpawn;
    private float _timeForNodeSpawn;

    private PoolsController _poolsController;

    private List<Node> _detachedNodes;
    private List<Polyline> _polylines;


    private Transform _playerTransform;

    private void Awake()
    {
        TryGetComponent(out _poolsController);

        _detachedNodes = new List<Node>(1000);
        _polylines = new List<Polyline>(5);

        _timeFromLastNodeSpawn = 0;
        _timeForNodeSpawn = 1 / _nodeSpawnRate;

        EventPlayerEnteredEdge += OnPlayerEnteredEdge;
    }

    private void OnDestroy()
    {
        EventPlayerEnteredEdge -= OnPlayerEnteredEdge;
    }

    private void Start()
    {
        _playerTransform = Player.FuncGetTransform();
    }

    private void Update()
    {
        _timeFromLastNodeSpawn += Time.deltaTime;
        if (_timeFromLastNodeSpawn < _timeForNodeSpawn)
        {
            return;
        }

        int amount = (int)(_timeFromLastNodeSpawn / _timeForNodeSpawn);
        _timeFromLastNodeSpawn -= amount * _timeForNodeSpawn;
        SpawnNodes(amount);

        CreateNewEdgeConnection();
    }

    private void CreateNewEdgeConnection()
    {
        Edge newEdge = _poolsController.GetEdge();

        Detach detachedCanditate = new Detach();
        ConnectTo connectToCanditate = new ConnectTo();

        if (_detachedNodes.Count > 1 && _polylines.Count < 5)
        {
            detachedCanditate = ComputeDetachedCanditate();
        }

        if (_polylines.Count > 0 && _detachedNodes.Count > 0)
        {
            connectToCanditate = ComputeConnectToRopeCanditate();
        }

        if (detachedCanditate.IsValid && connectToCanditate.IsValid)
        {
            float rnd = UnityEngine.Random.value;
            if (rnd < 0.5f)
            {
                detachedCanditate.ProcessChoosing(_detachedNodes, _polylines, newEdge);
                //ProcessDetachedCanditateChoosing(detachedCanditate);
            }
            else //if (rnd >= 0.3f && rnd < 0.6f)
            {
                connectToCanditate.ProcessChoosing(_detachedNodes, _polylines, newEdge);
                //ProcessConnectToCanditateChoosing(connectToCanditate);
            }
        }
        else if (detachedCanditate.IsValid)
        {
            detachedCanditate.ProcessChoosing(_detachedNodes, _polylines, newEdge);
            //ProcessDetachedCanditateChoosing(detachedCanditate);
        }
        else if (connectToCanditate.IsValid)
        {
            connectToCanditate.ProcessChoosing(_detachedNodes, _polylines, newEdge);
            //ProcessConnectToCanditateChoosing(connectToCanditate);
        }
    }

    private void SpawnNodes(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Node node = _poolsController.GetNode();

            Vector2 pos2d = UnityEngine.Random.insideUnitCircle * _radiusOfSpawnArea;
            Vector3 spawnPos = new Vector3(pos2d.x, 0, pos2d.y);
            node.transform.position = spawnPos;

            _detachedNodes.Add(node);
        }
    }

    private Detach ComputeDetachedCanditate()
    {
        Node d1, d2;
        if (_detachedNodes.Count == 2)
        {
            (d1, d2) = _detachedNodes.GetTwoRandom();
            return new Detach(d1, d2);
        }
        int iterationCount = 1000;
        for (int i = 0; i < iterationCount; i++)
        {
            (d1, d2) = _detachedNodes.GetTwoRandom();
            if (IsTooSmallDistanceFromPlayer(d1, d2))
            {
                continue;
            }
            return new Detach(d1, d2);
        }
        Debug.Log("Ahhh, anyway, I am tired. Let the distance be less now...");
        return new Detach();
    }

    private ConnectTo ComputeConnectToRopeCanditate()
    {
        for (int i = 0; i < _polylines.Count; i++)
        {
            Polyline rndPolyline = GetRandomRope(i);
            for (int j = 0; j < _detachedNodes.Count; j++)
            {
                Node rndNode = _detachedNodes.GetRandomWithSwap(j);
                if (!IsNodeHelpTrapPlayer(rndNode, rndPolyline))
                {
                    //TODO: probability of trapping;
                    continue;
                }
                if (IsTooSmallDistanceFromPlayer(rndNode, rndPolyline.End))
                {
                    continue;
                }
                return new ConnectTo(rndPolyline, rndNode);
            }
        }
        return new ConnectTo();
    }

    /// <summary>
    /// Using clock order of the rope,
    /// determine whether added node
    /// will result in invalid clockOrder for the rope  
    /// </summary>
    /// <param name="node"> </param>
    /// <param name="polyline"> Checking as if node was added to the end of the rope </param>
    /// <returns></returns>
    private bool IsNodeHelpTrapPlayer(Node node, Polyline polyline)
    {
        Vector3 nodeVector = node.transform.position - polyline.End.transform.position;
        Vector3 playerVector = _playerTransform.position - node.transform.position;
        float crossProdSign = MathUtilities.GetCrossProdSign(nodeVector, playerVector);
        ClockOrderType checkClockOrder = MathUtilities.ConvertSignToClockOrder(crossProdSign);
        ClockOrderType ropeClockOrder = polyline.ClockOrder;
        if (!polyline.IsClockOrderValid)
        {
            Vector3 ropeVector = polyline.End.transform.position - polyline.GetPrevEnd().transform.position;
            crossProdSign = Mathf.Sign(Vector3.Cross(ropeVector, nodeVector).y);
            ropeClockOrder = MathUtilities.ConvertSignToClockOrder(crossProdSign); // not real
            polyline.AssignClockOrder(ropeClockOrder);
        }
        if (checkClockOrder == ropeClockOrder)
        {
            return true;
        }
        return false;
    }

    private bool IsTooSmallDistanceFromPlayer(Node n1, Node n2)
    {
        Vector3 p0 = _playerTransform.position;
        Vector3 p1 = n1.transform.position;
        Vector3 p2 = n2.transform.position;
        float dist = MathUtilities.GetPointLineSegmentDistance(p0, p1, p2);
        if (dist < _desiredMinDistanceFromPlayer)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Polyline GetRandomRope(int startIndex)
    {
        if (_polylines.Count == 0)
        {
            Debug.LogError("There are no detached nodes!");
        }
        if (startIndex < 0 && startIndex >= _polylines.Count)
        {
            Debug.LogError("Not Valid startIndex for detached nodes!");
        }
        int rnd = UnityEngine.Random.Range(startIndex, _polylines.Count);
        Polyline result = _polylines[rnd];
        _polylines[rnd] = _polylines[startIndex];
        _polylines[startIndex] = result;
        return result;
    }

    private void OnPlayerEnteredEdge(Edge edge)
    {
        _poolsController.ReleaseEdge(edge);
    }
}
