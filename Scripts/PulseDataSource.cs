/* Distributed under the Apache License, Version 2.0.
   See accompanying NOTICE file for details.*/

using UnityEngine;

public abstract class PulseDataSource: MonoBehaviour
{
    [SerializeField, HideInInspector]
    public PulseData data;
}
