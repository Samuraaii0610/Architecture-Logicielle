using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Add this line

public class HUD : MonoBehaviour
{
    // Déclaration des Textes UI pour afficher les informations
    public Text FPS_UItext; // Affichage des FPS
    public Text Mesh_UItext; // Affichage du nombre de maillages
    public Text Vertices_UItext; // Affichage du nombre de sommets
    public Text Triangles_UItext; // Affichage du nombre de triangles
    public Text Neighborhood_UItext; // Affichage du voisinage
    public Text Intensity_UItext; // Affichage de l'intensité
    public Text Pattern_UItext; // Affichage du motif

    // Déclaration des variables pour le calcul des FPS
    private int currentFPS;
    private float cumulatedTime;

    // Référence au gestionnaire de chunks
    private ChunkManager chunkManager;

    // Méthode appelée avant la première mise à jour de la frame
    void Start()
    {
        // Initialisation des variables
        currentFPS = 0;
        cumulatedTime = 0;
        chunkManager = ChunkManager.Instance;
    }

    // Méthode appelée à chaque frame
    void Update()
    {
        // Incrémentation du nombre de FPS et ajout du temps écoulé depuis la dernière frame au temps cumulé
        currentFPS++;
        cumulatedTime += Time.deltaTime;

        // Si le temps cumulé est supérieur à 1 seconde
        if (cumulatedTime > 1)
        {
            // Mise à jour du texte des FPS et réinitialisation des variables
            FPS_UItext.text = "FPS=" + currentFPS.ToString() + "   FPS moyen=" + Mathf.RoundToInt(Time.frameCount / Time.time);
            cumulatedTime -= 1;
            currentFPS = 0;

            // Mise à jour des autres statistiques
            Mesh_UItext.text = " Mesh Count: " + GetMeshCount();
            Vertices_UItext.text = " Total Vertices: " + GetTotalVertices();
            Triangles_UItext.text = " Total Triangles: " + GetTotalTriangles();
            Neighborhood_UItext.text = "Neighborhood: " + chunkManager.radius;
            Intensity_UItext.text = "Intensity: " + chunkManager.intensity;
            Pattern_UItext.text = "Pattern: " + GetPatternName();
        }
    }

    // Méthode pour obtenir le nombre de maillages
    private int GetMeshCount()
    {
        int count = 0;
        foreach (Chunk chunk in chunkManager.chunks)
        {
            count++;
        }
        return count;
    }

    // Méthode pour obtenir le nombre total de sommets
    private int GetTotalVertices()
    {
        int count = 0;
        foreach (Chunk chunk in chunkManager.chunks)
        {
            Mesh mesh = chunk.GetMesh(); // Utilisation de la méthode GetMesh
            if (mesh != null)
            {
                count += mesh.vertexCount;
            }
        }
        return count;
    }

    // Méthode pour obtenir le nombre total de triangles
    private int GetTotalTriangles()
    {
        int count = 0;
        foreach (Chunk chunk in chunkManager.chunks)
        {
            Mesh mesh = chunk.GetMesh(); // Utilisation de la méthode GetMesh
            if (mesh != null)
            {
                count += mesh.triangles.Length / 3;
            }
        }
        return count;
    }

    // Méthode pour obtenir le nom du motif actuel
    private string GetPatternName()
    {
        switch (chunkManager.CurrentPatternIndex)
        {
            case 0:
                return "Default";
            case 1:
                return "Pattern 1";
            case 2:
                return "Pattern 2";
            case 3:
                return "Pattern 3";
            default:
                return "Unknown";
        }
    }
}