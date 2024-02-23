using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager : MonoBehaviour
{
    // Références aux panneaux de l'interface utilisateur, assignées dans l'inspecteur
    public GameObject F1Panel;
    public GameObject F2Panel;
    public GameObject F3Panel;
    public GameObject F10Panel;

    // Méthode appelée à chaque frame
    void Update()
    {
        // Si la touche F1 est enfoncée, l'état d'activation du panneau F1 est inversé
        if (Input.GetKeyDown(KeyCode.F1))
        {
            F1Panel.SetActive(!F1Panel.activeSelf);
        }

        // Si la touche F2 est enfoncée, l'état d'activation du panneau F2 est inversé
        if (Input.GetKeyDown(KeyCode.F2))
        {
            F2Panel.SetActive(!F2Panel.activeSelf);
        }

        // Si la touche F3 est enfoncée, l'état d'activation du panneau F3 est inversé
        if (Input.GetKeyDown(KeyCode.F3))
        {
            F3Panel.SetActive(!F3Panel.activeSelf);
        }

        // Si la touche F10 est enfoncée, l'état d'activation du panneau F10 est inversé
        if (Input.GetKeyDown(KeyCode.F10))
        {
            F10Panel.SetActive(!F10Panel.activeSelf);
        }
    }
}