using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public float ghostDelay;
    private float ghostDelayseconds;
    public GameObject ghost;
    public bool makeGhost = false;

    // Start is called before the first frame update
    void Start()
    {
        ghostDelayseconds = ghostDelay;

    }

    // Update is called once per frame
    void Update()
    {
        if (makeGhost)
        {
            if (ghostDelayseconds > 0)
            {
                ghostDelayseconds -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                ghostDelayseconds = ghostDelay;
                Destroy(currentGhost, .5f);
            }

        }
       
    }
}
