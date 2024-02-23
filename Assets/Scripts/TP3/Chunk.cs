using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// Cette classe nécessite les composants MeshFilter, MeshRenderer et MeshCollider
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]

public class Chunk : MonoBehaviour
{
    // Courbe actuelle utilisée pour la déformation du terrain
    [SerializeField]
    private AnimationCurve currentPatternMirror;

    // Mesh du chunk
    private Mesh p_mesh;

    // Gestionnaire de chunks
    private ChunkManager chunkManager;

    // Méthode appelée au démarrage
    void Start()
    {
        // Création d'un nouveau mesh
        p_mesh = new Mesh();

        // Nettoyage du mesh
        p_mesh.Clear();

        // Récupération de l'instance du gestionnaire de chunks
        chunkManager = ChunkManager.Instance;

        // Calcul du nombre de vertices et de la taille de chaque pas
        int numVertices = Mathf.RoundToInt(chunkManager.resolution) + 1;
        float stepSize = chunkManager.dimension / chunkManager.resolution;

        // Vérifie si stepSize est NaN
        if (float.IsNaN(stepSize))
        {
            Debug.LogError("stepSize is NaN. Check the values of chunkManager.dimension and chunkManager.resolution.");
            return;
        }

        // Création des listes pour stocker les vertices et les triangles
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Calcul des offsets pour centrer le chunk
        float offsetX = -chunkManager.dimension / 2;
        float offsetZ = -chunkManager.dimension / 2;

        // Création des vertices et des triangles
        for (int i = 0; i < numVertices; i++)
        {
            for (int j = 0; j < numVertices; j++)
            {
                // Calcul des coordonnées x, y et z du vertex
                float x = i * stepSize + offsetX;
                float z = j * stepSize + offsetZ;
                float y = 0;

                // Ajout du vertex à la liste des vertices
                vertices.Add(new Vector3(x, y, z));

                // Si ce n'est pas le dernier vertex de la ligne ou de la colonne, création des triangles
                if (i < numVertices - 1 && j < numVertices - 1)
                {
                    // Calcul des indices des vertices du carré actuel
                    int topLeft = i * numVertices + j;
                    int topRight = topLeft + 1;
                    int bottomLeft = (i + 1) * numVertices + j;
                    int bottomRight = bottomLeft + 1;

                    // Ajout des deux triangles du carré à la liste des triangles
                    triangles.Add(topLeft);
                    triangles.Add(topRight);
                    triangles.Add(bottomLeft);

                    triangles.Add(topRight);
                    triangles.Add(bottomRight);
                    triangles.Add(bottomLeft);
                }
            }
        }

        // Affectation des vertices et des triangles au mesh
        p_mesh.vertices = vertices.ToArray();
        p_mesh.triangles = triangles.ToArray();

        // Calcul des normales et des bounds du mesh
        p_mesh.RecalculateNormals();
        p_mesh.RecalculateBounds();

        // Affectation du mesh aux composants MeshFilter et MeshCollider
        GetComponent<MeshFilter>().mesh = p_mesh;
        GetComponent<MeshCollider>().sharedMesh = p_mesh;
    }
    void Update()
    {
        // Mise à jour de la courbe actuelle pour refléter la courbe actuelle du gestionnaire de chunks
        currentPatternMirror = CurrentPattern;
    }

    // Méthode pour obtenir le mesh du chunk
    public Mesh GetMesh()
    {
        return p_mesh;
    }

    // Propriété pour obtenir la courbe actuelle du gestionnaire de chunks
    public AnimationCurve CurrentPattern
    {
        get { return ChunkManager.Instance.CurrentPattern; }
    }

    // Méthode pour déformer le terrain vers le haut
    public void DeformTerrainUp(Vector3 hitPoint, bool propagateToNeighbors = true)
    {
        // Obtention des vertices du mesh
        Vector3[] vertices = p_mesh.vertices;

        // Pour chaque vertex, si la distance entre le vertex et le point d'impact est inférieure au rayon, déformation du vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(hitPoint, transform.TransformPoint(vertices[i]));
            if (distance < chunkManager.radius)
            {
                float deformation = ChunkManager.Instance.CurrentPattern.Evaluate(distance / chunkManager.radius) * chunkManager.intensity;
                vertices[i] += deformation * Vector3.up;
            }
        }

