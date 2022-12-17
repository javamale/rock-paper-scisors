using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockController : MonoBehaviour
{
    bool createNewInstanceOnDestroy = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Paper"))
        {
            createNewInstanceOnDestroy = true;
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (createNewInstanceOnDestroy)
        {
            GameManager.instance.AddToRockCount(-1);
            GameManager.instance.AddToPaperCount(1);
            GameManager.instance.InstantiatePaperPrefab(transform.position);
            createNewInstanceOnDestroy = false;
        }
    }
}
