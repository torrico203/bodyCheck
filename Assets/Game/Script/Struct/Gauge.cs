using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gauge : MonoBehaviour
{
    [SerializeField]
    protected long max = 0, value = 0, min = 0;

    protected float rate = 0f;

    [SerializeField]
    protected float barMax = 0f,barMin = 0f;
}