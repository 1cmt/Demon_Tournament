using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    private readonly float movespeed = 60f;
    private float destroyTime = 3f;

    private TextMeshProUGUI damageText;

    private void Awake()
    {
        damageText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        gameObject.transform.position += movespeed * Time.deltaTime * Vector3.up;
        destroyTime -= Time.deltaTime;
        if (destroyTime <= 0) DestroyText();
    }

    public void SetText(int damage)
    {
        if (damageText == null)
        {
            Debug.LogError("컴포넌트 못 찾음");
            return;
        }

        damageText.text = $"-{damage}";
    }

    public void DestroyText()
    {
        Destroy(gameObject);
    }
}
