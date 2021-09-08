using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelToVoxel : MonoBehaviour
{
    public Texture2D image;

    public MeshRenderer render;
    public MeshRenderer renderCurrept1 , renderCurrept2, renderCurrept3, renderCurrept4;

    public GameObject[] voxels;
    int xSize;
    int ySize;
    Color32[] pixels;
    Texture2D curreptImage;
    Texture2D bubbleSortImage, heapSortImage, quickSortImage;


    [Range (1, 100)]
    public int visualizationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        xSize = image.width;
        ySize = image.height;

        pixels = image.GetPixels32();

        Debug.Log("X= " + xSize);
        Debug.Log("Y= " + ySize);
        Debug.Log("pixels= " + pixels.Length);

        render.material.mainTexture = GenerateTexture();
        render.transform.localScale = new Vector3(((float)xSize / (float)ySize), 1, 1);

        
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) {

            curreptImage = CurreptTexture();
            renderCurrept1.material.mainTexture = curreptImage;
            renderCurrept1.transform.localScale = new Vector3(((float)xSize / (float)ySize), 1, 1);
        }

        if (Input.GetKeyDown(KeyCode.B)) {
            //BubbleSort();
            StartCoroutine(VisualizeBubbleSort());
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HeapSort();
        }
    }

    Texture2D GenerateTexture() {
        Texture2D generatedImage = new Texture2D(xSize, ySize);

        for (int y = 0; y < ySize; y++)
        {
            for (int x = 0; x < xSize; x++)
            {
                generatedImage.SetPixel(x, y, pixels[y * xSize + x]);
            }
        }

        generatedImage.Apply();
        return generatedImage;
    }


    ImageSortingData ThisImageSortingData;
    Texture2D CurreptTexture()
    {
        Texture2D generatedImage = new Texture2D(xSize, ySize);
        Color32[] pixelsBuffer = new Color32[pixels.Length];
        ThisImageSortingData = new ImageSortingData(pixels.Length);


        pixelsBuffer = pixels;

        for (int i = 0; i < pixelsBuffer.Length - 1; i++) // O(n)
        {
            int rand = Random.Range(i+1, pixelsBuffer.Length-1);

            Color32 bucket;

            //backup
            bucket = pixelsBuffer[i];

            //Swap
            pixelsBuffer[i] = pixelsBuffer[rand];
            pixelsBuffer[rand] = bucket;

            //UpdateSortingData
            int temp = ThisImageSortingData.shuffeledPosition[i];
            ThisImageSortingData.shuffeledPosition[i] = ThisImageSortingData.shuffeledPosition[rand];
            ThisImageSortingData.shuffeledPosition[rand] = temp;
        }

        generatedImage.SetPixels32(pixelsBuffer);

        generatedImage.Apply();
        return generatedImage;
    }


    IEnumerator VisualizeBubbleSort() {
        int[] bubbleData = new int[ThisImageSortingData.shuffeledPosition.Length];

        for (int i = 0; i < ThisImageSortingData.shuffeledPosition.Length; i++)
        {
            bubbleData[i] = ThisImageSortingData.shuffeledPosition[i];
        }

        Color32[] pixelsBuffer = new Color32[pixels.Length];

        pixelsBuffer = curreptImage.GetPixels32();

        bubbleSortImage = new Texture2D(xSize, ySize);
        bubbleSortImage.SetPixels32(pixelsBuffer);
        bubbleSortImage.Apply();
        renderCurrept2.material.mainTexture = bubbleSortImage;
        renderCurrept2.transform.localScale = new Vector3(((float)xSize / (float)ySize), 1, 1);

        //string originalData = "Original DATA: ", unsortedData = "Unsorted DATA: ", sortedData = "Sorted DATA: ";
       


        int n = bubbleData.Length;

        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (bubbleData[j] > bubbleData[j + 1])
                {
                    int temp = bubbleData[j];

                    bubbleData[j] = bubbleData[j + 1];
                    bubbleData[j + 1] = temp;

                    Color32 tempColor = pixelsBuffer[j];
                    pixelsBuffer[j] = pixelsBuffer[j + 1];
                    pixelsBuffer[j + 1] = tempColor;

                    
                }
            }
            if (i % visualizationSpeed == 0)
            {
                yield return null;
                bubbleSortImage.SetPixels32(pixelsBuffer);
                bubbleSortImage.Apply();
            }
        }

        yield return null;
        bubbleSortImage.SetPixels32(pixelsBuffer);
        bubbleSortImage.Apply();
        Debug.Log("HALT");
        

    }


    int[] heapData;
    Color32[] heapPixelsBuffer;
    void HeapSort() {
        //fixing memoryleaks
        heapSortImage = new Texture2D(xSize, ySize);
        heapPixelsBuffer = new Color32[pixels.Length];

        heapData = new int[ThisImageSortingData.shuffeledPosition.Length];

        for (int i = 0; i < ThisImageSortingData.shuffeledPosition.Length; i++)
        {
            heapData[i] = ThisImageSortingData.shuffeledPosition[i];
        }

        heapPixelsBuffer = curreptImage.GetPixels32();
        heapSortImage.SetPixels32(heapPixelsBuffer);
        heapSortImage.Apply();

        renderCurrept3.material.mainTexture = heapSortImage;
        renderCurrept3.transform.localScale = new Vector3(((float)xSize / (float)ySize), 1, 1);

        StartCoroutine(heapSort());
    }

    IEnumerator heapSort()
    {
        int n = heapData.Length;

        // Build heap (rearrange array)
        for (int i = n / 2 - 1; i >= 0; i--) {
            heapify(n, i);


            //if (i % visualizationSpeed == 0)
            //{
            //    yield return null;
            //    heapSortImage.SetPixels32(heapPixelsBuffer);
            //    heapSortImage.Apply();
            //}
        }
            

        // One by one extract an element from heap
        for (int i = n - 1; i > 0; i--)
        {
            // Move current root to end
            int temp = heapData[0];
            heapData[0] = heapData[i];
            heapData[i] = temp;

            Color32 tempCol = heapPixelsBuffer[0];
            heapPixelsBuffer[0] = heapPixelsBuffer[i];
            heapPixelsBuffer[i] = tempCol;

            // call max heapify on the reduced heap
            heapify( i, 0);
            if (i % visualizationSpeed == 0)
            {
                yield return null;
                heapSortImage.SetPixels32(heapPixelsBuffer);
                heapSortImage.Apply();
            }
        }
    }

    // To heapify a subtree rooted with node i which is
    // an index in arr[]. n is size of heap
    void heapify(int n, int i)
    {
        int largest = i; // Initialize largest as root
        int l = 2 * i + 1; // left = 2*i + 1
        int r = 2 * i + 2; // right = 2*i + 2

        // If left child is larger than root
        if (l < n && heapData[l] > heapData[largest])
            largest = l;

        // If right child is larger than largest so far
        if (r < n && heapData[r] > heapData[largest])
            largest = r;

        // If largest is not root
        if (largest != i)
        {
            int swap = heapData[i];
            heapData[i] = heapData[largest];
            heapData[largest] = swap;

            Color32 tempCol = heapPixelsBuffer[i];
            heapPixelsBuffer[i] = heapPixelsBuffer[largest];
            heapPixelsBuffer[largest] = tempCol;

            // Recursively heapify the affected sub-tree
            heapify(n, largest);
        }
    }
}


public class ImageSortingData {
    public int[] originalPosition;
    public int[] shuffeledPosition;

    public ImageSortingData(int pixelLength) {
        originalPosition = new int[pixelLength];
        shuffeledPosition = new int[pixelLength];

        for (int i = 0; i < pixelLength; i++)
        {
            originalPosition[i] = i;
            shuffeledPosition[i] = i;
        }
    }
}

//----------------Debugging CODE------------------------------

//string originalData = "Original DATA: ", unsortedData = "Unsorted DATA: ", sortedData = "Sorted DATA: ";
//for (int i = 0; i < heapData.Length; i++)
//{
//    originalData = originalData + "\n" + heapData[i];
//}



//for (int i = 0; i < heapData.Length; i++)
//{
//    unsortedData = unsortedData + "\n" + heapData[i];
//}


////Debug.Log("HALT");
//Debug.Log(originalData);
//Debug.Log(unsortedData);