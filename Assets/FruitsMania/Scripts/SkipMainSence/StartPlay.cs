using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using dotmob.Scripts.MapScripts.StaticMap.Editor;
using dotmob.Scripts.System;
using UnityEngine.SceneManagement;
public class StartPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play () {
			
			LeanTween.delayedCall(1, ()=>SceneManager.LoadScene(Resources.Load<MapSwitcher>("Scriptable/MapSwitcher").GetSceneName()));
		}
}
