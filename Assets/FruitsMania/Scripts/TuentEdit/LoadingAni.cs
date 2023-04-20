using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LoadingAni : MonoBehaviour
{
    public TMP_Text textCompoment;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        textCompoment.ForceMeshUpdate();
        var textInfo = textCompoment.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            var charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
            {
                continue;
            }

            var verts = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                var orjg = verts[charInfo.vertexIndex + j];
                verts[charInfo.vertexIndex + j] = orjg + new Vector3(0, Mathf.Sin(Time.time * 2f + orjg.x + 0.01f) * 10f, 0);
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            textCompoment.UpdateGeometry(meshInfo.mesh, i);
        }
    }
}
