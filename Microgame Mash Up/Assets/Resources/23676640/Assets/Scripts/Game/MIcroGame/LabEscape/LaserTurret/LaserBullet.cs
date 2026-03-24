using UnityEngine;

public class LaserBullet : MonoBehaviour
{
    BoxCollider2D col;

    [SerializeField] private GameObject laserSpriteGO;

    [SerializeField] private bool isActive;

    public bool IsActive => isActive;

    Vector3 startPos;

    Vector3 endPos;
    private float speed;
    private float travelDistance;
    private float t;


    public void OnBulletInit(Vector3 start, float bulletSpeed, float bulletTravelDist)
    {
        if (col == null)
        {
            col = GetComponent<BoxCollider2D>(); 
        }
        col.enabled = false;

        speed = bulletSpeed;

        startPos = start;
        endPos = start + transform.right * bulletTravelDist;

        t = 0f;
        isActive = false;
        laserSpriteGO.SetActive(false);

        
    }

    public void OnBulletEnable()
    {
        isActive = true;
        laserSpriteGO.SetActive(true);

        col.enabled = true;

        // Reset lerp
        t = 0f;
        transform.position = startPos;
    }

    public void OnBulletUpdate()
    {
        if (!isActive) return;


        
        t += Time.deltaTime * speed;

        transform.position = Vector3.Lerp(startPos, endPos, t);

        
        if (t >= 1f)
            OnReset();
    }

    public void OnReset()
    {
        isActive = false;
        laserSpriteGO.SetActive(false);

        t = 0f;
        transform.position = startPos;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        

        if (collision.CompareTag("Player"))
        {
            PlayerManager.Instance.OnPlayerHit();

        }
    }
}
