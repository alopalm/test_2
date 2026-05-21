using UnityEngine;

public class MirrorControllerPose : MonoBehaviour
{
    public Transform sourceController;
    public Transform bodyCenter;
    public Transform mirroredHandTarget;

    [Header("Ejes de Espejo (Espacio Humano)")]
    public bool mirrorX = true;
    public bool mirrorY = false;
    public bool mirrorZ = false;

    [Header("Cubo de Movimiento Humano (En Unity respecto al pecho)")]
    public float minX_Unity = -0.40f; // 40cm izquierda
    public float maxX_Unity = 0.40f;  // 40cm derecha
    public float minY_Unity = -0.30f; // 30cm abajo
    public float maxY_Unity = 0.40f;  // 40cm arriba
    public float minZ_Unity = 0.25f;  // Rango cercano (25cm del pecho)
    public float maxZ_Unity = 0.80f;  // Rango lejano (80cm brazo estirado)

    void Update()
    {
        if (sourceController == null || bodyCenter == null || mirroredHandTarget == null)
            return;

        // ========================================================
        // 1. CAPTURA Y CLAMP EN ESPACIO HUMANO (UNITY LOCAL)
        // ========================================================
        Vector3 sourceLocalPos = bodyCenter.InverseTransformPoint(sourceController.position);
        Vector3 mirroredLocalPos = sourceLocalPos;

        if (mirrorX) mirroredLocalPos.x = -sourceLocalPos.x;
        if (mirrorY) mirroredLocalPos.y = -sourceLocalPos.y;
        if (mirrorZ) mirroredLocalPos.z = -sourceLocalPos.z;

        // Forzar a la mano a no salirse de la zona cómoda de VR
        mirroredLocalPos.x = Mathf.Clamp(mirroredLocalPos.x, minX_Unity, maxX_Unity);
        mirroredLocalPos.y = Mathf.Clamp(mirroredLocalPos.y, minY_Unity, maxY_Unity);
        mirroredLocalPos.z = Mathf.Clamp(mirroredLocalPos.z, minZ_Unity, maxZ_Unity);

        // ========================================================
        // 2. ASIGNACIÓN EN ESPACIO LOCAL (Evita que caiga al 0,0,0 global)
        // ========================================================
        // Colocamos la bola de forma relativa al BodyCenter para que flote en la caja
        mirroredHandTarget.localPosition = mirroredLocalPos;

        // ========================================================
        // 3. CAPTURA DE ROTACIÓN LOCAL
        // ========================================================
        // Copiamos la orientación de tu muñeca en el espacio del pecho
        mirroredHandTarget.localRotation = Quaternion.Inverse(bodyCenter.rotation) * sourceController.rotation;

        // Debug visual en la pestaña Scene
        Debug.DrawLine(bodyCenter.position, sourceController.position, Color.green);
        Debug.DrawLine(bodyCenter.position, mirroredHandTarget.position, Color.red);
    }
}
