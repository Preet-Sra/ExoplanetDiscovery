using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Effects
{
    public class ExplosionPhysicsForce : MonoBehaviour
    {
        public float explosionForce = 4;

        private void OnEnable()
        {
            StartCoroutine(ApplyExplosionForce());
        }

        private IEnumerator ApplyExplosionForce()
        {
            // wait one frame because some explosions instantiate debris which should then
            // be pushed by physics force
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.01f);
            // Assuming a ParticleSystemMultiplier exists on this GameObject
            float multiplier = GetComponent<ParticleSystemMultiplier>().multiplier;

            // Radius of the explosion
            float radius = 10 * multiplier;

            // Get all colliders in the explosion radius
            var cols = Physics.OverlapSphere(transform.position, radius);

            // Create a list of unique rigidbodies
            var rigidbodies = new List<Rigidbody>();
            foreach (var col in cols)
            {
                if (col.attachedRigidbody != null && !rigidbodies.Contains(col.attachedRigidbody))
                {
                    rigidbodies.Add(col.attachedRigidbody);
                }
            }

            // Apply explosion force to the collected rigidbodies
            foreach (var rb in rigidbodies)
            {
                if (rb.CompareTag("Pieces"))
                {
                    
                    rb.AddExplosionForce(explosionForce * multiplier, transform.position, radius, 1 * multiplier, ForceMode.Impulse);
                }
            }
        }
    }
}
