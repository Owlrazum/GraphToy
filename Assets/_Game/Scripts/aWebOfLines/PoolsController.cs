using System;
using UnityEngine;
using UnityEngine.Pool;

public class PoolsController : MonoBehaviour
{
    [SerializeField]
    private Node _nodePrefab;

    [SerializeField]
    private Edge _edgePrefab;

    private ObjectPool<Node> _nodes;
    private ObjectPool<Edge> _edges;

    public Node GetNode()
    {
        return _nodes.Get();
    }

    public void ReleaseNode(Node node)
    {
        _nodes.Release(node);
    }

     public Edge GetEdge()
    {
        return _edges.Get();
    }

    public void ReleaseEdge(Edge edge)
    {
        _edges.Release(edge);
    }

    private void Awake()
    {
        _nodes = new ObjectPool<Node>(
            CreateNode,
            OnGetNode,
            OnReleaseNode,
            OnDestroyNode,
            false,
            1000,
            10000
        );

        _edges = new ObjectPool<Edge>(
            CreateEdge,
            OnGetEdge,
            OnReleaseEdge,
            OnDestroyEdge,
            false,
            1000,
            10000
        );
    }

    private Node CreateNode()
    {
        Node node = Instantiate(_nodePrefab);
        node.gameObject.SetActive(false);
        return node;
    }

    private Edge CreateEdge()
    {
        Edge edge = Instantiate(_edgePrefab);
        edge.gameObject.SetActive(false);
        return edge;
    }

    private void OnGetNode(Node node)
    {
        node.gameObject.SetActive(true);
    }

    private void OnGetEdge(Edge edge)
    { 
        edge.gameObject.SetActive(true);
    }

    private void OnReleaseNode(Node node)
    {
        node.gameObject.SetActive(false);
    }

    private void OnReleaseEdge(Edge edge)
    {
        Debug.Log("OnReleaseEdge");
        edge.gameObject.SetActive(false);
    }

    private void OnDestroyNode(Node node)
    {
        Destroy(node.gameObject);
    }

    private void OnDestroyEdge(Edge edge)
    { 
        Destroy(edge.gameObject);
    }
}