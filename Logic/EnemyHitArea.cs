using UnityEngine;

namespace Custom.Logic
{
    public class EnemyHitArea : MonoBehaviour
    {
        [SerializeField] private EnemyNavMeshDestination _enemyMain;
        /*private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 3)
            {
                _enemyMain.Hit(collision);
                
            }
        }*/
    }
}
