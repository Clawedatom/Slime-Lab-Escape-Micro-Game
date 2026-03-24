using UnityEngine;

public class LabKey : MonoBehaviour
{
    Vector2Int tilePos;
    public Vector2Int GetPos => tilePos;
    public void KeyInit(Vector2Int t)
    {
        tilePos = t;
        
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
 
}

