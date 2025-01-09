using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacer : MonoBehaviour
{
    private List<GameObject> _placedObjects = new();

    public (int Index, List<Vector3Int> FinalOffset) PlaceObject(GameObject prefab, Vector3 gridPos, Vector3 surfaceDirection, List<Vector3Int> shapeOffsets)
    {
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = gridPos;
        newObject.transform.LookAt(newObject.transform.position + surfaceDirection);
        newObject.transform.RotateAround(newObject.transform.position, newObject.transform.right, 90);
        if (surfaceDirection != Vector3.up) { 
            newObject.transform.RotateAround(newObject.transform.position, newObject.transform.up, 180);
        }
        List<Vector3Int> rotatedOffsets = new();
        for (int i = 0; i < shapeOffsets.Count; i++)
        {
            var rotatedOffset = newObject.transform.rotation * shapeOffsets[i];
            rotatedOffsets.Add(Vector3Int.RoundToInt(rotatedOffset));
        }
        _placedObjects.Add(newObject);
        return (_placedObjects.Count-1, rotatedOffsets);
    }
}
