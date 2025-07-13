using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RetryScript : MonoBehaviour
{
    [SerializeField] private Image black;
    [SerializeField] private GameObject menu;
    [SerializeField] private float fadeDuration = 0.5f;

    public void Transition()
    {
        black.gameObject.SetActive(true);
        black.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
        {

            menu.SetActive(true);


            black.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                black.gameObject.SetActive(false);
            });
        });
    }
}
