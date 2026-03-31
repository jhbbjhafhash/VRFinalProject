using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Camera cam;
    public GameObject projectile;
    public Transform LHFirePoint, RHFirePoint;
    public float ProjectileSpeed = 20f;

    private Vector3 targetPoint;
    public bool leftHand = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            Shoot();
        }
    }

    void Shoot()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        } else {
            targetPoint = ray.GetPoint(1000);
        }

        if (leftHand == true)
        {
            leftHand = false;
            InstantiateProjectile(LHFirePoint);
        } else {
            leftHand = true;
            InstantiateProjectile(RHFirePoint);
        }

    }

    void InstantiateProjectile(Transform firePoint)
    {
        var projectileInstance = Instantiate(projectile, firePoint.position, Quaternion.identity) as GameObject;
        projectileInstance.GetComponent<Rigidbody>().linearVelocity = (targetPoint - firePoint.position).normalized * ProjectileSpeed;
    }
}
