using _Project.Scripts.Game;
using UnityEngine;

namespace _Project.Scripts.ObjScripts
{
    public class RunningEnemyMover : MonoBehaviour
    {
        [SerializeField] private float extraSpeed = 3f;

        private void Update()
        {
            float speed = 5f;

            if (LevelProgressManager.Instance != null)
            {
                speed = LevelProgressManager.Instance.CurrentGameSpeed;
            }

            transform.position +=
                Vector3.left *
                (speed + extraSpeed) *
                Time.deltaTime;
        }
    }
}