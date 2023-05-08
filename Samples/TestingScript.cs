using System;
using System.Collections;
using System.Collections.Generic;
using BatteryAcid.Serializables;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    [Serializable]
    public class TestStructure
    {
        public int BigInt;
        public string SuperString;
        public TestStructure Structure;

        public TestStruct Meh;
    }

    [Serializable]
    public struct TestStruct
    {
        public float thing;
        public char otherChar;
    }

    [SerializeField]
    private SerializableDateTime testDate;
    private SerializableDateTime TestDate => testDate;

    [SerializeField]
    private SerializableUri testUri;
    private SerializableUri TestUri => testUri;

    [SerializeField]
    private SerializableGuid testGuid;
    private SerializableGuid TestGuid => testGuid;

    public TestStructure Structure;
    public TestStruct Meh;

    [SerializeField]
    private SerializableStack<float> testFloatStack;
    private SerializableStack<float> TestFloatStack => testFloatStack;

    [SerializeField]
    private SerializableStack<SerializableGuid> testGuidStack;
    private SerializableStack<SerializableGuid> TestGuidStack => testGuidStack;

    [SerializeField]
    private SerializableStack<TestStructure> testStructureStack;
    private SerializableStack<TestStructure> TestStructureStack => testStructureStack;

}
