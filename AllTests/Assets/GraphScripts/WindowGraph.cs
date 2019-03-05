using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CodeMonkey.Utils;
using System.Threading.Tasks;

public class WindowGraph : MonoBehaviour {

    [SerializeField]
    private Sprite CircleSprite;

    private FireBaseController fire;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private RectTransform AxisLabelX;
    private RectTransform AxisLabelY;
    private List<GameObject> gameObjectList;
   
    private int currNoOfVals=0;

    List<float> ylst;
    List<string> xlst;
    string xAxisName;
    string yAxisName;
    private void Awake()
    {
       fire = GameObject.FindGameObjectWithTag("FireBaseObject").GetComponent<FireBaseController>();
       Task tsk=fire.loadUserDataAsync();
        StartCoroutine(waitForLoadData(tsk));
        ylst = new List<float>();
        xlst = new List<string>();

        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LableTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LableTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        AxisLabelX = graphContainer.Find("xAxisLabel").GetComponent<RectTransform>();
        AxisLabelY = graphContainer.Find("yAxisLabel").GetComponent<RectTransform>();
        // List<float> lst = new List<float>() {12,45,66,78,90,87,65,43,56 };







        transform.Find("AddButton").GetComponent<Button_UI>().ClickFunc = () =>
        {

            if (currNoOfVals + 1 <= xlst.Count) showGraph(xlst, ylst, ++currNoOfVals, xAxisName, yAxisName);
        };

        transform.Find("SubButton").GetComponent<Button_UI>().ClickFunc = () =>
        {

            if (currNoOfVals - 1 > 0) showGraph(xlst, ylst, --currNoOfVals, xAxisName, yAxisName);
        };

        transform.Find("distanceBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {

            if (LoadGraphData("DistanceKM")) showGraph(xlst, ylst, currNoOfVals, xAxisName, yAxisName);
        };
        transform.Find("speedBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {

            if (LoadGraphData("Speed")) showGraph(xlst, ylst, currNoOfVals, xAxisName, yAxisName);
        };
        transform.Find("timeBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {

            if (LoadGraphData("TimeMins")) showGraph(xlst, ylst, currNoOfVals, xAxisName, yAxisName);
        };
        transform.Find("caloriesBtn").GetComponent<Button_UI>().ClickFunc = () =>
        {

            if (LoadGraphData("Calories")) showGraph(xlst, ylst, currNoOfVals, xAxisName, yAxisName);
        };
    }

    IEnumerator waitForLoadData(Task tsk)
    {


        while (true)
        {
            if (LoadGraphData("Calories"))
            {
                showGraph(xlst, ylst, currNoOfVals, xAxisName, yAxisName);
                break;
            }
            else {
                yield return new WaitForSeconds(1);
            }
        }


    }


