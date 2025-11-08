using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectInfo : MonoBehaviour
{
    public Sprite objectImage;
    [TextArea] public string description;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ObjectInfo obj = hit.collider.GetComponent<ObjectInfo>();
                if (obj != null)
                {
                    obj.OnSelect();
                }
            }
        }
    }
    
    public void OnSelect()
    {
        // Afficher l’UI
        UIManager.instance.ShowInfo(objectImage, description);

        // Zoom caméra
        Camera.main.GetComponent<ZoomOnObject>().ZoomIn();
    }

    public void OnDeselect()
    {
        UIManager.instance.HideInfo();
        Camera.main.GetComponent<ZoomOnObject>().ZoomOut();
    }
    
}