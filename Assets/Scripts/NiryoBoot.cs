using UnityEngine;

public class NiryoBoot : MonoBehaviour
{
    void Awake()
    {
        Debug.LogError("NIRYO_TEST_AWAKE");
        transform.position = new Vector3(0, 3, 0);
    }

    void Start()
    {
        Debug.LogError("NIRYO_TEST_START");
        transform.localScale = new Vector3(3, 3, 3);
    }

    void OnEnable()
    {
        Debug.LogError("NIRYO_TEST_ONENABLE");
    }
}
