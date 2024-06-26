using System.Collections;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private SpriteRenderer rend;
    private Color orignColor;

    private void Awake()
    {
        rend = gameObject.GetComponent<SpriteRenderer>();
        orignColor = rend.color;
    }

    public void ChangeColor(Color color)
    {
        Color newColor = color;
        newColor.a = 0.2f;

        StartCoroutine(FlashColor(newColor));
    }

    private IEnumerator FlashColor(Color color)
    {
        for (int i = 0; i < 3; i++)
        {
            rend.color = color;
            yield return new WaitForSeconds(0.2f);
            rend.color = orignColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
}