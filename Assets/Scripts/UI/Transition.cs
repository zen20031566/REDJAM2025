using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    [SerializeField] private Image black;
    [SerializeField] private float fadeDuration = 0.5f;

    public void TransitionTo(string sceneName)
    {
        black.gameObject.SetActive(true);
        black.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {

            SceneManager.LoadScene(sceneName);


            black.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                black.gameObject.SetActive(false);
            });
        });
    }
   
}
