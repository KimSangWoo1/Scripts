using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DescriptSO : ScriptableObject
{
    [Header("설명")]
    [TextArea(3,5)]
    [SerializeField]
    private string descript; 
}
