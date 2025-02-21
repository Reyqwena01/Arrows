using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private float initialSpeed = 20f; // Vitesse initiale de la flèche
    [SerializeField] private float horizontalSpeed = 5f; // Vitesse de déplacement horizontal
    [SerializeField] private float verticalSpeed = 5f; // Vitesse de déplacement vertical
    [SerializeField] private AudioSource _shootSound = null; // Source audio pour le son de tir
    [SerializeField] private Rigidbody _rb = null;

    [SerializeField] private ParticleSystem _particleSystem = null;

    private Vector3 velocity; // Vecteur de vitesse

    void Start()
    {
        // Initialiser la vitesse avec une direction vers l'avant
        velocity = transform.forward * initialSpeed;
        _rb.drag = 10;

        // Jouer le son au démarrage si une source audio est définie
        if (_shootSound != null)
        {
            _shootSound.Play();
        }
    }

    void Update()
    {
        // Gérer les entrées utilisateur
        float horizontalInput = Input.GetAxis("Horizontal"); // Valeur entre -1 et 1
        float verticalInput = Input.GetAxis("Vertical"); // Valeur entre -1 et 1

        // Calculer les mouvements horizontaux et verticaux
        Vector3 horizontalMovement = transform.right * horizontalInput * horizontalSpeed;
        Vector3 verticalMovement = transform.up * verticalInput * verticalSpeed;

        // Mettre à jour la position de la flèche
        transform.position += (velocity + horizontalMovement + verticalMovement) * Time.deltaTime;

        // Mettre à jour la taille du système de particules en fonction de la vitesse totale
        ParticleSystem.MainModule mainModule = _particleSystem.main;
        mainModule.startSize = Mathf.Clamp((velocity + horizontalMovement + verticalMovement).magnitude * 0.1f, 0.1f, 1f);

        // Ajuster la rotation pour que la flèche pointe dans la direction du mouvement
        Vector3 movementDirection = velocity + horizontalMovement + verticalMovement;
        if (movementDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(movementDirection);
        }
    }
}
