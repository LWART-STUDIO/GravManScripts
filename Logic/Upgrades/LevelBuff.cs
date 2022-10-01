using UnityEngine;

namespace Custom.Logic.Upgrades
{
    [CreateAssetMenu(fileName = "New LevelBuff", menuName = "Level/Create Level Buff Settings", order = 1)]
    public class LevelBuff : ScriptableObject
    {
        public BuffId BuffId;
        public Sprite _buffSprite;
        public int Level;
        public int MaxLevel;
        public float MultiPlier;
    }
}