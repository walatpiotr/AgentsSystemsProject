using UnityEngine;

namespace Assets.Scripts
{
    public class SimulationUtility
    {
        private const float UNIFIED_SPACING = 10f;

        public static bool CheckLane(Transform nearestPoint, float SPAWNING_SPEED, float forwardModifier = 1, bool lookBackwards = true)
        {
            Vector2 backwardDirection;
            Vector2 forwardDirection;
            if (nearestPoint.position.y < 9f)
            {
                backwardDirection = Vector2.left;
                forwardDirection = Vector2.right;
            }
            else
            {
                backwardDirection = Vector2.right;
                forwardDirection = Vector2.left;
            }
            RaycastHit2D hitForward = Physics2D.Raycast(nearestPoint.position, forwardDirection);
            if(lookBackwards)
            {
                RaycastHit2D hitBackward = Physics2D.Raycast(nearestPoint.position, backwardDirection);
                PointCreator lane = nearestPoint.parent.GetComponent<PointCreator>();
                // distance < (2v1 - v2p - v2k)(v2k - v2p)/2a2 + buffer [km i h]
                // we assume v1 = lane.carMaxVelocity; buffer = v1; v2p = 70km/h 
                if ((hitBackward.collider != null))
                {
                    if (hitBackward.collider.tag == "car" && (hitBackward.distance < ((2 * lane.carMaxVelocity - SPAWNING_SPEED - lane.carMaxVelocity) * (lane.carMaxVelocity - SPAWNING_SPEED)) / (2 * UNIFIED_SPACING * 3.6f) / 1000 + lane.carMaxVelocity))
                    {
                        return false;
                    }
                    if (hitBackward.collider.tag == "truck" && (hitBackward.distance < ((2 * lane.carMaxVelocity - SPAWNING_SPEED - lane.truckMaxVelocity) * (lane.truckMaxVelocity - SPAWNING_SPEED)) / (2 * UNIFIED_SPACING * 3.6f) / 1000 + lane.carMaxVelocity))
                    {
                        return false;
                    }
                }
            }
            if ((hitForward.collider != null) && (hitForward.collider.tag == "car" || hitForward.collider.tag == "truck") && (hitForward.distance < SPAWNING_SPEED*forwardModifier))
            {
                return false;
            }
            return true;
        }


    }
}
