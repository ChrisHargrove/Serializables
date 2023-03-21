using System.Collections;
using System.Collections.Generic;
using BatteryAcid.Serializables;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    [SerializeField]
    private SerializableDateTime testDate;
    private SerializableDateTime TestDate => testDate;
}
