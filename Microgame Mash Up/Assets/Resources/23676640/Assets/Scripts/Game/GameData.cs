using UnityEngine;

[CreateAssetMenu(menuName = "Game Data", fileName = "Scriptable Objects/ Game Data")]
public class GameData : ScriptableObject
{
    [Header("Tile Sprites")]
    [SerializeField] private Sprite exitTileOpen;
    [SerializeField] private Sprite exitTileClosed;

    [SerializeField] private Sprite playerSpawnTile;

    [SerializeField] private Sprite gameTileSprite;

    [Header("audio")]
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private AudioClip shootSound;

    [SerializeField] private AudioClip mainMusic;
    public AudioClip ShootSound => shootSound;

    public AudioClip GetMainMusic => mainMusic;

    public Sprite GetDefaultSprite => gameTileSprite;
    public Sprite GetPlayerSpawnSprite => playerSpawnTile;
    public Sprite[] GetExitTiles
    {
        get
        {
            return new Sprite[] { exitTileClosed, exitTileOpen };  // index 0 is locked , 1 is open
        }
    }
}
