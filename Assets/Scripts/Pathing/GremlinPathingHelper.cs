using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GremlinPathingHelper : PathingHelper{
    public GremlinPathingHelper (float size) : base(size)  {}

    public override Vector3 getNextWaypoint(Vector3 source, Vector3 destination) {
        int iterations = 0;
        RaycastHit hit;
        Vector3 originalDestination = destination;
        Vector3 direction = (destination - source).normalized;
        float distance = (destination - source).magnitude;
        while (Physics.SphereCast(source, 0.45f, direction, out hit, distance, TERRAIN_LAYER_MASK) && iterations < 5) {
            iterations++;
            Vector3 leftEdge, rightEdge;
            Vector3 newDestination;
            this.getWallEdges(source, hit.collider.gameObject, out leftEdge, out rightEdge);
            float leftDist = (leftEdge - source).magnitude + (originalDestination - leftEdge).magnitude;
            float rightDist = (rightEdge - source).magnitude + (originalDestination - rightEdge).magnitude;
            // TODO: Somehow, dont re-randomize a path if we're going around the same object. Keep track of the object we're moving around in memory?
            if (GrapeMath.goLeftAroundEdge(leftDist, rightDist)) {
                newDestination = leftEdge;
            } else {
                newDestination = rightEdge;
            }
            // Avoids infinite loops: happens when a ball gets stuck on an edge that spherecasting can't go through
            if (newDestination == destination) {
                break;
            }
            destination = newDestination;
            direction = (destination - source).normalized;
            distance = (destination - source).magnitude;
        }

        if (iterations < 5) {
            return destination;
        } else {
            Debug.Log("Error: Could not find path in 5 attempts");
            return originalDestination;
        }
    }

    private void getWallEdges(Vector3 source, GameObject wall, out Vector3 leftEdge, out Vector3 rightEdge) {
        Vector3 location = source;
        float xpos = wall.transform.position.x;
        float zpos = wall.transform.position.z;
        float xscale = wall.transform.localScale.x;
        float zscale = wall.transform.localScale.z;
        float size = 0.45f;
        float top = zpos + (zscale / 2.0f);
        float leftSide = xpos - (xscale / 2.0f);
        float rightSide = xpos + (xscale / 2.0f);
        float bottom = zpos - (zscale / 2.0f);
        Vector3 topLeft = new Vector3(leftSide - size, 0.5f, top + size);
        Vector3 topRight = new Vector3(rightSide + size, 0.5f, top + size);
        Vector3 bottomLeft = new Vector3(leftSide - size, 0.5f, bottom - size);
        Vector3 bottomRight = new Vector3(rightSide + size, 0.5f, bottom - size);
        // Edges
        if (location.x <= leftSide && location.z >= top) {
            leftEdge = topRight;
            rightEdge = bottomLeft;
        } else if (location.x >= rightSide && location.z >= top) {
            leftEdge = bottomRight;
            rightEdge = topLeft;
        } else if (location.x <= leftSide && location.z <= bottom) {
            leftEdge = topLeft;
            rightEdge = bottomRight;
        } else if (location.x >= rightSide && location.z <= bottom) {
            leftEdge = bottomLeft;
            rightEdge = topRight;
        // Faces
        } else if (location.z >= top) {
            leftEdge = topRight;
            rightEdge = topLeft;
        } else if (location.z <= bottom) {
            leftEdge = bottomLeft;
            rightEdge = bottomRight;
        } else if (location.x <= leftSide) {
            leftEdge = topLeft;
            rightEdge = bottomLeft;
        } else if (location.x >= rightSide) {
            leftEdge = bottomRight;
            rightEdge = topRight;
        } else {
            Debug.LogFormat("Help! I have no idea where to go on this object!\nLocation: {0}\nTop Left: {1}\nTop Right: {2}\nBottom Left: {3}\nBottom Right: {4}", location, topLeft, topRight, bottomLeft, bottomRight);
            leftEdge = source;
            rightEdge = source;
        }
    }
}
