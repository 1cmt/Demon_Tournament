using UnityEngine;

public class IntroSceneUtils : MonoBehaviour
{
    [Header("UI View")]
    [SerializeField] private GameObject _titleView;
    [SerializeField] private GameObject _selectView;

    private void Start()
    {
        if (GameManager.Instance.hasMovedScene)
        {
            _titleView.SetActive(false);
            _selectView.SetActive(true);
        }
        else
        {
            _titleView.SetActive(true);
            _selectView.SetActive(false);
        }
    }
}
