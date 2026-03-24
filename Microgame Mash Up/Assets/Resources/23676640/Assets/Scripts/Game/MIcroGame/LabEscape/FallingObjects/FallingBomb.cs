using UnityEngine;

public class FallingBomb : MonoBehaviour
{
    [Header("Falling Bomb Fields")]
    [SerializeField] float fallDuration; // how long it will fall for
    private float fallTimer; // falling for


    [Header("Bomb Fields")]
    [SerializeField]private int bombYStartOffset; 
    [SerializeField] private Transform bombTransform;
    Vector3 bombStartPos;
    Vector2Int tilePos;

    [Header("Shadow Fields")]
    [SerializeField] private Transform shadowTransform;
    private SpriteRenderer shadowSR;
    [SerializeField] private Color shadowCol;

    private Vector3 startScale = Vector3.zero;
    private Vector3 endScale;

    public void InitFallingBomb(Vector2Int tile, Vector3 spawnPos, float fallDuration, Vector3 scale)
    {
        tilePos = tile;
        this.fallDuration = fallDuration;
        fallTimer = 0.0f;
        transform.position = spawnPos;

        if (shadowTransform != null)
        {
            shadowSR = shadowTransform.GetComponent<SpriteRenderer>();
            shadowTransform.localScale = scale;
            endScale = scale;
        }
        bombTransform.localScale = scale;
        
        SetUp();
    }

    private void SetUp()
    {
        shadowTransform.position = transform.position;

        bombStartPos = shadowTransform.position;
        bombStartPos.y += bombYStartOffset;
        bombTransform.position = bombStartPos;
        
        Color col = shadowCol;
        col.a = 0f;
        shadowSR.color = col;
    }

    public bool OnFBUpdate() // bool is landed
    {
;
        fallTimer += Time.deltaTime;
        float t = fallTimer / fallDuration;
        t = Mathf.Clamp01(t);
        
        //Shadow
        //fade
        Color col = shadowCol;
        col.a = t;
        shadowSR.color = col;

        //scale
        shadowTransform.localScale = Vector3.Lerp(startScale, endScale, t);

        //Bomb
        if (fallTimer <  fallDuration) // still falling
        {
            //decrease  y 
                       
            Vector3 targetPos = shadowTransform.position;
            float accelerateT = t * t;
            bombTransform.position = Vector3.Lerp(bombStartPos, targetPos, accelerateT);
           
        }
        else
        {
            //when falltimer == fall duratoin
           
            return true; // returns true when landed so manager can handle it without messing up list
        }

        return false;
    }

    public void OnFBExplode()
    {
        //explode
        //destroy bomb and shadow
        //enable explosion from tile manager
        //
        TileManager.Instance.CreateExplosionOnTile(tilePos);
        Destroy(this.gameObject);
    }
}
