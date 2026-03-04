using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class ActorDirector : MonoBehaviour
{
    [SerializeField]
    private Actor actor;

    private float direction = 0f;

    [SerializeField]
    private GameObject arrow;

    private float dir = 0f;
    private float angle = 0f;

    [SerializeField]
    private bool target = false;


    void Update()
    {
        // if(actor.Target != null){

        //     if(!arrow.activeSelf) arrow.SetActive(true);

        //     float dir = this.transform.eulerAngles.z;
        //     float angle = Util.GetAngle((actor.Target.transform.position - actor.transform.position).normalized);
            
        //     // 현재 각도와 목표 각도의 차이를 계산
        //     float angleDiff = angle - dir;
            
        //     // 360도 범위를 넘어가는 경우를 처리
        //     if (angleDiff > 180f) angleDiff -= 360f;
        //     else if (angleDiff < -180f) angleDiff += 360f;
            
        //     // 더 짧은 경로로 회전하도록 목표 각도 설정
        //     direction = dir + angleDiff;
            
        //     // 부드러운 회전을 위한 보간
        //     dir = Mathf.Lerp(dir, direction, 0.1f);
        //     this.transform.rotation = Quaternion.Euler(0f, 0f, dir);
        // }
        // else{
        //     if(arrow.activeSelf) arrow.SetActive(false);
        // }

        bool confirm = false;

        if(target){
            if(actor.Target != null){
                confirm = true;
                angle = Util.GetAngle((actor.Target.transform.position - actor.transform.position).normalized);
            }
        }
        else{
            if(actor.MoveDir != Vector2.zero){
                confirm = true;
                angle = Util.GetAngle(actor.MoveDir);
            }

        }

        if(confirm){
            if(!arrow.activeSelf) arrow.SetActive(true);

            dir = this.transform.eulerAngles.z;
            //float angle = Util.GetAngle(actor.MoveDir);

            // 현재 각도와 목표 각도의 차이를 계산
            float angleDiff = angle - dir;
                
            // 360도 범위를 넘어가는 경우를 처리
            if (angleDiff > 180f) angleDiff -= 360f;
            else if (angleDiff < -180f) angleDiff += 360f;
            
            // 더 짧은 경로로 회전하도록 목표 각도 설정
            direction = dir + angleDiff;
            
            // 부드러운 회전을 위한 보간
            dir = Mathf.Lerp(dir, direction, 0.1f);
            this.transform.rotation = Quaternion.Euler(0f, 0f, dir);
        }
        else{
            if(arrow.activeSelf) arrow.SetActive(false);
        }
        

        // if(actor.MoveDir != Vector2.zero){
        //     if(!arrow.activeSelf) arrow.SetActive(true);

        //     dir = this.transform.eulerAngles.z;
        //     float angle = Util.GetAngle(actor.MoveDir);

        //     // 현재 각도와 목표 각도의 차이를 계산
        //     float angleDiff = angle - dir;
                
        //     // 360도 범위를 넘어가는 경우를 처리
        //     if (angleDiff > 180f) angleDiff -= 360f;
        //     else if (angleDiff < -180f) angleDiff += 360f;
            
        //     // 더 짧은 경로로 회전하도록 목표 각도 설정
        //     direction = dir + angleDiff;
            
        //     // 부드러운 회전을 위한 보간
        //     dir = Mathf.Lerp(dir, direction, 0.1f);
        //     this.transform.rotation = Quaternion.Euler(0f, 0f, dir);
        // }
        // else{
        //     if(arrow.activeSelf) arrow.SetActive(false);
        // }
        
    }
}
