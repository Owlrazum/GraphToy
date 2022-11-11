using System.Collections;
using UnityEngine;
using static Orazum.Math.EasingUtilities;

public class Edge : MonoBehaviour
{
    [SerializeField]
    private float _scaleLerpSpeed;

    public Polyline _containingPolyline;

    public Node Start { get; private set; }
    public Node End { get; private set; }

    private bool _hasPlayerEntered;
    private IEnumerator _scaleAnimationCoroutine;

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
        Vector3 direction = delta.normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        if (_hasPlayerEntered)
        {
            _containingPolyline.OnPlayerEntered(this);
            return;
        }

        _scaleAnimationCoroutine = ScaleAnimation(delta.magnitude);
        StartCoroutine(_scaleAnimationCoroutine);
    }

    private IEnumerator ScaleAnimation(float targetScale)
    {
        Vector3 scale = transform.localScale;
        float lerpParam = 0;
        while (lerpParam < 1)
        {
            lerpParam += _scaleLerpSpeed * Time.deltaTime;
            scale.z = Mathf.Lerp(0, targetScale, EaseOut(lerpParam));
            transform.localScale = scale;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            EdgeWebMaker.EventPlayerEnteredEdge?.Invoke(this);
            if (_containingPolyline == null)
            {
                _hasPlayerEntered = true;
                return;
            }

            if (_scaleAnimationCoroutine != null)
            { 
                StopCoroutine(_scaleAnimationCoroutine);
                _scaleAnimationCoroutine = null;
            }
        }
    }
}
