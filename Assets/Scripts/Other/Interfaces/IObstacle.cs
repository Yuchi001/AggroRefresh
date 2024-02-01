using UnityEngine;

namespace Other.Interfaces
{
    public interface IObstacle
    {
        public float ForcedMovementSpeed { get; }
        public bool TryMove(Vector3 sourceObjectPosition);
    }
}