using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class Minion : MonoBehaviour
{
    public enum eMinionState
    {
        WAITING_FOR_BATTLE,
        IDLE,
        ATTCKING,
        HIT,
        DEAD,
        VICTORY
    }
    eMinionState minionState = eMinionState.WAITING_FOR_BATTLE;
    Minion opponent;

    string m_minionName;
    public string minionName { get { return m_minionName; } }

    public Animator animator;

    bool waitingToAttack = false;

    public void MinionInit(string p_name)
    {
        m_minionName = p_name;
    }

    public int health;
    public int damage;
    float waitToAttackTime = 4f;

    public void ChangeState(eMinionState newState)
    {
        ARDebug.Log(newState.ToString() + minionName, 5);

        minionState = newState;

        switch (newState)
        {
            case eMinionState.ATTCKING:
                animator.SetTrigger("Attack");
                break;
            case eMinionState.IDLE:
                animator.SetTrigger("Idle");
                break;
            case eMinionState.DEAD:
                animator.SetTrigger("Dead");
                break;
            case eMinionState.HIT:
                animator.SetTrigger("Hit");
                break;
            case eMinionState.VICTORY:
                animator.SetTrigger("Victory");
                break;
        }
    }

    public void Activate(Minion minion)
    {
        opponent = minion;


        ChangeState(eMinionState.IDLE);
        StartCoroutine(QueAttack());
    }

    IEnumerator QueAttack()
    {
        float newWaitTime = waitToAttackTime + waitToAttackTime * Random.Range(0.75f, 1.25f);

        //Wait for attack time
        while(newWaitTime > 0)
        {
            newWaitTime -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        //if hit wait until not hit
        if (minionState == eMinionState.HIT)
        {
            waitingToAttack = true;
            yield return new WaitUntil(() => minionState != eMinionState.HIT);
        }
        
        //Break if dead
        if (minionState == eMinionState.DEAD) yield break;

        //attack
        Attack();
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));


        if (opponent.health > 0) {
            ChangeState(eMinionState.IDLE);
            StartCoroutine(QueAttack());
        }
        else
        {
            ChangeState(eMinionState.VICTORY);
        }
    }

    void Attack()
    {
        ChangeState(eMinionState.ATTCKING);
        waitingToAttack = false;
        opponent.TakeDamage();
    }

    public void TakeDamage()
    {
        StartCoroutine(ProcessDamage());
    }

    IEnumerator ProcessDamage()
    {
        ChangeState(eMinionState.HIT);
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
        if(!waitingToAttack) ChangeState(eMinionState.IDLE);
    }

    private void Update()
    {
        if(minionState != eMinionState.WAITING_FOR_BATTLE)
        {
            float xRot = transform.rotation.eulerAngles.x;
            transform.LookAt(opponent.gameObject.transform);
            transform.rotation = Quaternion.Euler(xRot, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        }
    }
}
