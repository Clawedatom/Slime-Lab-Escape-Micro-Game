using UnityEngine;

public class Explosion : MonoBehaviour
{
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
