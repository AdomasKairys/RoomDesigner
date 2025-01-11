using System.Collections.Generic;
using UnityEngine;

public class FurniturePreviewUIController : MonoBehaviour
{
    [SerializeField] ObjectDatabaseSO objectDatabaseSO;
    [SerializeField] GameObject previewItemPrefab;
    [SerializeField] Transform scrollViewContentListTransform;
    [SerializeField] PlacementStystem placementStystem;

    private void Awake()
    {
        foreach (var data in objectDatabaseSO.objectsData)
        {
            var instance = Instantiate(previewItemPrefab, scrollViewContentListTransform);
            if (!instance.TryGetComponent(out FurniturePreviewUIItemController itemController))
            {
                Debug.LogError("Furniture preview prefab doesn't have a controller");
                return;
            }
            itemController.SetUpPreviewPrefab(data.Prefab, () => placementStystem.StartPlacement(data.Id));
        }
    }
}
