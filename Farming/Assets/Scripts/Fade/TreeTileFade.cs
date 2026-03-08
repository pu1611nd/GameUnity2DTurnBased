using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class TreeTileFade : MonoBehaviour
{
    public Tilemap treeTilemap;     // Tilemap chứa cây
    public Transform player;        // Player (nhân vật)
    [Range(0f, 1f)] public float fadedAlpha = 0.3f;
    public float fadeDuration = 0.3f;
    public float checkRadius = 0.2f; // Bán kính kiểm tra va chạm (nếu muốn gần cây là mờ)

    private Vector3Int? lastTile = null;
    private Coroutine fadeCoroutine;

    void Update()
    {
        Vector3Int playerCell = treeTilemap.WorldToCell(player.position);

        // Nếu nhân vật đang đứng hoặc chạm vào tile có cây
        if (treeTilemap.HasTile(playerCell))
        {
            if (lastTile == null || lastTile != playerCell)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeTile(playerCell, true));
                lastTile = playerCell;
            }
        }
        else
        {
            if (lastTile != null)
            {
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeTile(lastTile.Value, false));
                lastTile = null;
            }
        }
    }

    private IEnumerator FadeTile(Vector3Int cellPos, bool fadeOut)
    {
        // Cho phép thay đổi màu tile
        treeTilemap.SetTileFlags(cellPos, TileFlags.None);
        Color startColor = treeTilemap.GetColor(cellPos);
        float startAlpha = startColor.a;
        float endAlpha = fadeOut ? fadedAlpha : 1f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeDuration;
            float a = Mathf.Lerp(startAlpha, endAlpha, t);
            Color c = startColor;
            c.a = a;
            treeTilemap.SetColor(cellPos, c);
            yield return null;
        }
    }
}
