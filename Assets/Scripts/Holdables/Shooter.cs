using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Shooter {
    bool isSelf(GameObject gameObject);
    Vector3 getTracerSource();
}
