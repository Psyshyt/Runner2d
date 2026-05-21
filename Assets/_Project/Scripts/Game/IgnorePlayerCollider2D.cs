using System.Collections;
using UnityEngine;

namespace _Project.Scripts.ObjScripts
{
    [RequireComponent(typeof(Collider2D))]
    public class IgnorePlayerCollider2D : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private string playerTag = "Player";

        private Collider2D currentCollider;
        private Collider2D[] ignoredPlayerColliders;

        private void Awake()
        {
            currentCollider = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            StartCoroutine(SetupIgnoreCollision());
        }

        private IEnumerator SetupIgnoreCollision()
        {
            GameObject playerObject = null;

            while (playerObject == null)
            {
                playerObject = GameObject.FindGameObjectWithTag(playerTag);
                yield return null;
            }

            ignoredPlayerColliders = playerObject.GetComponentsInChildren<Collider2D>();

            foreach (Collider2D playerCollider in ignoredPlayerColliders)
            {
                if (playerCollider == null)
                    continue;

                Physics2D.IgnoreCollision(currentCollider, playerCollider, true);
            }

            Debug.Log($"{name} теперь игнорирует коллайдеры игрока");
        }

        private void OnDisable()
        {
            if (currentCollider == null || ignoredPlayerColliders == null)
                return;

            foreach (Collider2D playerCollider in ignoredPlayerColliders)
            {
                if (playerCollider == null)
                    continue;

                Physics2D.IgnoreCollision(currentCollider, playerCollider, false);
            }
        }
    }
}