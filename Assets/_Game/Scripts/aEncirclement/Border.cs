using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : MonoBehaviour
{
    /// <summary>
    /// The goal is to have each border only have two countries
    /// </summary>

    public CircleSector PrimaryStart { get; private set; }
    public CircleSector PrimaryEnd { get; private set; }
    public Encirclement PrimaryCountry { get; private set; }

    public CircleSector SecondaryStart { get; private set; }
    public CircleSector SecondaryEnd { get; private set; }
    public Encirclement SecondaryCountry { get; private set; }

    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("Enter");
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
    //    {
    //        Debug.Log("Player");
    //        PrimaryCountry.ProcessBorderCross(this);
    //        gameObject.SetActive(false);

    //    }
    //}

    public void Initialize(CircleSector primaryStart, CircleSector primaryEnd, Encirclement primaryCountry)
    {
        PrimaryStart = primaryStart.GetIndex() < primaryEnd.GetIndex() ? primaryStart : primaryEnd;
        PrimaryEnd = primaryStart.GetIndex() < primaryEnd.GetIndex() ? primaryEnd : primaryStart;
        PrimaryCountry = primaryCountry;
    }

    public void AddSecondaryCountry(Encirclement secondaryCountry)
    {
        SecondaryCountry = secondaryCountry;
    }

    public void StartGrowth(float growthTime, Vector3 start, Vector3 end)
    {
        StartCoroutine(BorderGrowthAnimation(growthTime, start, end));
    }

    private IEnumerator BorderGrowthAnimation(float growthTime, Vector3 start, Vector3 end)
    {
        Vector3 direction = (end - start).normalized;
        Quaternion newRotation = Quaternion.LookRotation(direction, Vector3.up);
        ModifyRotations(newRotation);

        float scaleDelta = (start - end).magnitude;
        float time = 0;

        Vector3 oldScale = transform.localScale;
        Vector3 newScale = transform.localScale;
        newScale.z = scaleDelta - 0.1f;

        while (time < growthTime)
        {
            float t = time / growthTime;
            Vector3 interpolatedScale = Vector3.Lerp(oldScale, newScale, t);
            ModifyScales(interpolatedScale);
            time += Time.deltaTime;
            yield return null;
        }
        ModifyScales(newScale);
    }


    private void ModifyRotations(Quaternion value)
    {
        transform.localRotation = value;
    }

    private void ModifyScales(Vector3 value)
    {
        transform.localScale = value;
    }

    public void StartFadeOut()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        var renders = GetComponentsInChildren<Renderer>();
        float alpha = 1;
        for (int i = 0; i < 100; i++)
        {
            foreach (Renderer rend in renders)
            {
                Color newColor = rend.material.color;
                newColor.a = alpha;
                rend.material.color = newColor;
            }
            alpha -= 0.01f;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
