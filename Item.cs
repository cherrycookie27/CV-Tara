using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IPickupable
{
    public float jiggleAgressiveness = 0.25f;
    public float jiggleSpeed = 1f;

    public int id;

    void Start()
    {
        StartCoroutine(Jiggle());
    }

    IEnumerator Jiggle()
    {
        while (true)
        {
            Vector2 originalPositon = gameObject.transform.position;

            gameObject.transform.position = new Vector2(originalPositon.x, originalPositon.y + jiggleAgressiveness);

            yield return new WaitForSeconds(jiggleSpeed);

            gameObject.transform.position = new Vector2(originalPositon.x, originalPositon.y - jiggleAgressiveness);

            yield return new WaitForSeconds(jiggleSpeed);

            gameObject.transform.position = originalPositon;
        }
    }
    public void PickUp()
    {
        bool b = Inventory.instance.PickUp(id);
        if (b)
        {
            AudioManager.instance.PlaySFX("PickUp");
            Destroy(gameObject);
        }
    }
}
