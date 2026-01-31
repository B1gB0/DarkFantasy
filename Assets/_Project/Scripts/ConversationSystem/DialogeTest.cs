using DialogueEditor;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public class DialogeTest : MonoBehaviour
    {
        [SerializeField] private NPCConversation myConversation;

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    ConversationManager.Instance.StartConversation(myConversation);
                }
            }
        }
    }
}