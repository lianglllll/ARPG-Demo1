using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayeMoveTest : MonoBehaviour
{
    private Animator _animator;
    private Transform playerTransform;  //如果我们直接使用transform效率是有点低下的，我们直接记录它的引用地址
    private Vector3 playerInputVec;
    private Vector3 playerMovement;

    public float currentSpeed;
    public float targetSpeed;
    private float walkSpeed = 1.5f;
    private float runSpeed = 3.5f;

    private float rotateSpeed = 100;    //旋转速度

    private bool isRunning;
    private bool armedRifle;
    private bool isInjured;  

    private int injuredLayerIndex;
    private int fatigueLayerIndex;

    private float injuredFactor = 0.6f;//受伤影响因子，影响速度

    private float currentFatigue;       //疲劳值
    private float minFatigue = 0f;
    private float maxFatigue = 10f;



    private void Awake()
    {
        _animator = GetComponent<Animator>();
        playerTransform = transform;
    }

    private void Start()
    {
        injuredLayerIndex =  _animator.GetLayerIndex("Injured");
        fatigueLayerIndex =  _animator.GetLayerIndex("Fatigue");
        isInjured = false;
    }
    private void Update()
    {
        GetPlayerInput(); 
        RotatePlayer();
        MovePlayer();
        CaculateFatigue();
    }




    private void GetPlayerInput()
    {
        playerInputVec = InputManager.Instance.Movement;
        isRunning = InputManager.Instance.IsRun;

        if (InputManager.Instance.Equip)
        {
            armedRifle = !armedRifle;
            _animator.SetBool("Rifle", armedRifle);
        }

        if (InputManager.Instance.LAttack)
        {
            isInjured = !isInjured;
            if (isInjured)
            {
                _animator.SetLayerWeight(injuredLayerIndex, 1);
            }
            else
            {
                _animator.SetLayerWeight(injuredLayerIndex, 0);
            }
        }


    }

    private void RotatePlayer()
    {
        if (playerInputVec.Equals(Vector2.zero)) return;
        playerMovement.x = playerInputVec.x;
        playerMovement.z = playerInputVec.y;

        Quaternion targetRotation = Quaternion.LookRotation(playerMovement, Vector3.up);
        playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    private void MovePlayer()
    {
        targetSpeed = isRunning ? runSpeed : walkSpeed;
        targetSpeed *= playerInputVec.magnitude;
        if (isInjured)
        {
            targetSpeed *= injuredFactor;
        }
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.5f);
        _animator.SetFloat("VerticalSpeed", currentSpeed);
    }

    void CaculateFatigue()
    {
        if(currentSpeed < 1f && currentFatigue > 0)
        {
            currentFatigue -= Time.deltaTime;
        }
        else if(currentSpeed > 2f && currentFatigue < 10f)
        {
            currentFatigue += Time.deltaTime;
        }
        else
        {
            return;
        }
        currentFatigue = Mathf.Clamp(currentFatigue, minFatigue, maxFatigue);
        _animator.SetLayerWeight(fatigueLayerIndex, currentFatigue / maxFatigue);

    }


}
