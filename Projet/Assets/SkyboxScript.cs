using UnityEngine;
using System.Collections;

public class SkyboxScript : MonoBehaviour {

    private GameObject player, endLine;

    public float minHeight, maxHeight;
    private float maxDistance, normalizedMaxHeight;

    void Start()
    {
        if (minHeight > maxHeight)
            throw new System.Exception("minHeight > maxHeight (Skybox)");

        player = GameObject.FindGameObjectWithTag("Player");
        endLine = GameObject.Find("EndLine");

        maxDistance = Vector3.Distance(player.transform.position, endLine.transform.position);
        transform.position = new Vector3(transform.position.x, minHeight, transform.position.z);

        normalizedMaxHeight = maxHeight - minHeight;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, endLine.transform.position);

        transform.position = new Vector3(transform.position.x,
                                         (maxHeight - (distance * normalizedMaxHeight / maxDistance)),
                                         transform.position.z);
    }
}
