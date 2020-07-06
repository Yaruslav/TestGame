using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void CheckPosition()
    {
        if (transform.position.x <= Settings.minX)
            transform.position = new Vector3(Settings.maxX, transform.position.y, transform.position.z);
        else if (transform.position.x >= Settings.maxX)
            transform.position = new Vector3(Settings.minX, transform.position.y, transform.position.z);

        if (transform.position.z <= Settings.minZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, Settings.maxZ);
        else if (transform.position.z >= Settings.maxZ)
            transform.position = new Vector3(transform.position.x, transform.position.y, Settings.minZ);
    }

    private void Update()
    {
        CheckPosition();
    }
}
