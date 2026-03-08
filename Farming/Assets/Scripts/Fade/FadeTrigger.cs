using UnityEngine;

public class FadeTrigger : MonoBehaviour
{
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.LogWarning("Thiếu SpriteRenderer trên " + name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            RoofFadeManager.Instance.SetFade(sr, true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            RoofFadeManager.Instance.SetFade(sr, false);
    }
}
