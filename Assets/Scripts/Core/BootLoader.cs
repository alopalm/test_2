using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "01_TherapistSetup";
    [SerializeField] private float minimumBootTime = 1.0f;
    [SerializeField] private TextMeshProUGUI bootText;

    private IEnumerator Start()
    {
        if (bootText != null)
            bootText.text = "Iniciando sistema...";

        yield return new WaitForSeconds(minimumBootTime);

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(nextSceneName);

        while (!loadOp.isDone)
            yield return null;
    }
}
