using UnityEngine;

public class MirrorControllerPose : MonoBehaviour
{
    public Transform sourceController;
    public Transform cameraHead; // Arrastra aquí la cámara de tus Oculus (Main Camera)
    public Transform bodyCenter;
    public Transform mirroredHandTarget;

    [Header("Ejes de Espejo")]
    public bool mirrorX = true;
    public bool mirrorY = false;
    public bool mirrorZ = false;

    [Header("Cubo de Movimiento Humano")]
    public float minX_Unity = -0.40f; float maxX_Unity = 0.40f;  
    public float minY_Unity = -0.30f; float maxY_Unity = 0.40f;  
    public float minZ_Unity = 0.25f;  float maxZ_Unity = 0.80f;  

    void Start()
    {
        // CALIBRACIÓN AUTOMÁTICA AL ARRANCAR:
        // Colocamos el centro del cuerpo alineado con tu cabeza actual, pero a la altura del pecho (Y fija)
        if (cameraHead != null && bodyCenter != null)
            {
                Vector3 posicionInicialCuerpo = cameraHead.position;
                posicionInicialCuerpo.y -= 0.35f; // Bajamos 35 cm desde tus ojos para situar el "pecho"
                bodyCenter.position = posicionInicialCuerpo;
                
                // Copiamos la rotación de tu silla para que el "frente" coincida hacia donde miras al empezar
                bodyCenter.rotation = Quaternion.Euler(0, cameraHead.eulerAngles.y, 0); 
                Debug.Log("[Calibración VR] Pecho fijado y bloqueado en el espacio.");
            }
    }

    void Update()
    {
        if (sourceController == null || bodyCenter == null || mirroredHandTarget == null)
            return;

        // A partir de aquí, bodyCenter NO SE MUEVE aunque tú muevas la cabeza. 
        // Mide el mando respecto a un punto fijo en el espacio real de tu habitación.
        Vector3 sourceLocalPos = bodyCenter.InverseTransformPoint(sourceController.position);
        Vector3 mirroredLocalPos = sourceLocalPos;

        if (mirrorX) mirroredLocalPos.x = -sourceLocalPos.x;
        if (mirrorY) mirroredLocalPos.y = -sourceLocalPos.y;
        if (mirrorZ) mirroredLocalPos.z = -sourceLocalPos.z;

        // Límites estrictos de la caja virtual
        mirroredLocalPos.x = Mathf.Clamp(mirroredLocalPos.x, minX_Unity, maxX_Unity);
        mirroredLocalPos.y = Mathf.Clamp(mirroredLocalPos.y, minY_Unity, maxY_Unity);
        mirroredLocalPos.z = Mathf.Clamp(mirroredLocalPos.z, minZ_Unity, maxZ_Unity);

        mirroredHandTarget.localPosition = mirroredLocalPos;
        mirroredHandTarget.localRotation = Quaternion.identity; // Rotación capada anti-giros raros

        Debug.DrawLine(bodyCenter.position, sourceController.position, Color.green);
        Debug.DrawLine(bodyCenter.position, mirroredHandTarget.position, Color.red);
    }
}
