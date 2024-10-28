using UnityEngine;

public class DestroyPartsOnStart : MonoBehaviour
{
    // Configuration
    [Tooltip("Game Objects to destroy.")]
    public GameObject[] gameObjects;
    [Tooltip("Components to destroy.")]
    public Component[] components;


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject gameObject in gameObjects) Destroy(gameObject);
        foreach (Component component in components) Destroy(component);
        Destroy(this);
    }
}
