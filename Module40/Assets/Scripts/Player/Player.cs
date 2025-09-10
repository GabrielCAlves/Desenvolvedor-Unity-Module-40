using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Ebac.StateMachine;
using Core.Singleton;
using Cloth;

public class Player : Singleton<Player>, IDamageable
{
    public Rigidbody playerRigidbody;
    public Transform cameraTransform;

    [Header("Setup")]
    public SOPlayerSetup soPlayerSetup;

    [Header("HealthBase")]
    public HealthBase healthBase;

    public float startLife = 10;

    [Header("Jump Collision Check")]
    public Collider collider;
    public float distToGround;
    public float spaceToGround = .1f;
    //public ParticleSystem jumpVFX;

    //[Header("Rotation")]
    //public float rotationSpeed = 1f;

    [Header("Friction Settings")]
    public float turningFrictionMultiplier = 0.3f;
    public float minFriction = 0.1f;

    [Header("Power-Up Cloth")]
    public string puCloth = "";

    public List<SFXType> sfxType;


    private Animator _currentPlayer;

    private GetPlayerPrefabAnimator _cameraController;

    public enum PlayerStates
    {
        RUN,
        IDLE,
        DEATH,
        REVIVE,
        JUMP,
        JUMP_GOING_UP,
        JUMP_GOING_DOWN,
        LANDING
    }

    public StateMachine<PlayerStates> stateMachine;

    public string death = "DeathTrigger";
    public string revive = "Revive";
    public string jump = "Jump";
    public string jumpUp = "Jump-Up";
    public string jumpDown = "Jump-Down";
    public string landing = "Landing";

    [Header("Configurações de Rotação")]
    public CharacterController characterController;
    public float turnSpeed = 1f;
    public float gravity = 9.8f;

    [Header("Scale Settings")]
    public bool isScaling = false;

    [Space]
    public float speed = 1f;

    [Header("Flash")]
    public List<FlashColor> flashcolors;

    public bool boostedSpeed = false;


    private Vector3 _regularScale;
    private Vector3 _jumpPoint;
    private float _currentSpeed;
    private bool _colided = false;
    private bool _isInTheGround = false;
    private bool _alreadyScaled = false;

    private bool _cancelOtherAnim = false;

    private float vSpeed = 0f;

    private Vector3 respawnPosition;

    /*[SerializeField]*/
    private ClothChanger _clothChanger;

    private bool _isWalking = false;

    #region LIFE
    public void Damage(float damage)
    {
        healthBase.Damage(damage);

        flashcolors.ForEach(i =>
        {
            i.Flash();
        });

        Debug.Log("Damage = "+damage);
        Debug.Log("(Damage) healthBase.currentLife = " + healthBase.currentLife);

        EffectsManager.Instance.ChangeVignette();

        if (_cameraController != null)
        {
            _cameraController.ApplyCameraNoise(1.5f, 2f, 1f);
        }
    }

    public void Damage(float damage, Vector3 dir)
    {
        Damage(damage);
    }

    private void Revive()
    {
        healthBase.ResetLife();
        Debug.Log("(Revive) healthBase.currentLife = "+healthBase.currentLife);
        Respawn();
    }

    #endregion

    #region CAMERA
    public void SetVirtualCameraReference(GetPlayerPrefabAnimator cameraController)
    {
        _cameraController = cameraController;
    }
    #endregion

