using System;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectTracker
{
    /// <summary>
    /// Object Information
    /// </summary>
    public struct Item
    {
        // Object Identifcation
        public int o_id;
        // Object Name
        public string name;
        // Unique Id
        public int id;

        // Constructor
        public Item(int id, int o_id, string name)
        {
            this.o_id = o_id;
            this.name = name;
            this.id = id;
        }

    }

    /// <summary>
    /// Handles Object Tracking, Deletion, and Recovery
    /// </summary>
    public class ToyBox
    {
        // Variables
        List<Item> Added;
        List<Item> Removed;
        private int u_id;

        // Constructor
        public ToyBox()
        {
            Added = new List<Item> ();
            Removed = new List<Item> ();
            u_id = 0;
        }

        /* Information Calls */
        // Outputs String Containing Object Information, and Whether it's Added or Removed
        public string ObjectInfo(int uid)
        {
            // Variable
            string info = string.Format("Object {uid} Not Found");
            int aCount = GetAddedCount();
            int rCount = GetRemovedCount();

            // If Both Lists Are Empty, Check Neither Lists
            if(aCount == 0 && rCount == 0)
            {
                info = "No Objects Added Yet";
            }
            // If Only Added List is Empty, Check in Removed List
            else if(aCount == 0)
            {
                var result = SearchByUniqueId(uid, Removed);
                if (result.check)
                {
                    int rs = result.position;
                    info = string.Format("Id {uid} :: Object_Id => {Removed[rs].o_id} :: " +
                        "Name => {Removed[rs].name} :: Status => Removed ::");
                }
            }
            // If Only Removed List is Empty, Check in Added List
            else if(rCount == 0)
            {
                var result = SearchByUniqueId(uid, Added);
                if (result.check)
                {
                    int rs = result.position;
                    info = string.Format("Id {uid} :: Object_Id => {Added[rs].o_id} :: " +
                        "Name => {Added[rs].name} :: Status => Added ::");
                }
            }
            // If Both Lists Aren't Empty, Check in Both Lists
            else
            {
                var result = SearchByUniqueId(uid, Added);
                if (result.check)
                {
                    int rs = result.position;
                    info = string.Format("Id {uid} :: Object_Id => {Added[rs].o_id} :: " +
                        "Name => {Added[rs].name} :: Status => Added ::");
                }
                else
                {
                    result = SearchByUniqueId(uid, Removed);
                    if (result.check)
                    {
                        int rs = result.position;
                        info = string.Format("Id {uid} :: Object_Id => {Removed[rs].o_id} :: " +
                            "Name => {Removed[rs].name} :: Status => Removed ::");
                    }
                }
            }

            
            return info;
        }
        public string AddedObjectInfo(int uid)
        {
            string info = string.Format("Object {uid} hasn't been Spawned");
            var result = SearchByUniqueId(uid, Added);
            if (result.check)
            {
                int rs = result.position;
                info = string.Format("Id {uid} :: Object_Id => {Added[rs].o_id} :: " +
                        "Name => {Added[rs].name} :: Status => Added ::");
            }
            return info;
        }
        public string RemovedObjectInfo(int uid)
        {
            string info = string.Format("Object {uid} hasn't been Removed");
            var result = SearchByUniqueId(uid, Removed);
            if (result.check)
            {
                int rs = result.position;
                info = string.Format("Id {uid} :: Object_Id => {Removed[rs].o_id} :: " +
                            "Name => {Removed[rs].name} :: Status => Removed ::");
            }
            return info;
        }
        public string[] AllObjectsInfo()
        {
            if(GetTotalCount() <= 0)
            {
                return new string[0];
            }
            string[] holder = new string[GetTotalCount()];
            for(int i = 0; i < GetTotalCount(); i++)
            {
                var result = SearchByUniqueId(i, Added);
                if (result.check)
                {
                    int rs = result.position;
                    holder[i] = string.Format("Id {i} :: Object_Id => {Added[rs].o_id} :: " +
                        "Name => {Added[rs].name} :: Status => Added ::");
                }
                else
                {
                    int rs = result.position;
                    result = SearchByUniqueId(i, Removed);
                    holder[i] = string.Format("Id {i} :: Object_Id => {Removed[rs].o_id} :: " +
                            "Name => {Removed[rs].name} :: Status => Removed ::");
                }
                
            }
            return holder;
        }
        public string[] AllAddedObjectsInfo()
        {
            if (GetAddedCount() <= 0)
            {
                return new string[0];
            }
            string[] holder = new string[GetAddedCount()];
            for (int i = 0; i < GetAddedCount(); i++)
            {
                Item item = Added[i];

                holder[i] = string.Format("Id {item.id} :: Object_Id => {item.o_id} :: " +
                        "Name => {item.name} :: Status => Added ::");

            }
            return holder;
        }
        public string[] AllRemovedObjectsInfo()
        {
            if (GetRemovedCount() <= 0)
            {
                return new string[0];
            }
            string[] holder = new string[GetRemovedCount()];
            for (int i = 0; i < GetRemovedCount(); i++)
            {
                Item item = Removed[i];

                holder[i] = string.Format("Id {item.id} :: Object_Id => {item.o_id} :: " +
                            "Name => {item.name} :: Status => Removed ::");

            }
            return holder;
        }
        

        /* :: Add Calls :: */
        // Creates New Object
        public void CreateObject(int objectId, string objectName)
        {
            // Create Item based on Given Parameters
            Item item = new Item(u_id, objectId, objectName);
            // Add to Added Array
            Added.Add(item);
            // Increment Unique Id
            u_id++;
        }
        // Retrieves Deleted Object <Is this neccessary?, Ai will remember>
        public bool RecoverObject(int uid)
        {
            var result = SearchByUniqueId(uid, Removed);
            if(result.check)
            {
                Added.Add(Removed[result.position]);
                Removed.RemoveAt(result.position);
            }
            return result.check;
        }


        /* :: Remove Calls :: */
        // Deletes Object with Specific Unique Id
        public bool RemoveObjectByUniqueId(int uid)
        {
            var result = SearchByUniqueId(uid, Added);
            if (result.check)
            {
                Removed.Add(Added[result.position]);
                Added.RemoveAt(result.position);
            }
            return result.check;
            
        }
        // Deletes nth Instance of Object with Specific Object Id
        public bool RemoveObjectByObjectId(int objectId, int n)
        {
            bool n_value = (n > 0 && n <= GetAddedCount());
            var result = SearchByObjectId(objectId, Added);

            if (result.check && n_value)
            {
                Removed.Add(Added[result.position[n-1]]);
                Added.RemoveAt(result.position[n-1]);
            }
            if (!n_value) { return false; }

            return result.check;
        }
        // Deletes All Objects
        public bool RemoveAllObjects()
        {
            int aCount = GetAddedCount();
            if (aCount == 0)
            {
                return false;
            }
            for(int i = 0; i < aCount; i++)
            {
                Removed.Add(Added[i]);
            }
            Added.Clear();

            return true;
        }
        // Deletes All Objects With Specific Object Id
        public bool RemoveAllObjectsByObjectId(int objectId)
        {
            var result = SearchByObjectId(objectId, Added);
            if (result.check)
            {
                for(int i = 0; i < result.position.Length; i++)
                {
                    Removed.Add(Added[result.position[i]]);
                    Added.RemoveAt(result.position[i]);
                }
            }
            return result.check;
        }

        /* :: Search Calls :: */
        // Search by Unique Id
        private (bool check, int position) SearchByUniqueId(int uid, List<Item> list)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if(list[i].id == uid)
                {
                    return (true, i);
                }
            }
            return (false, -1);
        }
        // Search by Object Id
        private (bool check, int[] position) SearchByObjectId(int objectId, List<Item> list)
        {
            List<int> holder = new List<int>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].o_id == objectId)
                {
                    holder.Add(i);
                }
            }
            if (list.Count > 0)
            {
                int[] hold = holder.ToArray();
                return (true, hold);
            }
            else
            {
                int[] hold = new int[0];
                return (false, hold);
            }
        }

        /* :: Count Calls */
        // <Can Ai Handle This? Or Should These be Treated as Utility?>
        public int GetAddedCount()
        {
            return Added.Count;
        }
        public int GetRemovedCount()
        {
            return Removed.Count;
        }
        public int GetTotalCount()
        {
            return GetAddedCount() + GetRemovedCount();
        }
        public int GetAddedObjectCount(int objectId)
        {
            var result = SearchByObjectId(objectId, Added);
            
            return result.position.Length;
        }
        public int GetRemovedObjectCount(int objectId)
        {
            var result = SearchByObjectId(objectId, Removed);

            return result.position.Length;
        }
        public int GetTotalObjectCount(int objectId)
        {
            return GetAddedObjectCount(objectId) + GetRemovedObjectCount(objectId);
        }

        /* :: Getter Calls */
        public (bool added, bool removed, int objectId, string name) RetrieveInfoByUniqueId(int uid)
        {
            var result = SearchByUniqueId(uid, Added);
            if (result.check)
            {
                Item item = Added[result.position];
                return (true, false, item.o_id, item.name);
            }
            else
            {
                result = SearchByUniqueId(uid, Removed);
                if (result.check)
                {
                    Item item = Removed[result.position];
                    return (false, true, item.o_id, item.name);
                }
                else
                {
                    return (false, false, -1, null);
                }
            }

        }
    }
}