        // Si propagateToNeighbors est vrai, déformation des terrains des chunks voisins
        if (propagateToNeighbors)
        {
            Chunk[] neighbors = { chunkManager.GetNeighborChunk(this, Vector3.left), chunkManager.GetNeighborChunk(this, Vector3.right), chunkManager.GetNeighborChunk(this, Vector3.forward), chunkManager.GetNeighborChunk(this, Vector3.back) };
            foreach (Chunk neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    neighbor.DeformTerrainUp(hitPoint, false);
                }
            }
        }

        // Affectation des nouveaux vertices au mesh et recalcul des normales
        p_mesh.vertices = vertices;
        p_mesh.RecalculateNormals();

        // Affectation du mesh au composant MeshCollider
        GetComponent<MeshCollider>().sharedMesh = p_mesh;
    }

    // Méthode pour déformer le terrain vers le bas
    public void DeformTerrainDown(Vector3 hitPoint, bool propagateToNeighbors = true)
    {
        // Obtention des vertices du mesh
        Vector3[] vertices = p_mesh.vertices;

        // Pour chaque vertex, si la distance entre le vertex et le point d'impact est inférieure au rayon, déformation du vertex
        for (int i = 0; i < vertices.Length; i++)
        {
            float distance = Vector3.Distance(hitPoint, transform.TransformPoint(vertices[i]));
            if (distance < chunkManager.radius)
            {
                float deformation = ChunkManager.Instance.CurrentPattern.Evaluate(distance / chunkManager.radius) * chunkManager.intensity;
                vertices[i] -= deformation * Vector3.up;
            }
        }

        // Si propagateToNeighbors est vrai, déformation des terrains des chunks voisins
        if (propagateToNeighbors)
        {
            Chunk[] neighbors = { chunkManager.GetNeighborChunk(this, Vector3.left), chunkManager.GetNeighborChunk(this, Vector3.right), chunkManager.GetNeighborChunk(this, Vector3.forward), chunkManager.GetNeighborChunk(this, Vector3.back) };
            foreach (Chunk neighbor in neighbors)
            {
                if (neighbor != null)
                {
                    neighbor.DeformTerrainDown(hitPoint, false);
                }
            }
        }

        // Affectation des nouveaux vertices au mesh et recalcul des normales
        p_mesh.vertices = vertices;
        p_mesh.RecalculateNormals();

        // Affectation du mesh au composant MeshCollider
        GetComponent<MeshCollider>().sharedMesh = p_mesh;
    }


    public void AddTerrain(Vector3 direction, GameObject chunkPrefab)
    {
        // Calcule la position du nouveau terrain en ajoutant la direction multipliée par la dimension du chunk à la position actuelle
        Vector3 newPosition = transform.position + direction * chunkManager.dimension;

        // Détermine s'il y a déjà un terrain à cet emplacement en vérifiant si des colliders se chevauchent avec une sphère centrée sur la nouvelle position
        Collider[] colliders = Physics.OverlapSphere(newPosition, chunkManager.dimension / 4);
        bool terrainAlreadyExists = colliders.Length > 0;

        if (!terrainAlreadyExists)
        {
            // Instancie un nouveau terrain à la position calculée
            GameObject newTerrain = Instantiate(chunkPrefab, newPosition, Quaternion.identity);
            newTerrain.name = "Terrain " + direction.ToString();

            // Obtient le composant Chunk du nouveau terrain
            Chunk newChunk = newTerrain.GetComponent<Chunk>();

            // Ajoute le nouveau chunk à la liste des chunks gérés par le ChunkManager
            ChunkManager.Instance.chunks.Add(newChunk);
        }
        else
        {
            Debug.LogWarning("Terrain already exists at " + newPosition);
        }
    }

    public void HighlightChunks()
    {
        // Obtient le composant MeshRenderer et le matériau original
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material originalMaterial = renderer.material;

        // Change le matériau pour un matériau standard de couleur aléatoire
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = new Color(Random.value, Random.value, Random.value);

        // Réinitialise le matériau après un délai
        StartCoroutine(ResetMaterial(renderer, originalMaterial, 3f));
    }

    IEnumerator ResetMaterial(MeshRenderer renderer, Material originalMaterial, float delay)
    {
        // Attend le délai spécifié
        yield return new WaitForSeconds(delay);

        // Réinitialise le matériau
        renderer.material = originalMaterial;
    }
}
