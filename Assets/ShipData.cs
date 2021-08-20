using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipData : MonoBehaviour
{
    [field: SerializeField]
    public float RotationSpeed { get; private set; }
    [field: SerializeField]
    public float ThrusterPower { get; private set; }
}
