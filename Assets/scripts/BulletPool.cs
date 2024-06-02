using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;
    public GameObject bulletPrefab;
    public int poolSize = 20;
    private Queue<GameObject> bulletPool;

    void Awake()
    {
        Instance = this;
        bulletPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            bulletPool.Enqueue(bullet);
        }
    }

    public GameObject GetBullet()
    {
        while (bulletPool.Count > 0)
        {
            GameObject bullet = bulletPool.Dequeue();
            if (bullet != null)
            {
                bullet.SetActive(true);
                return bullet;
            }
            // Nếu đối tượng đã bị hủy, tiếp tục lặp để lấy đối tượng khác từ pool
        }

        // Pool hết đạn, tạo mới đạn
        return Instantiate(bulletPrefab);
    }


    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }
}
