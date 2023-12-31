using UnityEngine;

public class Trigger: MonoBehaviour
{
    [SerializeField] private bool isStraw, isWood, isBrick;

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Straw"))
        {
            isStraw = true;
        }
        
        if (col.CompareTag("Wood"))
        {
            isWood = true;
        }
        
        if (col.CompareTag("Brick"))
        {
            isBrick = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Straw"))
        {
            isStraw = false;
        }
        
        if (col.CompareTag("Wood"))
        {
            isWood = false;
        }
        
        if (col.CompareTag("Brick"))
        {
            isBrick = false;
        }
    }

    public bool IsTriggered(string material)
    {
        if (material == "Straw") return isStraw;
        if (material == "Wood") return isWood;
        if (material == "Brick") return isBrick;

        return false;
    }
}
