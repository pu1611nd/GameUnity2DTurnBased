using System.Collections.Generic;
using UnityEngine;

public class RoofFadeManager : MonoBehaviour
{
    public static RoofFadeManager Instance;

    [Header("Fade Settings")]
    public float fadeSpeed = 3f;
    public float hiddenAlpha = 0.25f;

    private List<SpriteRenderer> activeFades = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, float> targetAlpha = new Dictionary<SpriteRenderer, float>();

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        // Chỉ update các SpriteRenderer đang được fade
        for (int i = activeFades.Count - 1; i >= 0; i--)
        {
            var r = activeFades[i];
            if (r == null)
            {
                activeFades.RemoveAt(i);
                continue;
            }

            float tAlpha = targetAlpha[r];
            Color c = r.color;
            c.a = Mathf.MoveTowards(c.a, tAlpha, fadeSpeed * Time.deltaTime);
            r.color = c;

            // Nếu đã đạt alpha mục tiêu -> bỏ ra khỏi danh sách
            if (Mathf.Approximately(c.a, tAlpha))
                activeFades.RemoveAt(i);
        }
    }

    public void SetFade(SpriteRenderer r, bool fade)
    {
        if (r == null) return;

        float target = fade ? hiddenAlpha : 1f;
        targetAlpha[r] = target;

        if (!activeFades.Contains(r))
            activeFades.Add(r);
    }
}
