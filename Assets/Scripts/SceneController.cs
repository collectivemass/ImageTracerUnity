using CollectiveMass.ImageTracerUnity;
using UnityEngine;
using Unity.VectorGraphics;
using System.IO;

public class SceneController : MonoBehaviour
{

    public Options options = new Options();
    public Texture2D texture;

    public Material startMaterial;
    public Material blurMaterial;


    private void Awake()
    {
        startMaterial.mainTexture = texture;
    }

    public void Generate() {

        Debug.Log($"Starting");

        Texture2D blurTexture = TextureUtils.SelectiveGaussianBlur(texture, options.blur.blurRadius, options.blur.blurDelta);
        blurMaterial.mainTexture = blurTexture;

        string svgData = ImageTracer.ImageToSvg(texture, options);

        string path = Path.Combine(Application.dataPath, "vectorout.svg");

        File.WriteAllText(path, svgData);

        Debug.Log($"Done: {path}");


#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        //StringReader svgDataReader = new StringReader(svgData);
        //SVGParser.SceneInfo svgScene = SVGParser.ImportSVG(svgDataReader,
        //    0F, 1F,
        //    100, 100, false);

        //svgDataReader.Close();
    }
}
