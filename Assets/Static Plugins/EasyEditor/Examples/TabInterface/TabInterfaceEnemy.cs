using UnityEngine;
using System.Collections;
using EasyEditor;

namespace EasyEditor
{
    public class TabInterfaceEnemy : MonoBehaviour {

        public enum EnemyType
        {
            Dwarf,
            Angel,
            Wolf
        }

        [Inspector(group = "Description")]
        public string enemyName;
        public EnemyType type;
        public int age;
        public Color skinColor;

        [Inspector(group = "Skills")]
        public float speed;
        public int height;
        public bool canJump;

        [Inspector(group = "Personality")]
        public int charisma;
        public int generosity;
        public int positivism;
    }
}