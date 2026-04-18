using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potion"))
        {
           
            other.gameObject.SetActive(false);

            PlayTrashEffect();
        }
    }

    void PlayTrashEffect()
    {

    }
}