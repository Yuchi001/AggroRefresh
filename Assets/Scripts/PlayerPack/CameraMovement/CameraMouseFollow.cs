using System;
using UnityEngine;
using Utils;

namespace PlayerPack.CameraMovement
{
    public class CameraMouseFollow : MonoBehaviour
    {
        [SerializeField] private Vector2 cameraPositionOffset;
        [SerializeField] private float cameraMoveSpeed;
        private void Update()
        {
            var player = GameManager.Instance.PlayerObject;
            if (player == null) return;

            var playerPos = GameManager.Instance.PlayerObject.transform.position; 
            var dir = 
                (Vector3)UtilsMethods.GetMousePosition() 
                - playerPos;
            dir = dir.normalized;
            dir.x *= cameraPositionOffset.x;
            dir.y *= cameraPositionOffset.y;
            dir.z = -10;

            dir.x += playerPos.x;
            dir.y += playerPos.y;
            
            transform.position = Vector3.MoveTowards(transform.position, dir, cameraMoveSpeed * Time.deltaTime);
        }
    }
}