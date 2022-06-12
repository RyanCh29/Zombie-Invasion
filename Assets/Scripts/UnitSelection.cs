using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    [SerializeField] public List<string> unitsList;

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("SelectionBox collide with " + col.collider.name);

        unitsList.Add(col.name);

    }
}
