using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Définition de la classe ChunkManager
public class ChunkManager : MonoBehaviour
{
    // Singleton pour accéder à l'instance de ChunkManager
    public static ChunkManager Instance { get; private set; }

    // Index de la courbe de déformation actuelle
    public int CurrentPatternIndex { get; set; }

    // Courbe de déformation actuelle
    public AnimationCurve CurrentPattern { get; set; }

    // Dimension, résolution, courbe de déformation par défaut, courbes de déformation personnalisées, rayon et intensité de la déformation
    public float dimension;
    public float resolution;
    public AnimationCurve deformationCurve;
    public AnimationCurve Pattern_1;
    public AnimationCurve Pattern_2;
    public AnimationCurve Pattern_3;
    public float radius = 1f;
    public float intensity = 1f;

    // Préfabriqué du chunk et matériaux
    public GameObject chunkPrefab;
    public List<Material> materials;
    private int currentMaterialIndex = 0;

    // Liste des chunks
    public List<Chunk> chunks = new List<Chunk>();

    // Méthode appelée lors de la création de l'objet
    private void Awake()
    {
        // Si Instance est null, cette instance devient l'instance singleton et n'est pas détruite lors du chargement d'une nouvelle scène
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Si une autre instance existe déjà, cette instance est détruite
            Destroy(gameObject);
        }
    }

    // Méthode appelée au démarrage
    private void Start()
    {
        // Instanciation du préfabriqué du chunk et ajout de tous les chunks à la liste des chunks
        Instantiate(chunkPrefab);
        chunks.AddRange(FindObjectsOfType<Chunk>());

        // La courbe de déformation actuelle est définie comme la courbe de déformation par défaut
        CurrentPattern = deformationCurve;
    }

    // Méthode appelée à chaque frame
    void Update()
    {
        // Si le bouton gauche de la souris est enfoncé
        if (Input.GetMouseButton(0))
        {
            // Si la touche Ctrl gauche est enfoncée, déformation du terrain vers le bas
            if (Input.GetKey(KeyCode.LeftControl))
            {
                // Détection du chunk survolé dans la liste des chunks
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                foreach (Chunk chunk in chunks)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider == chunk.GetComponent<MeshCollider>())
                        {
                            chunk.DeformTerrainDown(hit.point);
                        }
                    }
                }
            }
            else
            {
                // Sinon, déformation du terrain vers le haut
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                foreach (Chunk chunk in chunks)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.Log(hit.collider.gameObject.name);
                        if (hit.collider == chunk.GetComponent<MeshCollider>())
                        {
                            chunk.DeformTerrainUp(hit.point);
                        }
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // Si la touche Maj gauche est enfoncée, l'intensité de la déformation est augmentée de 0.5
            intensity += 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            // Si la touche Maj droite est enfoncée, l'intensité de la déformation est diminuée de 0.5
            intensity -= 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            // Si la touche Plus du pavé numérique est enfoncée, le rayon de la déformation est augmenté de 5
            radius += 5f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            // Si la touche Moins du pavé numérique est enfoncée, le rayon de la déformation est diminué de 5
            radius -= 5f;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Si la touche Flèche haut est enfoncée, un nouveau terrain est ajouté dans la direction avant pour chaque chunk
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.forward, chunkPrefab);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // Si la touche Flèche bas est enfoncée, un nouveau terrain est ajouté dans la direction arrière pour chaque chunk
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.back, chunkPrefab);
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Si la touche Flèche gauche est enfoncée, un nouveau terrain est ajouté dans la direction gauche pour chaque chunk
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.left, chunkPrefab);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Si la touche Flèche droite est enfoncée, un nouveau terrain est ajouté dans la direction droite pour chaque chunk
            List<Chunk> tempChunks = new List<Chunk>(chunks);
            foreach (Chunk chunk in tempChunks)
            {
                chunk.AddTerrain(Vector3.right, chunkPrefab);
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            // Si la touche M est enfoncée, tous les chunks sont mis en évidence
            foreach (Chunk chunk in chunks)
            {
                chunk.HighlightChunks();
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // Si la touche P est enfoncée, l'index de la courbe de déformation actuelle est incrémenté
            // Si l'index est hors limites, il est réinitialisé à 0
            // La courbe de déformation actuelle est définie comme la courbe à l'index actuel
            CurrentPatternIndex++;
            if (CurrentPatternIndex >= 4)
            {
                CurrentPatternIndex = 0;
            }
            switch (CurrentPatternIndex)
            {
                case 0:
                    CurrentPattern = deformationCurve;
                    break;
                case 1:
                    CurrentPattern = Pattern_1;
                    break;
                case 2:
                    CurrentPattern = Pattern_2;
                    break;
                case 3:
                    CurrentPattern = Pattern_3;
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            // Si la touche F12 est enfoncée, l'index du matériau actuel est incrémenté
            // Si l'index est hors limites, il est réinitialisé à 0
            // Le matériau de tous les chunks est changé pour le nouveau matériau
            currentMaterialIndex++;
            if (currentMaterialIndex >= materials.Count)
            {
                currentMaterialIndex = 0;
            }
            Material newMaterial = materials[currentMaterialIndex];
            foreach (Chunk chunk in chunks)
            {
                chunk.GetComponent<MeshRenderer>().sharedMaterial = newMaterial;
            }
        }
    }
    public Chunk GetNeighborChunk(Chunk chunk, Vector3 direction)
    {
        // Parcours de tous les chunks
        foreach (Chunk c in chunks)
        {
            // Si le chunk courant n'est pas le chunk passé en paramètre
            if (c != chunk)
            {
                // Calcul de la différence de position entre le chunk courant et le chunk passé en paramètre
                Vector3 diff = c.transform.position - chunk.transform.position;

                // Si la magnitude de cette différence est approximativement égale à la dimension du chunk
                // et si le produit scalaire entre la direction normalisée de cette différence et la direction normalisée passée en paramètre est approximativement égal à 1
                // (ce qui signifie que les deux directions sont les mêmes)
                if (Mathf.Approximately(diff.magnitude, dimension) && Mathf.Approximately(Vector3.Dot(diff.normalized, direction.normalized), 1))
                {
                    // Alors le chunk courant est le chunk voisin recherché et est retourné
                    return c;
                }
            }
        }

        // Si aucun chunk voisin n'est trouvé, la méthode retourne null
        return null;
    }
}