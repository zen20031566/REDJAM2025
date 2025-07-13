using UnityEngine;

public class PerfectAndFail : MonoBehaviour
{
    [SerializeField] private GameObject perfectVisual;
    [SerializeField] private GameObject failVisual;
    [SerializeField] private float displayDuration = 1f;

    public void ShowPerfect()
    {
        failVisual.SetActive(false); 
        CancelInvoke(nameof(HidePerfect));
        perfectVisual.SetActive(true);
        Invoke(nameof(HidePerfect), displayDuration);
    }

    public void ShowFail()
    {
        perfectVisual.SetActive(false); 
        CancelInvoke(nameof(HideFail));
        failVisual.SetActive(true);
        Invoke(nameof(HideFail), displayDuration);
    }

    private void HidePerfect()
    {
        perfectVisual.SetActive(false);
    }

    private void HideFail()
    {
        failVisual.SetActive(false);
    }
}