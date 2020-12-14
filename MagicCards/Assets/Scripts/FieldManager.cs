using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public int lines;
    public int columns;
    public GameObject fieldPref;


    public List<GameObject> field;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void CreateField()
    //{
    //    for (int i = 0; i < columns; i++)
    //    {
    //        var newCol = new List<GameObject>();
    //        for (int j = 0; j < lines; j++)
    //        {
    //            newCol.Add(Instantiate(fieldPref, spawnPoint));
    //        }
    //        field.Add(newCol);
    //    }
    //}
}
