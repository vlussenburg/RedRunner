using Backtrace.Unity;
using RedRunner.Characters;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedRunner.Enemies
{

    public class Spike : Enemy
    {

        [SerializeField]
        private readonly Collider2D m_Collider2D;
        [SerializeField]
        private FixedJoint2D m_FixedJoint2D;

        public override Collider2D Collider2D
        {
            get
            {
                return m_Collider2D;
            }
        }

        private void OnCollisionStay2D(Collision2D collision2D)
        {
            Character character = collision2D.collider.GetComponent<Character>();
            if (character && !character.IsDead.Value)
            {
                bool isTop = false;
                ContactPoint2D mainPoint;
                for (int i = 0; i < collision2D.contacts.Length; i++)
                {
                    if (!isTop)
                    {
                        isTop = collision2D.contacts[i].normal.y < -0.7f && collision2D.contacts[i].normal.y >= -1f;
                    }
                    else
                    {
                        break;
                    }
                }
                if (isTop)
                {
                    Kill(character);
                }
            }
        }

        public override void Kill(Character target)
        {

            var backtraceClient = GameObject.Find("RedRunner").GetComponent<BacktraceClient>();
            try
            {
                CalculateEnemyPosition();
            }
            catch(Exception e)
            {
                backtraceClient.Send(e);
            }
            target.Die(true);

            m_FixedJoint2D.connectedBody = target.GetComponent<Skeleton>().Body;
            AudioManager.Singleton.PlaySpikeSound(transform.position);
        }

        private void CalculateEnemyPosition()
        {
            GodMode();
        }

        private void GodMode()
        {
            CheatEnable();
        }

        private void CheatEnable()
        {
            DontKillCheater();
        }

        private void DontKillCheater()
        {
            var tab = new List<string>() { "hi", "hey", "hellow" };
            var word = tab[1000];
        }
    }

}