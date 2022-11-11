using UnityEngine;

public class Edge : MonoBehaviour
{
    public Polyline _containingPolyline;

    public Node Start { get; private set; }
    public Node End { get; private set; }

    private bool _hasPlayerEntered;

    public void ConnectNodes(Node start, Node end, Polyline containingPolyline)
    {
        _containingPolyline = containingPolyline;

        Start = start;
        End = end;

        Start.ToEdge = this;
        End.FromEdge = this;

        Vector3 startPos = start.transform.position;
        Vector3 endPos = end.transform.position;
        transform.position = startPos;

        Vector3 delta = (endPos - startPos);
        Vector3 scale = transform.localScale;
        scale.z = delta.magnitude;
        transform.localScale = scale;

        Vector3 direction = delta.normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        if (_hasPlayerEntered)
        {
            _containingPolyline.OnPlayerEntered(this);
            return;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            WebOfRopesMaker.EventPlayerEnteredEdge?.Invoke(this);
            if (_containingPolyline == null)
            {
                _hasPlayerEntered = true;
                return;
            }

            // _containingPolyline.OnPlayerEntered(this);
        }
    }
}
