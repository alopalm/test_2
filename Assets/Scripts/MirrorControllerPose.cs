using UnityEngine;

public class MirrorControllerPose : MonoBehaviour
{
    public Transform sourceController;
    public Transform bodyCenter;
    public Transform mirroredHandTarget;

    public bool mirrorX = true;
    public bool mirrorY = false;
    public bool mirrorZ = false;

    public Vector3 visualOffset = Vector3.zero;

    void Update()
    {
        if (sourceController == null || bodyCenter == null || mirroredHandTarget == null)
            return;

        Vector3 sourceLocal = bodyCenter.InverseTransformPoint(sourceController.position);

        Vector3 mirroredLocal = sourceLocal;

        if (mirrorX) mirroredLocal.x = -sourceLocal.x;
        if (mirrorY) mirroredLocal.y = -sourceLocal.y;
        if (mirrorZ) mirroredLocal.z = -sourceLocal.z;

        Vector3 mirroredWorld = bodyCenter.TransformPoint(mirroredLocal);
        mirroredHandTarget.position = mirroredWorld + visualOffset;

        Debug.DrawLine(bodyCenter.position, sourceController.position, Color.green);
        Debug.DrawLine(bodyCenter.position, mirroredHandTarget.position, Color.red);
    }
}