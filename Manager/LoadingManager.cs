using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : Singleton<LoadingManager>
{
    [SerializeField] private GameObject _loadingUI;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private TextMeshProUGUI _progressText;

    public override void Awake()
    {
        base.Awake();
    }

    public void LoadScene(int sceneNum)
    {
        StartCoroutine(LoadSceneAsync(sceneNum));
    }

    private IEnumerator LoadSceneAsync(int sceneNum)
    {
        // 로딩 UI 활성화
        _loadingUI.SetActive(true);

        // 해당 씬을 비동기적으로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNum);
        // 씬 로딩이 끝나면 자동으로 넘어갈지 선택
        asyncLoad.allowSceneActivation = false;

        // 로딩 진행 상황 업데이트
        while (!asyncLoad.isDone)
        {
            // 진행률을 0 ~ 1 사이 값으로 변환
            #region // 설명
            // AsyncOperation.progress 값은 0 ~ 1 까지의 범위를 가진다.
            // 0 ~ 0.9는 로딩 진행 상황, 0.9 ~ 1은 씬이 활성화되는 과정이므로,
            // 0.9로 나눠주면 진행 상황을 0% ~ 100%까지 모두 반영할 수 있다.
            #endregion
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            _progressBar.value = progress;
            _progressText.text = $"Loading...{Mathf.FloorToInt(progress * 100)}%";

            // 로딩이 완료되면 씬 활성화
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // 로딩이 완료되면 로딩 UI 비활성화
        _loadingUI.SetActive(false);
    }
}