    private GameObject CreateCircle(Vector2 anchoredPos) {

        GameObject gameObject = new GameObject("circle",typeof(Image));
        gameObject.transform.SetParent(graphContainer,false);
        gameObject.GetComponent<Image>().sprite = CircleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        rectTransform.sizeDelta = new Vector2(11,11);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

  private void showGraph(List<string> xValues,List<float> yValues,int NoOfVals ,string xName,string yName) {
       

        float graphHeight = graphContainer.sizeDelta.y;
        float graphWidth = graphContainer.sizeDelta.x;
        float yMax = yValues[0];
        float yMin = yValues[0];
        float xSize = (graphWidth/ xValues.Count)  ;
        AxisLabelX.GetComponent<Text>().text = xName;
        AxisLabelY.GetComponent<Text>().text = yName;
        int startIndex = yValues.Count - NoOfVals;
        int totalVals = yValues.Count;
       

        if (gameObjectList != null)
        {
            foreach (GameObject obj in gameObjectList)
            {

                Destroy(obj);
            }
            gameObjectList.Clear();

        }
        else {

            gameObjectList = new List<GameObject>();
        }

       

        for (int i=startIndex;i<totalVals;i++) {
            if (yValues[i] > yMax) yMax = yValues[i];
            if (yValues[i] < yMin) yMin = yValues[i];
        }
       
        yMax =yMax + (yMax-yMin)*0.2f;
        yMin = yMin - (yMax - yMin) * 0.2f;

        if (yMax == yMin) yMin = 0;

        GameObject oldGameObj = null;
        for(int i = 0; i < NoOfVals; i++) {
            float xPosition = i * xSize+20f;
            float yPosition = ((yValues[i+startIndex]-yMin) /(yMax-yMin)) * graphHeight;

            GameObject currGameObj=CreateCircle(new Vector2(xPosition,yPosition));
            gameObjectList.Add(currGameObj);

            if (oldGameObj != null) {
               GameObject dotConnectionObject=createDotConnection(oldGameObj.GetComponent<RectTransform>().anchoredPosition, currGameObj.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionObject);
            }
            oldGameObj = currGameObj;
            RectTransform labelx = Instantiate(labelTemplateX);
            labelx.SetParent(graphContainer);
            labelx.gameObject.SetActive(true);
            labelx.anchoredPosition=new Vector2(xPosition,-30f);
            labelx.GetComponent < Text >().text= xValues[i + startIndex].ToString();
            gameObjectList.Add(labelx.gameObject);

            RectTransform dashx = Instantiate(dashTemplateY);
            dashx.SetParent(graphContainer);
            dashx.gameObject.SetActive(true);
            dashx.anchoredPosition = new Vector2(xPosition, -7f);
            gameObjectList.Add(dashx.gameObject);

        }

        int seperatorCount = 10;
        //
        for (int i = 0; i < seperatorCount; i++) {
            RectTransform labely = Instantiate(labelTemplateY);
            labely.SetParent(graphContainer);
            labely.gameObject.SetActive(true);
            float normalizedValue = i * 1f / seperatorCount;
            labely.anchoredPosition = new Vector2(-25f, normalizedValue*graphHeight);
            // labely.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * graphHeight).ToString();
            labely.GetComponent<Text>().text = Mathf.RoundToInt(yMin+ (yMax-yMin)*normalizedValue).ToString();
            gameObjectList.Add(labely.gameObject);


            RectTransform dashy = Instantiate(dashTemplateX);
            dashy.SetParent(graphContainer);
            dashy.gameObject.SetActive(true);
            dashy.anchoredPosition = new Vector2(-7f, normalizedValue*graphHeight);
            gameObjectList.Add(dashy.gameObject);

        }
    }
    
 private GameObject createDotConnection(Vector2 startPos,Vector2 endPos) {

        GameObject gameObject = new GameObject("dotConnection",typeof(Image));

        gameObject.transform.SetParent(graphContainer,false);
        gameObject.GetComponent<Image>().color = new Color(1,1,1,.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (endPos - startPos).normalized;
        float distance = Vector2.Distance(startPos,endPos);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.anchoredPosition = startPos+dir*distance*0.5f;
        //  rectTransform.localEulerAngles=new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        rectTransform.localEulerAngles = new Vector3(0, 0,AngleinnFloat(dir));

        return gameObject;
    }

    private float getAngleFromVectFloat(Vector2 source,Vector2 target) {

        float angle = Mathf.DeltaAngle(Mathf.Atan2(source.y, source.x) * Mathf.Rad2Deg,
                               Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg);
        return angle;
    }

    public static float AngleinnFloat(Vector3 p_vector2)
    {
        p_vector2 = p_vector2.normalized;
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.y, p_vector2.x) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.y, p_vector2.x) * Mathf.Rad2Deg;
        }
    }

   bool LoadGraphData(string type) {
        Debug.Log("lading graph data");
        if (!fire.dataIsLoaded())
        {
            SSTools.ShowMessage("Loading Sessions", SSTools.Position.bottom, SSTools.Time.twoSecond);
            return false;
        }
        else
        {
            List<Sessions> ls = fire.getUserData();
            if (ls.Count == 0)
            {
                SSTools.ShowMessage("No Sessions to Display", SSTools.Position.bottom, SSTools.Time.twoSecond);
                return false;
            }
            else {

                List<float> yList = new List<float>();
                List<string> xList = new List<string>();
                foreach ( Sessions ses in ls) {
                    float val = 0;
                    if (type == "DistanceKM") val = ses.DistanceKM;
                    else if (type == "TimeMins") val = ses.TimeMins;
                    else if (type == "Calories") val = ses.Calories;
                    else if (type == "Speed") val = (float)Math.Round(ses.DistanceKM * 60 / (ses.TimeMins), 2);
                    Debug.Log("val:"+val.ToString());
                    yList.Add(val);
                    string[] str = ses.key.Split(' ');
                    xList.Add(str[0]+"\n"+str[1]);
                }
                currNoOfVals = yList.Count;
                xlst = xList;
                ylst = yList;
                xAxisName = "Sessions(Time)";
                yAxisName = type;
                if (type == "Speed") yAxisName = "Speed(Kmph)";
                return true;  
            }
        }
   }
}

