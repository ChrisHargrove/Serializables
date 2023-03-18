using System.Collections;
using System.Collections.Generic;
using BatteryAcid.Serializables;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    [SerializeField]
    private SerializableGuid first;
    private SerializableGuid First => first;

    [SerializeField]
    private SerializableGuid second;
    private SerializableGuid Second => second;

    [SerializeField]
    private List<SerializableGuid> guids;
    private List<SerializableGuid> Guids => guids;
}
