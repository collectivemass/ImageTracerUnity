using CollectiveMass.ImageTracerUnity;
using UnityEngine;
using Unity.VectorGraphics;
using System.IO;

public class SceneController : MonoBehaviour
{

    public Texture2D texture;

    public Material startMaterial;
    public Material blurMaterial;


    private void Start()
    {
        Options options = new Options();

        startMaterial.mainTexture = texture;

        Texture2D blurTexture = ImageTracer.Blur(texture, options.Blur.BlurRadius, options.Blur.BlurDelta);
        blurMaterial.mainTexture = blurTexture;

        string svgData = ImageTracer.ImageToSvg(texture, options);

        string path = Path.Combine(Application.dataPath, "vectorout.svg");

        File.WriteAllText(path, svgData);

        Debug.Log($"Done: {path}");



        //StringReader svgDataReader = new StringReader(svgData);
        //SVGParser.SceneInfo svgScene = SVGParser.ImportSVG(svgDataReader,
        //    0F, 1F,
        //    100, 100, false);

        //svgDataReader.Close();
    }
}
