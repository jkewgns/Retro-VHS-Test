using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
 
namespace LRS {
    [RequireComponent(typeof(LineRenderer))]
    public class StationaryScanner : MonoBehaviour {
        private LineRenderer _lineRenderer;
        [SerializeField] private List<PointsData> pointsData = new();
        private const string REJECT_LAYER_NAME = "PointReject";
        private const string TEXTURE_NAME = "PositionsTexture";
        private const string RESOLUTION_PARAMETER_NAME = "Resolution";
        [SerializeField] private bool reuseOldParticles = false;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private GameObject vfxContainer;
        [SerializeField] private Transform castPoint;
        [SerializeField] private float radius = 10f;
        [SerializeField] private float maxRadius = 10f;
        [SerializeField] private float minRadius = 1f;
        [SerializeField] private int pointsPerScan = 100;
        [SerializeField] private float range = 10f;
        [SerializeField] private int resolution = 100;
        // safety flag – ensures NewVisualEffect() is only called when needed
        private bool _createNewVFX;

        private void Start() {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.enabled = false;
            // Initialize each PointsData element
            pointsData.ForEach(data => {
                data.ClearData();
                _createNewVFX = true;
                data.currentVisualEffect = NewVisualEffect(data.prefab, out data.texture, out data.positionsAsColors);
                ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
            });
        }
 
        private void FixedUpdate() {
            // Always perform scanning without any player input
            Scan();
        }
 
        private void ApplyPositions(List<Vector3> positionsList, VisualEffect currentVFX, Texture2D texture, Color[] positions) {
            // Convert list to array for processing
            Vector3[] pos = positionsList.ToArray();
            // Cache the VFX object's position for offset
            Vector3 vfxPos = currentVFX.transform.position;
            int loopLength = texture.width * texture.height;
            int posListLen = pos.Length;
            for (int i = 0; i < loopLength; i++) {
                Color data;
                if (i < posListLen - 1) {
                    // Offset the point relative to the VFX position
                    data = new Color(pos[i].x - vfxPos.x, pos[i].y - vfxPos.y, pos[i].z - vfxPos.z, 1);
                } else {
                    data = new Color(0, 0, 0, 0);
                }
                positions[i] = data;
            }
            texture.SetPixels(positions);
            texture.Apply();
            currentVFX.SetTexture(TEXTURE_NAME, texture);
            currentVFX.Reinit();
        }
 
        private VisualEffect NewVisualEffect(VisualEffect visualEffect, out Texture2D texture, out Color[] positions) {
            // Instantiate a new VFX and set it up
            VisualEffect vfx = Instantiate(visualEffect, transform.position, Quaternion.identity, vfxContainer.transform);
            vfx.SetUInt(RESOLUTION_PARAMETER_NAME, (uint)resolution);
            texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false);
            positions = new Color[resolution * resolution];
            _createNewVFX = false;
            return vfx;
        }
 
        private void Scan() {
            // Loop for a set number of points per scan
            for (int i = 0; i < pointsPerScan; i++) {
                // Generate a random point within a sphere scaled by the radius and offset by castPoint's position
                Vector3 randomPoint = Random.insideUnitSphere * radius;
                randomPoint += castPoint.position;
                // Calculate the direction from this object to the random point
                Vector3 dir = (randomPoint - transform.position).normalized;
                // Cast a ray along the calculated direction
                if (Physics.Raycast(transform.position, dir, out RaycastHit hit, range, layerMask)) {
                    if (hit.collider.CompareTag(REJECT_LAYER_NAME))
                        continue;
                    // Check if the hit object has any of the valid tags and add the point accordingly
                    int resolution2 = resolution * resolution;
                    pointsData.ForEach(data => {
                        data.includedTags.ForEach(tag => {
                            if (hit.collider.CompareTag(tag)) {
                                if (data.positionsList.Count < resolution2) {
                                    data.positionsList.Add(hit.point);
                                } else if (reuseOldParticles) {
                                    data.positionsList.RemoveAt(0);
                                    data.positionsList.Add(hit.point);
                                } else {
                                    _createNewVFX = true;
                                    data.currentVisualEffect = NewVisualEffect(data.prefab, out data.texture, out data.positionsAsColors);
                                    data.positionsList.Clear();
                                }
                            }
                        });
                    });
                    // Optionally enable and update the LineRenderer for visual debugging
                    _lineRenderer.enabled = true;
                    _lineRenderer.SetPositions(new[] { transform.position, hit.point });
                } else {
Debug.DrawRay(transform.position, dir * range, Color.red);
                }
            }
            // Update the VisualEffect with the new positions
            pointsData.ForEach(data => {
                ApplyPositions(data.positionsList, data.currentVisualEffect, data.texture, data.positionsAsColors);
            });
        }
    }
}