using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ObjectData
{
    [field: SerializeField]
    public string Name { get; private set; }
    [field: SerializeField]
    public int Id { get; private set; }
    [field: SerializeField, Tooltip("Rectangular boundry size for the shape on the grid")]
    public Vector3Int Size { get; private set; } = Vector3Int.one;
    [field: SerializeField, Tooltip("Space that the shape takes up on the grid in grid space")]
    public List<Vector3Int> ShapeOffsets { get; private set; }
    [field: SerializeField, Tooltip("Pivot for rotation in grid space")]
    public Vector3 PivotPoint { get; private set; }
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
    [field: SerializeField, Tooltip("What axis should the suface look at so that the object could be placed")]
    public Vector3Int PlacableSurfaceAxis { get; private set; }
    public List<Vector3Int> CurrentShapeOffsets { get; set; } = null;
}

[CreateAssetMenu(fileName = "ObjectDatabaseSO", menuName = "Scriptable Objects/ObjectDatabaseSO")]
public class ObjectDatabaseSO : ScriptableObject
{
    public List<ObjectData> objectsData;
}


