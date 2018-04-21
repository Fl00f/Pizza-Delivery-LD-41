using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms2D;
using UnityEngine;

public class MouseDataSystem : ComponentSystem
{
    [Inject] private MouseData mouseData;

    private float2 lastFrameMousePosition;
    private Plane m_Plane;

    protected override void OnUpdate()
    {
        for (int i = 0; i < mouseData.Length; i++)
        {
            Position2D worldPosition = mouseData.worldPosition[i];

            float2 worldPos = worldPosition.Value;
            //Create a ray from the Mouse click position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Initialise the enter variable
            RaycastHit raycastHit2D;
            if (Physics.Raycast(ray, out raycastHit2D))
            {
                worldPos.x = raycastHit2D.point.x;
                worldPos.y = raycastHit2D.point.z;
            }

            mouseData.worldPosition[i] = new Position2D { Value = worldPos };
        }
    }
}

public struct MouseData
{
    public int Length;
    public ComponentDataArray<Mouse> mouse;
    public ComponentDataArray<Heading2D> worldDelta;
    public ComponentDataArray<Position2D> worldPosition;
}