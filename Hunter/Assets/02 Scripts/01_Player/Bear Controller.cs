using System.ComponentModel.Design.Serialization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlayerState
{
    IDLE,
    MOVING,
    AIMING,
}

public class BearController : MonoBehaviour
{
    [Header("레이어 마스크")]
    [SerializeField] private LayerMask aimAreaLayer;

    [Header("Reference")]
    [SerializeField] private Animator animator;
    [SerializeField] private FollowCamera followCamera;
    [SerializeField] private BearAttacker attacker;

    private GameData gameData;
    private PlayerState currentState = PlayerState.IDLE;

    void Start()
    {
        if (gameData == null)
        {
            gameData = GameDataManager.Instance.GAMEDATA;
        }

        Camera mainCamera = Camera.main;

        Vector3 playerViewportPos = new Vector3(gameData.playerViewportX, gameData.playerViewportY, 0);
        Vector3 playerWorldPos = mainCamera.ViewportToWorldPoint(playerViewportPos);
        playerWorldPos.z = transform.position.z;
        transform.position = playerWorldPos;

        if (followCamera == null)
        {
            followCamera = FindObjectOfType<FollowCamera>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity, aimAreaLayer);

            if (hit.collider)
            {
                currentState = PlayerState.AIMING;
                attacker.DoAiming();
                animator.SetTrigger("Aiming");
                return;
            }

            currentState = PlayerState.MOVING;
            animator.SetTrigger("Moving");

            if (followCamera != null)
            {
                followCamera.OnPlayerStartMoving();
            }
        }

        if (Input.GetMouseButton(0))
        {
            switch (currentState)
            {
                case PlayerState.AIMING:
                    {
                        attacker.UpdateAiming();
                        break;
                    }
                case PlayerState.MOVING:
                    {
                        MouseMove();
                        break;
                    }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (currentState == PlayerState.AIMING)
            {
                attacker.DoFire();
                animator.SetTrigger("Shoot");
            }
            else if (currentState == PlayerState.MOVING)
            {
                if (followCamera != null)
                {
                    followCamera.OnPlayerStopMoving();
                }
                animator.SetTrigger("Idle");
            }
            currentState = PlayerState.IDLE;
        }
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void MouseMove()
    {
        transform.Translate(Vector3.left * gameData.playerSpeed * Time.deltaTime);
    }

    public bool IsMoving()
    {
        return currentState == PlayerState.MOVING;
    }
}
