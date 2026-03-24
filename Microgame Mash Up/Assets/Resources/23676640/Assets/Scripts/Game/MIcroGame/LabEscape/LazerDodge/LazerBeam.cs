using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    Vector2Int gridPos;

    [Header("Laser Parts")]
    [SerializeField] private GameObject laserGraphic;   
    [SerializeField] private Collider2D hitbox;          

    [Header("Optional Warmup Flash")]
    [SerializeField] private float warmupDuration = 0.5f;
    [SerializeField] private float warmupTimer; 
    [SerializeField] private GameObject warmupGraphic; 

    [SerializeField]private bool isActive;

    public Vector2Int GetTilePos => gridPos;
   
    public void OnInitLazer(Vector2Int grid)
    {
        //set values
        gridPos = grid;
        DisableLazer();
    }

    public void ToggleLazer(bool state)
    {
        isActive = state;

       if (!isActive)
        {
            DisableLazer();
        }
    }

    public void OnUpdateLazer()
    {
        if (isActive)
        {
            if (warmupTimer < warmupDuration) // warming up
            {
                warmupTimer += Time.deltaTime;

                warmupGraphic.SetActive(true);

            }
            else
            {
                if (warmupGraphic.activeSelf)
                    warmupGraphic.SetActive(false);
                EnableLazer();
            }
        }
    }

    private void EnableLazer()
    {
        laserGraphic.SetActive(true);
        hitbox.enabled = true;
    }
    private void DisableLazer()
    {
        laserGraphic.SetActive(false);
        warmupTimer = 0.0f;
        hitbox.enabled = false;
        warmupGraphic.SetActive(false);
    }
  
}