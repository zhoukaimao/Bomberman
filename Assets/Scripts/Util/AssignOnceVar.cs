using UnityEngine;
using System.Collections;

public class AssignOnceVar{
    private readonly int value;
    public int Value
    {
        get
        {
            return value;
        }
    }
    public AssignOnceVar(int value)
    {
        this.value = value;
    }
}