    protected override void Awake()
    {
        base.Awake();

        _currentPlayer = Instantiate(soPlayerSetup.player, transform);

        if(collider != null)
        {
            distToGround = collider.bounds.extents.y;
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        //Init();
    }

    //private void Init()
    //{
    //    healthBase.currentLife = startLife;
    //}

    private void Start()
    {
        _regularScale = gameObject.transform.localScale;

        FlashColor[] mdls = gameObject.GetComponentsInChildren<FlashColor>();

        for (int i = 0; i < mdls.Length; ++i)
        {
            flashcolors.Add(mdls[i]);
            flashcolors[i].Start();
        }

        stateMachine = new StateMachine<PlayerStates>();

        stateMachine.Init();
        stateMachine.RegisterStates(PlayerStates.RUN, new StateBase());
        stateMachine.RegisterStates(PlayerStates.IDLE, new StateBase());
        stateMachine.RegisterStates(PlayerStates.DEATH, new StateBase());
        stateMachine.RegisterStates(PlayerStates.REVIVE, new StateBase());
        stateMachine.RegisterStates(PlayerStates.JUMP, new StateBase());
        stateMachine.RegisterStates(PlayerStates.JUMP_GOING_UP, new StateBase());
        stateMachine.RegisterStates(PlayerStates.JUMP_GOING_DOWN, new StateBase());
        stateMachine.RegisterStates(PlayerStates.LANDING, new StateBase());

        stateMachine.SwitchState(PlayerStates.IDLE, soPlayerSetup, _currentPlayer);

        _clothChanger = gameObject.GetComponentInChildren<ClothChanger>();

        if(puCloth != null)
        {
            if (puCloth.Equals("Cartoony"))
            {
                var i = GameObject.FindObjectOfType<ClothItemCartoony>();
                i.Collect();
            }
            else if (puCloth.Equals("Health"))
            {
                var i = GameObject.FindObjectOfType<ClothItemHealth>();
                i.Collect();
            }
            else if (puCloth.Equals("Defense"))
            {
                var i = GameObject.FindObjectOfType<ClothItemDefense>();
                i.Collect();
            }
            else if (puCloth.Equals("Speed"))
            {
                var i = GameObject.FindObjectOfType<ClothItemSpeed>();
                i.Collect();
            }
        }
    }

    void Update()
    {
        stateMachine.Update();

        HandleMovement();
        
        if (!_isInTheGround)
        {
            jumpState();
        }

        if (healthBase.currentLife <= 0 && !_cancelOtherAnim)
        {
            Debug.Log("(Update) healthBase.currentLife = " + healthBase.currentLife);

            _cancelOtherAnim = true;
            stateMachine.SwitchState(PlayerStates.DEATH, soPlayerSetup, _currentPlayer, death);

            PlaySFX(sfxType[1]);
        
            if (CheckpointManager.Instance.HasCheckpoint())
            {
                respawnPosition = CheckpointManager.Instance.GetPositionFromLastCheckpoint();
                Invoke(nameof(Revive), 2f);
            }
        }
    }

    private void HandleMovement()
    {
        if (!_cancelOtherAnim)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                _currentSpeed = soPlayerSetup.speedRun;
                _currentPlayer.speed = 2;
            }
            else
            {
                if (!boostedSpeed)
                {
                    _currentSpeed = soPlayerSetup.speed;
                    _currentPlayer.speed = 1;
                }
            }

            transform.Rotate(0, Input.GetAxis("Horizontal") * turnSpeed * Time.deltaTime, 0);
            if(transform.rotation != new Quaternion(0, 0, 0, 0))
            {
                Debug.Log("transform.rotation != new Quaternion(0, 0, 0, 0)!!!!!!!!!");
            }

            var inputAxisVertical = Input.GetAxis("Vertical");
            var speedVector = transform.forward * inputAxisVertical * _currentSpeed;

            if (characterController.isGrounded)
            {
                Debug.Log("characterController.isGrounded: "+characterController.isGrounded);
                Debug.Log("_isInTheGround: " + _isInTheGround);
                
                vSpeed = 0;
                if (Input.GetKeyDown(KeyCode.Space) && _isInTheGround)
                {
                    Debug.Log("Pulou!!!!! KeyCode.Space");

                    vSpeed = soPlayerSetup.forceJump;

                    PlaySFX(sfxType[0]);

                    DOTween.Kill(playerRigidbody.transform);

                    stateMachine.SwitchState(PlayerStates.JUMP, soPlayerSetup, _currentPlayer, jump);

                    //playerRigidbody.velocity = Vector3.up * soPlayerSetup.forceJump;
                    //playerRigidbody.transform.localScale = Vector3.one;

                    HandleScaleJump();
                    //PlayJumpVFX();

                    _isInTheGround = false;
                    _colided = false;
                }

                //HandleJump();
            }

            vSpeed -= gravity * Time.deltaTime;
            speedVector.y = vSpeed;

            characterController.Move(speedVector * Time.deltaTime);

            if(inputAxisVertical != 0)
            {
                stateMachine.SwitchState(PlayerStates.RUN, soPlayerSetup, _currentPlayer);
                Debug.Log("Andando!!!!!");

                if (!_isWalking && _isInTheGround)
                {
                    StartCoroutine(PlayingWalkSound());
                }


            }
            else
            {
                stateMachine.SwitchState(PlayerStates.IDLE, soPlayerSetup, _currentPlayer);
                Debug.Log("Parado!!!!!");
                _isWalking = false;
            }
        }
    }

    private IEnumerator PlayingWalkSound()
    {
        _isWalking = true;

        PlaySFX(sfxType[2]);

        yield return new WaitForSeconds(.2f);

        _isWalking = false;
    }

    private void jumpState()
    {
        if (_jumpPoint != null)
        {
            if (_jumpPoint.y != gameObject.transform.position.y)
            {
                if (_jumpPoint.y < gameObject.transform.position.y)
                {
                    stateMachine.SwitchState(PlayerStates.JUMP_GOING_UP, soPlayerSetup, _currentPlayer, jumpUp);
                    //_currentPlayer.SetBool(soPlayerSetup.boolJumpUp, true);
                    //_currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
                    new WaitForEndOfFrame();
                }
                else if (_jumpPoint.y > gameObject.transform.position.y)
                {
                    stateMachine.SwitchState(PlayerStates.JUMP_GOING_DOWN, soPlayerSetup, _currentPlayer, jumpDown);
                    //_currentPlayer.SetBool(soPlayerSetup.boolJumpUp, false);
                    //_currentPlayer.SetBool(soPlayerSetup.boolJumpDown, true);
                    new WaitForEndOfFrame();
                }
            }
        }

        _jumpPoint.y = gameObject.transform.position.y;
    }

    private void ScaleCharacter(float scaleZ, float scaleY, float scaleX)
    {
        isScaling = true;

        // Para todas as animações de escala atuais
        DOTween.Kill(playerRigidbody.transform);

        // Aplica a escala diretamente no transform, não no rigidbody
        transform.DOScale(new Vector3(scaleX, scaleY, scaleZ), soPlayerSetup.animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(soPlayerSetup.ease)
            .OnComplete(() => {
                isScaling = false;
                playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            });
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isInTheGround)
        {
            vSpeed = soPlayerSetup.forceJump;

            DOTween.Kill(playerRigidbody.transform);

            playerRigidbody.velocity = Vector3.up * soPlayerSetup.forceJump;
            playerRigidbody.transform.localScale = Vector3.one;

            HandleScaleJump();
            //PlayJumpVFX();

            _isInTheGround = false;
            _colided = false;
        }
    }

    //private void PlayJumpVFX()
    //{
    //    VFXManager.Instance.PlayVFXByType(VFXManager.VFXType.JUMP, transform.position);
    //}

    private void HandleScaleJump()
    {
        ScaleCharacter(soPlayerSetup.jumpScaleZ, soPlayerSetup.jumpScaleY, soPlayerSetup.jumpScaleX);
    }

    private void HandleLanding()
    {
        if (_colided)
        {
            //_currentPlayer.SetBool(soPlayerSetup.boolJumpUp, false);
            //_currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
            //_currentPlayer.SetBool(soPlayerSetup.boolJumpLanding, true);
            stateMachine.SwitchState(PlayerStates.LANDING, soPlayerSetup, _currentPlayer, landing);

            DOTween.Kill(playerRigidbody.transform);

            HandleScaleLanding();
        }

    }

    private void HandleScaleLanding()
    {
        ScaleCharacter(soPlayerSetup.landingScaleZ, soPlayerSetup.landingScaleY, soPlayerSetup.landingScaleX);
        
        playerRigidbody.constraints = RigidbodyConstraints.FreezeRotation;

        stateMachine.SwitchState(PlayerStates.IDLE, soPlayerSetup, _currentPlayer);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && _isInTheGround)
        {
            return;
        }
        else if (collision.gameObject.CompareTag("Ground") && !_isInTheGround)
        {
            _colided = true;
            _isInTheGround = true;

            HandleLanding();
        }

        Debug.Log("Entrou em contato");
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Contato ativo");
        
        //_currentPlayer.SetBool(soPlayerSetup.boolJumpDown, false);
        //_currentPlayer.SetBool(soPlayerSetup.boolJumpLanding, false);

        if (!_alreadyScaled && collision.gameObject.CompareTag("Ground") && !isScaling)
        {
            ScaleCharacter(_regularScale.z, _regularScale.y, _regularScale.x);
            _alreadyScaled = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Sem contato");
        _alreadyScaled = false;
    }

    [NaughtyAttributes.Button]
    public void Respawn()
    {
        //Debug.Log("(Respawn) checkpointPosition = " + respawnPosition);
        //Debug.Log("(Respawn) transform.position = " + transform.position);

        stateMachine.SwitchState(PlayerStates.REVIVE, soPlayerSetup, _currentPlayer, revive);

        //gameObject.transform.position = respawnPosition;
        //gameObject.transform.position = respawnPosition;

        while (transform.position != respawnPosition)
        {
            Debug.Log("(Respawn) checkpointPosition = " + respawnPosition + ". (Respawn) transform.position = " + transform.position);
            if (characterController != null)
                characterController.enabled = false;

            transform.position = respawnPosition;

            if (characterController != null)
                characterController.enabled = true;
        }


        _cancelOtherAnim = false;
    }

    public void ChangeSpeed(float speed, float duration)
    {
        StartCoroutine(ChangeSpeedCoroutine(speed, duration));
    }

    IEnumerator ChangeSpeedCoroutine(float speedBoost, float duration)
    {
        var defaultSpeed = _currentSpeed;
        boostedSpeed = true;
        _currentSpeed = speedBoost;
        _currentPlayer.speed = 2;
        yield return new WaitForSeconds(duration);
        _currentSpeed = defaultSpeed;
        boostedSpeed = false;
        //puCloth = "";
    }

    public void ChangeHealth(float health, float duration)
    {
        StartCoroutine(ChangeHealthCoroutine(health, duration));
    }

    IEnumerator ChangeHealthCoroutine(float healthBoost, float duration)
    {
        var defaultHealth = healthBase.startLife;
        healthBase.currentLife = healthBoost;
        yield return new WaitForSeconds(duration);
        healthBase.currentLife = defaultHealth;
        //puCloth = "";
    }

    public void ChangeTexture(ClothSetup setup, float duration)
    {
        StartCoroutine(ChangeTextureCoroutine(setup, duration));
    }

    IEnumerator ChangeTextureCoroutine(ClothSetup setup, float duration)
    {
        _clothChanger.ChangeTexture(setup);
        yield return new WaitForSeconds(duration);
        _clothChanger.ResetTexture();
        puCloth = "";
    }

    private void PlaySFX(SFXType sfxTypeWithIndex)
    {
        SFXPool.Instance.Play(sfxTypeWithIndex);
    }
}
