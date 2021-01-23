using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardPieces : MonoBehaviour
{
    public IntVariable HorizontalSize;
    public IntVariable VerticalSize;
    // Start is called before the first frame update
    void Start()
    {
        //sprite of the single board piece should be 1 unit size (100px)
        gameObject.GetComponent<SpriteRenderer>().size = new Vector2(HorizontalSize.Value, VerticalSize.Value);
        transform.position = Vector2.zero;
    }
}
