using System.Collections.Generic;
using UnityEngine;
public class Bus : MonoBehaviour {
    // Use this for initialization
    private List<Vector3> paths;
    void Awake () {
        paths = new List<Vector3>();
    }

    public void setPath(List<Vector3> path)
    {
        if (path.ToArray().Length > 0)
        {
            paths = path;
            iTween.MoveTo(transform.gameObject, iTween.Hash("path", paths.ToArray(), "orienttopath", true, "looktime", 0.001f, "lookahead", 0.001f, "time", 6000, "easetype", iTween.EaseType.linear, "looptype", iTween.LoopType.loop));
        }
    }
}
