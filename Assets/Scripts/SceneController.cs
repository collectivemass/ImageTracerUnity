using UnityEngine;
using ImageTracerUnity;
using System.IO;

public class SceneController : MonoBehaviour
{

    public Texture2D texture;


    // Start is called before the first frame update
    void Start()
    {
        Options options = new Options();

        string svgData = ImageTracer.ImageToSvg(texture, options);

        string path = Path.Combine(Application.dataPath, "vectorout.svg");

        File.WriteAllText(path, svgData);

        Debug.Log($"Done: {path}");

    }
}
