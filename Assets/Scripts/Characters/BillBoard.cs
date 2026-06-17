using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
    }
}
