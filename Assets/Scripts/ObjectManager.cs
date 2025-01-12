using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    private Dictionary<int,(GameObject, Color, int)> _placedObjects = new();
    public IReadOnlyDictionary<int,(GameObject, Color, int)> GetPlacedObjects() => _placedObjects;

    private int _index = 0;
    public int PlaceObject(GameObject prefab, Vector3 position, Quaternion bodyBotation, Quaternion rotation, Color color, int id, int index=-1)
    {
        GameObject newObject = Instantiate(prefab);
        var previewBody = newObject.transform.Find("Body");
        if (previewBody == null)
        {
            Destroy(newObject);
            Debug.LogError("Object doesn't have a Body child");
            return -1;
        }
        newObject.transform.position = position;
        newObject.transform.rotation = rotation;
        previewBody.rotation = bodyBotation;

        newObject.layer = LayerMask.NameToLayer("Furniture");
        var children = newObject.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
        children.ForEach(c => c.layer = newObject.layer);
        ChangeColor(newObject, color);
        if(index != -1)
            _placedObjects.Add(index,(newObject, color, id));
        else
            _placedObjects.Add(_index, (newObject, color, id));
        _index++;
        return _index-1;
    }
    public void RemoveObjectAt(int index)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return;
        Destroy(_placedObjects[index].Item1);
        _placedObjects.Remove(index);
    }
    public void LoadObjects(SaveData data, ObjectDatabaseSO database)
    {
        RemoveAllObjects();
        var placedData = data.placedObjectsData;
        foreach (var obj in placedData)
        {
            int index = database.objectsData.FindIndex((x) => obj.Id == x.Id);
            GameObject gameObject = database.objectsData[index].Prefab;
            PlaceObject(gameObject, obj.position, obj.bodyRotation, obj.rotation, obj.Color, obj.Id, obj.placedObjectIndex);
        }
    }
    public void RemoveAllObjects()
    {
        foreach (var (_,obj) in _placedObjects)
            Destroy(obj.Item1);

        _placedObjects.Clear();
    }
    public void ChangeObjectLayerByIndex(int index, int layer)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return;

        _placedObjects[index].Item1.layer = layer;
        var children = _placedObjects[index].Item1.GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToList();
        children.ForEach(c => c.layer = layer);
    }
    public Color GetColorByIndex(int index)
    {
        if(!IsIndexValid(index, $"Game object color not found at index: {index}"))
            return Color.black;

        return _placedObjects[index].Item2;
    }
    public GameObject GetGameObjectByIndex(int index)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return null;

        return _placedObjects[index].Item1;
    }
    public void ChangeColorByIndex(int index, Color color)
    {
        if (!IsIndexValid(index, $"Game object not found at index: {index}"))
            return;

        _placedObjects[index] = (_placedObjects[index].Item1,color, _placedObjects[index].Item3);
        ChangeColor(_placedObjects[index].Item1, color);
    }
    private bool IsIndexValid(int index, string errorMessage = "")
    {
        if (index < 0 || !_placedObjects.ContainsKey(index))
        {
            Debug.LogError(errorMessage);
            return false;
        }
        return true;
    }
    
    private void ChangeColor(GameObject gameObject, Color color)
    {
        MaterialPropertyBlock selected = new MaterialPropertyBlock();
        selected.SetColor("_BaseColor", color);
        MeshRenderer[] renderers = gameObject.GetComponentsInChildren<MeshRenderer>();

        foreach (var renderer in renderers)
            renderer.SetPropertyBlock(selected);
    }
    
}
