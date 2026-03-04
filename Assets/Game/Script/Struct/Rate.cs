using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Rate
{
    public GradeRate[] rate;
}

[System.Serializable]
public class GradeRate{
    public Grade grade;
    public float rate;
}
