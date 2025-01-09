using UnityEngine;

public class ArrowController : MonoBehaviour
{
    public float initialSpeed = 20f; // Vitesse initiale de la flèche
    public float horizontalSpeed = 5f; // Vitesse de déplacement horizontal

    private Vector3 velocity; // Vecteur de vitesse

    void Start()
    {
        // Initialiser la vitesse avec une direction vers l'avant
        velocity = transform.forward * initialSpeed;
    }

    void Update()
    {
        // Gérer le déplacement horizontal via les touches gauche et droite
        float horizontalInput = Input.GetAxis("Horizontal"); // Valeur entre -1 et 1
        Vector3 horizontalMovement = transform.right * horizontalInput * horizontalSpeed;

        // Ajouter le mouvement horizontal à la position
        transform.position += (velocity + horizontalMovement) * Time.deltaTime;

        // Ajuster la rotation pour que la flèche pointe dans la direction de la vitesse
        if (velocity + horizontalMovement != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(velocity + horizontalMovement);
        }
    }
}
