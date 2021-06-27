using System.Collections.Generic;
using Characters;
using UnityEngine;

namespace Party
{
    public class PartyManager : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Contains a list of all playable party members.")]
        public List<PartyMember> partyMembers;

        [SerializeField] 
        [Tooltip("Party member that will be seen on the map")]
        public Character primaryMember;
    }
}
