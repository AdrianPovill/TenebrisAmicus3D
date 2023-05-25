using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_Shot : MonoBehaviour
{
    [SerializeField] float timeToDisapear;
    [SerializeField] float bulletSpeed;
    private void Awake()
    {
        StartCoroutine(Destroy());
    }
    void Update()
    {
        transform.Translate(0f, bulletSpeed * Time.deltaTime, 0f);
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(timeToDisapear);

        Destroy(gameObject);
    }
}
