using UnityEngine;
using System.Collections;


public class BulletHole : MonoBehaviour
{
    public float lifeTime = 15.0f;

    public void Set()
    {
        Invoke("Recycle", lifeTime);
    }

    public void ForceRecycle()
    {
        // cancel the invoke and recycle now no matter how much life time was left
        CancelInvoke("Recycle");
        Recycle();
    }

    void Recycle()
    {
        BulletHoleManager.bulletHole.activeHoles.Remove(gameObject);
        ObjectPool.pool.PoolObject(gameObject);
    }

}
