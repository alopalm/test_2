using UnityEngine;

public class LeftControllerPos : MonoBehaviour
{
    public Vector3 leftPos;

    void Update()
    {
        // Busca directamente el LeftHandAnchor del XR Rig
        Transform leftHand = GameObject.Find("LeftHandAnchor")?.transform;
        if (leftHand != null)
        {
            leftPos = leftHand.position;
        }
    }
}