using UnityEngine;

public class MirrorControllerPose : MonoBehaviour
{
    public Transform sourceController;
    public Transform bodyCenter;
    public Transform mirroredHandTarget;

    [Header("Ejes de Espejo")]
    public bool mirrorX = true;
    public bool mirrorY = false;
    public bool mirrorZ = false;

    [Header("Límites de Seguridad (Metros)")]
    public float minX = 0.15f;
    public float maxX = 0.35f;
    public float minY = -0.15f;
    public float maxY = 0.15f;
    public float minZ = 0.05f;
    public float maxZ = 0.30f;

    public Vector3 visualOffset = Vector3.zero;

    void Update()
    {
        if (sourceController == null || bodyCenter == null || mirroredHandTarget == null)
            return;

        // 1. Calcular posición espejo (Tu lógica actual)
        Vector3 sourceLocal = bodyCenter.InverseTransformPoint(sourceController.position);
        Vector3 mirroredLocal = sourceLocal;

        if (mirrorX) mirroredLocal.x = -sourceLocal.x;
        if (mirrorY) mirroredLocal.y = -sourceLocal.y;
        if (mirrorZ) mirroredLocal.z = -sourceLocal.z;

        Vector3 mirroredWorld = bodyCenter.TransformPoint(mirroredLocal);
        
        // 2. APLICAR LÍMITES DE SEGURIDAD (Clamping)
        // Esto asegura que la bola target nunca mande al robot fuera de rango
        float clampedX = Mathf.Clamp(mirroredWorld.x, minX, maxX);
        float clampedY = Mathf.Clamp(mirroredWorld.y, minY, maxY);
        float clampedZ = Mathf.Clamp(mirroredWorld.z, minZ, maxZ);

        // 3. Aplicar posición final a la bola
        // Usamos localPosition si el BodyCenter es el padre directo
	mirroredHandTarget.localPosition = new Vector3(clampedX, clampedY, clampedZ);

        // Visualización en el Editor
        Debug.DrawLine(bodyCenter.position, sourceController.position, Color.green);
        Debug.DrawLine(bodyCenter.position, mirroredHandTarget.position, Color.red);
    }
}
