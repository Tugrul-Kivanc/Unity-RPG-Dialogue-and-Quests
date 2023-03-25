using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] private Fighter[] fighters;
        [SerializeField] private bool activateOnStart = false;

        private void Start()
        {
            Activate(activateOnStart);
        }
        public void Activate(bool shouldActivate)
        {
            foreach (Fighter fighter in fighters)
            {
                fighter.enabled = shouldActivate;

                CombatTarget target = fighter.GetComponent<CombatTarget>();
                if (target != null) target.enabled = shouldActivate;
            }
        }
    }
}
