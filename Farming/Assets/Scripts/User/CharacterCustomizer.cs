using UnityEngine;

public class ModularCharacter : MonoBehaviour
{
    public Animator playerAnimator;

    [Header("Parts")]
    public SpriteRenderer ShirtRenderer;
    public SpriteRenderer TrouserRenderer;
    public SpriteRenderer HeadRenderer;
    public SpriteRenderer EyeRenderer;
    public SpriteRenderer HairRenderer;
    public SpriteRenderer AccessoriesRenderer;
    public SpriteRenderer wingRenderer;

    [Header("Sprites (2 frame mỗi bộ phận)")]
    public Sprite[] ShirtIdle;
    public Sprite[] ShirtWalk;
    public Sprite[] TrouserIdle;
    public Sprite[] TrouserWalk;
    public Sprite[] wingsAnim;

    [Header("Animation Settings")]
    public float animSpeed = 6f;

    private float timer;
    private int frame;

    private bool IsMoving => playerAnimator.GetFloat("Speed") > 0.1f;

    private void Update()
    {
        // Flip theo hướng di chuyển
        float horizontal = playerAnimator.GetFloat("Horizontal");
        if (horizontal > 0.05f) Flip(false);  // nhìn phải
        else if (horizontal < -0.05f) Flip(true); // nhìn trái

        // Animation timing
        timer += Time.deltaTime;
        if (timer >= 1f / animSpeed)
        {
            timer = 0f;
            frame++;
            UpdateSprites();
        }
    }

    private void Flip(bool faceLeft)
    {
        ShirtRenderer.flipX = faceLeft;
        TrouserRenderer.flipX = faceLeft;
        HeadRenderer.flipX = faceLeft;
        EyeRenderer.flipX = faceLeft;
        HairRenderer.flipX = faceLeft;
        AccessoriesRenderer.flipX = faceLeft;
        wingRenderer.flipX = faceLeft;
    }

    private void UpdateSprites()
    {
        // BODY
        Sprite[] bArr = IsMoving ? ShirtWalk : ShirtIdle;
        if (bArr != null && bArr.Length > 0)
            ShirtRenderer.sprite = bArr[frame % bArr.Length];

        // CLOTHES
        Sprite[] cArr = IsMoving ? TrouserWalk : TrouserIdle;
        if (cArr != null && cArr.Length > 0)
            TrouserRenderer.sprite = cArr[frame % cArr.Length];

        // WINGS (1 trạng thái)
        if (wingsAnim != null && wingsAnim.Length > 0)
            wingRenderer.sprite = wingsAnim[frame % wingsAnim.Length];
    }
}
