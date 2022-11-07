using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class BotSnake : MonoBehaviour
{
    [Header("DirectionRateSettings")]
    [Space]
    [SerializeField, Tooltip("minimum should be greater than 1")]
    private Vector2 dirSwapIntervalRange;

    [SerializeField]
    private float rateOfRotation;

    [Header("NavigationSettings")]
    [Space]
    [SerializeField]
    private Transform centerOfArena;

    [SerializeField]
    private float maxDistance;

    [Header("DeathSettings")]
    [Space]
    [SerializeField]
    private Transform halfToDivide;

    [SerializeField]
    private Transform headModel;

    [SerializeField]
    private Transform neckRig;

    [SerializeField]
    private float distanceForHalves;

    [SerializeField]
    private Rig rigToDisable;

    private MoveMobile movement;
    private void Start()
    {
        movement = GetComponent<MoveMobile>();
        StartCoroutine(DirectionSetting());
        StartCoroutine(DirectionChanging());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StopAllCoroutines();
            movement.enabled = false;
            StartCoroutine(DeathAnimation());
        }
    }


    private Vector3 target;

    private Vector3 targetDirection;
    private Vector3 currentDirection;
    private float LerpParam;

    private IEnumerator DirectionSetting()
    {
        while(true)
        {
            target = Random.insideUnitSphere * maxDistance;
//            Debug.Log(target);
            target += centerOfArena.position;

            targetDirection = target - transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();
            LerpParam = 0;
            yield return new WaitForSeconds(
                                            Random.Range(dirSwapIntervalRange.x,
                                            dirSwapIntervalRange.y));
        }
    }

    private IEnumerator DirectionChanging()
    {
        while (true)
        {
            if (LerpParam <= 1)
            {
                LerpParam += rateOfRotation * Time.deltaTime;
                currentDirection = Vector3.Lerp(currentDirection, targetDirection, LerpParam);
                movement.UpdateMoveControl(currentDirection);
                //Debug.Log(currentDirection);
            }
            yield return null;
        }
    }

    private IEnumerator DeathAnimation()
    {
        var renders = GetComponentsInChildren<Renderer>();
        float alpha = 1;
        rigToDisable.weight = 0;
        for (int i = 0; i < 100; i++)
        {
            foreach (Renderer rend in renders)
            {
                Color newColor = rend.material.color;
                newColor.a = alpha;
                rend.material.color = newColor;
            }
            alpha -= 0.01f;
            halfToDivide.localPosition -= new Vector3(0, 0, distanceForHalves / 100 * 2);
            headModel.localPosition += new Vector3(0, 0, distanceForHalves / 100);
            neckRig.localPosition += new Vector3(0, 0, distanceForHalves / 100);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
