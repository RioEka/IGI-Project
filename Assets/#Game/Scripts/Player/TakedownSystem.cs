using UnityEngine;

using DG.Tweening;

namespace IGI.Player
{
    public class TakedownSystem : MonoBehaviour
    {
        [Header("References")]
        private Transform player => transform;

        [Header("Animation")]
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private Animator enemyAnimator;

        [Header("Offset & Movement")]
        [SerializeField] private Vector3 relativeOffsetFromEnemy = new Vector3(0.07083f, 0f, -0.54896f);
        [SerializeField] private float moveDuration = 0.25f;
        [SerializeField] private float rotateDuration = 0.2f;

        public void TriggerTakedown(Transform enemy)
        {
            // Hitung posisi target relatif ke enemy
            Vector3 targetPosition = enemy.position + enemy.TransformDirection(relativeOffsetFromEnemy);

            // Rotate player menghadap enemy (optional, jika ingin sinematik)
            Quaternion targetRotation = Quaternion.LookRotation(enemy.position - player.position);

            Sequence seq = DOTween.Sequence();
            seq.Append(player.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutSine));
            seq.Join(player.DORotateQuaternion(targetRotation, rotateDuration));
            seq.OnComplete(PlayTakedownAnimation);
        }

        private void PlayTakedownAnimation()
        {
            playerAnimator.Play("PlayerAttack");
            enemyAnimator.Play("EnemyTakedown");
        }
    }
}
