using System.Linq;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    
    struct BlobPoint
    {
        public Vector2 pos;

        public BlobPoint(Vector2 p)
        {
            pos = p;
        }
    }
    [Header("Blob Shape")]
    [SerializeField] private int pointCount = 32;
    [SerializeField] private float pointDebugRadius;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private float springSpeed = 10f;

    [SerializeField] private BlobPoint[] blobPoints;

    [SerializeField] private SpriteRenderer faceRenderer;

    [SerializeField] private Color blobColour;

    [Header("Stretch Behaviour")]
    [SerializeField] private float stretchAmount = 0.25f;
    [SerializeField] private float bulgeAmount = 0.15f;

    [SerializeField]private MeshFilter mf;
    private Mesh mesh;

    private Vector3[] baseVerts;     // ideal circle
    private Vector3[] currentVerts;  // smoothed, squishy verts

    private Vector2Int moveDir = Vector2Int.zero;
    private float stretchT = 0f;

    [Header("Blob Faces")]
    [Tooltip("Up Down Left Right Stunned ")][SerializeField] private Sprite[] faceSprites;
    private Sprite prevFace;

    public void OnStart()
    {
        mf = GetComponentInChildren<MeshFilter>();
       
        mesh = new Mesh();
        mf.mesh = mesh;

        blobPoints = new BlobPoint[pointCount];

        GenerateCircleMesh();

        GetComponentInChildren<MeshRenderer>().material.color = blobColour;
         
    }


    private void GenerateCircleMesh()
    {
        baseVerts = new Vector3[pointCount + 1];
        currentVerts = new Vector3[pointCount + 1];

        Vector2[] uvs = new Vector2[pointCount + 1];
        int[] triangles = new int[pointCount * 3];

        baseVerts[0] = Vector3.zero;
        currentVerts[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        float step = Mathf.PI * 2f / pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * step;
            Vector2 p = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            blobPoints[i] = new BlobPoint(p);
            baseVerts[i + 1] = p;
            currentVerts[i + 1] = p;
            uvs[i + 1] = p * 0.5f + new Vector2(0.5f, 0.5f);
        }

        // triangles
        int t = 0;
        for (int i = 0; i < pointCount; i++)
        {
            triangles[t++] = 0;
            triangles[t++] = i + 1;
            triangles[t++] = (i + 1) % pointCount + 1;
        }

        mesh.Clear();
        mesh.vertices = currentVerts;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }



    public void Anim_OnMovement(float t, Vector2Int dir)
    {
        moveDir = dir;
        stretchT = Mathf.Sin(t * Mathf.PI);   // nice ease in/out
        UpdateFace(moveDir);
         
    }

    public void ResetScale()
    {
        stretchT = 0f;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);

        if (blobPoints != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < blobPoints.Length; i++)
            {
                Vector3 worldPos = transform.TransformPoint(blobPoints[i].pos);
                Gizmos.DrawSphere(worldPos, pointDebugRadius);
            }
        }
    }

    public void SetRadius(float newRadius)
    {
        radius = newRadius;

        
        GenerateCircleMesh();

       
        for (int i = 0; i < currentVerts.Length; i++)
            currentVerts[i] = baseVerts[i];

        mesh.vertices = currentVerts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    private void OnValidate()
    {
        if (Application.isPlaying == false) return;
        if (mesh == null) return;

        SetRadius(radius);
    }

    private void Update()
    {
        if (currentVerts == null) return;

    
        for (int i = 1; i < pointCount + 1; i++)
        {
            Vector3 baseP = baseVerts[i];
            Vector2 normal = baseP.normalized;

            float dot = Vector2.Dot(normal, moveDir);

            float push = Mathf.Max(0, dot) * stretchAmount * stretchT;
            float pull = Mathf.Max(0, -dot) * bulgeAmount * stretchT;

            float offset = push - pull;

            Vector3 move = new Vector3(moveDir.x, moveDir.y, 0);
            Vector3 target = baseP + (move * offset);

            currentVerts[i] = Vector3.Lerp(
                currentVerts[i],
                target,
                Time.deltaTime * springSpeed
            );
        }

        mesh.vertices = currentVerts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        
    }

    public void UpdateFace(Vector2 moveDir)
    {
        if (faceRenderer == null) return;

        if (moveDir == Vector2.up) // face up
        {
            faceRenderer.sprite = faceSprites[0]; //up
        }
        else if (moveDir == Vector2.down) // face down
        {
            faceRenderer.sprite = faceSprites[1]; //down
        }
        else if (moveDir == Vector2.left) // face left
        {
            faceRenderer.sprite = faceSprites[2]; //left
        }
        else if (moveDir == Vector2.right) // face right
        {
            faceRenderer.sprite = faceSprites[3]; //right
        }
        else
        {
            faceRenderer.sprite = faceSprites[3]; //default right
        }

    }


    

    public void ToggleStunSprite(bool isStun)
    {
        if (isStun)
        {
            prevFace = faceRenderer.sprite; //save prev sprte
            faceRenderer.sprite = faceSprites[4]; // stun sprite
        }
        else
        {
            //normal sprite
            faceRenderer.sprite = prevFace;
        }
    }
}