using UnityEngine;

namespace Other
{
    public class PlayerOptions
    {
        public float volume;
        
        public KeyCode dashKeyBind;
        public KeyCode secondAbilityKeyBind;

        public KeyCode upKeyBind;
        public KeyCode downKeyBind;
        public KeyCode leftKeyBind;
        public KeyCode rightKeyBind;
        
        public PlayerOptions()
        {
            volume = 0.3f;
            dashKeyBind = KeyCode.Space;
            secondAbilityKeyBind = KeyCode.Q;
            upKeyBind = KeyCode.W;
            downKeyBind = KeyCode.S;
            leftKeyBind = KeyCode.A;
            rightKeyBind = KeyCode.D;
        }
    }
}