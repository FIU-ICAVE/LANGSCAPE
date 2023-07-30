ProCOMPASS: Simple & Minimal Navigator
Created by Arnab Raha

Version 1.3.0 (22 May 2022)

ProCOMPASS: Simple & Minimal Navigator, is an all in one compass tool that is 
lightweight, easy to use and allows you to use indicators and minimap 
(Not Render Texture) on it in a few steps. The tool is also customizable, 
so you can customize it according to your game styles.

This tool is designed to be as simple as possible. The main features of ProCOMPASS: Simple & Minimal 
Navigator are: 

1. A basic rotating compass 
2. Object tracker (Indicators) with three customizable behaviour (Clip Indicator, Show direction 
when outside or show direction of the gameObject when not in viewing area, Do nothing or 
only show indicator when the object is in compass area) that is common for almost all sorts of 
game 
3. Minimap without Render Texture 
4. Customizable viewing options 
5. Multiple compass support 
6. Easy to use - just drag and drop prefabs to set it up 

Whats new:
==========
1. New Layer Mask will allow you to show objects of specific layers on the compass
2. Added an easy way to change the scale of indicators on the compass
3. Fixed some minor issues

How to use:
===========
1. Since the compass works with Unity UI only, you must create a Canvas if there isn’t any. Right-
click on the Hierarchy or go to UI > Canvas. 

2. Go to ProCOMPASS > Prefabs in the Project tab and select one of those prefabs and then drag 
and drop that in the Canvas gameObject in Hierarchy tab. Modify the position and scale of Rect 
Transform to set it as you wish. 

3. There should be a gameObject according to which the Compass will work. So, assign the player 
or main controller of the game in the Player field of the compass component. The basic compass 
is implemented. 

4. To add indicators, select the gameObjects in the scene you want to indicate on the compass. 
ProCompass uses the Tracker component to indicate a gameObject. So click Add Component in the Inspector, 
type Tracker in the search box and then click on Tracker.  

5. You will have to assign an indicator gameObject which will be indicating the 
position of this gameObject on the Compass. You can set different indicators for different gameObjects. You will find some specific indicator prefabs in ProCOMPASS > Prefabs > Indicators. Assign one of those in the Indicator field 
of the Tracker component you added to the gameObject before. 

6. Select one of the activities depending on how the indicator of this gameObject will appear on 
some specific event.

7. Go to Compass in the Canvas and then assign this gameObject in the Trackers list of the 
Compass component. Similarly, add as many Trackers as you need. 

8. The compass with indicator has been implemented. Now it is time to customize it. You can 
change the parameters of the Compass and see how it works.

9. Click on the Play button of the editor to see if it works properly. 

10.  To set up a Minimap in the Compass, assign necessary objects to the parameters which you will 
find in the Minimap section of the Compass component.

To see more about each public field of ProCompass and Tracker, see the Readme.pdf documentation.

To add or remove indicator through scripts, follow these simple steps. Directly adding or removing 
trackers from the Trackers list of the compass may create inconsistency or throw exceptions in the 
runtime. Instead, there are methods by which you will be able to add or remove indicator of a specific 
gameObject easily. You can use AddIndicator() to add and RemoveIndicator() to remove. Both method 
takes the Tracker, which you want to add or remove, as parameter. However, destroying a gameObject 
which contains Tracker and was added to Compass, without removing it would do the similar job as you 
do by calling RemoveIndicator() method. So, there would be no problem doing it, but it will show a 
warning message in the Console. You can disable this by commenting out a line in the Compass script. 
To add an indicator, create a reference of the compass and call AddIndicator() method from it. Simply 
pass the Tracker of the gameObject to the method.

using UnityEngine; 
public class AddIndicatorTest : MonoBehaviour { 
    public Compass compass; 
    private Tracker tracker; 
    void Start () { 
        tracker = GetComponent<Tracker> (); 
        compass.AddIndicator (tracker); 
    } 
}

To remove an indicator, just call RemoveIndicator()  but don’t try to remove a Tracker which hasn’t 
been added yet. Otherwise, it will show error message in the console.

using UnityEngine; 
public class RemoveIndicatorTest : MonoBehaviour { 
    public Compass compass; 
    public Tracker tracker; 
    void Start () { 
        // Do other initializations and things 
        tracker = GetComponent<Tracker> (); 
        compass.AddIndicator (tracker); 
    } 
    // From somewhere, let’s assume Death() method is called 
    void Death () { 
        compass.RemoveIndicator (tracker); 
    } 
}

Contact and Support
===================
For any kind of queries and help, you can contact me through email 
Arnab Raha 
Email: arnabraha501@gmail.com

Thank you for choosing ProCOMPASS