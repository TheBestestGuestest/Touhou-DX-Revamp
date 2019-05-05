using UnityEngine;
using UnityEngine.UI;
public class LoadingScreen : MonoBehaviour {
    public static LoadingScreen Instance;
    private const float MIN_TIME_TO_SHOW = 1f;
    private AsyncOperation currentLoadingOperation;
    private bool isLoading;
    private float timeElapsed;
    private Animator animator;
    private bool didTriggerFadeOutAnimation;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }
        Configure();
        Hide();
    }
    private void Configure() {
        animator = GetComponent<Animator>();
    }
    private void Update() {
        if (isLoading) {
            if (currentLoadingOperation.isDone && !didTriggerFadeOutAnimation) {
                animator.SetTrigger("Hide");
                didTriggerFadeOutAnimation = true;
            }
            else {
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= MIN_TIME_TO_SHOW) {
                    currentLoadingOperation.allowSceneActivation = true;
                }
            }
        }
    }
    public void Show(AsyncOperation loadingOperation) {
        gameObject.SetActive(true);
        currentLoadingOperation = loadingOperation;
        currentLoadingOperation.allowSceneActivation = false;
        timeElapsed = 0f;
        animator.SetTrigger("Show");
        didTriggerFadeOutAnimation = false;
        isLoading = true;
    }
    public void Hide() {
        gameObject.SetActive(false);
        currentLoadingOperation = null;
        isLoading = false;
    }
}