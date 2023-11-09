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
        // Unique Id
        public int id;

        // Constructor
        public Item(int id, int o_id)
        {
            this.o_id = o_id;
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

        /* :: Add Calls :: */
        // Creates New Object
        public int CreateObject(int objectId)
        {
            // Create Item based on Given Parameters
            Item item = new Item(u_id, objectId);
            // Add to Added Array
            Added.Add(item);
            // Increment Unique Id
            u_id++;

            return (item.id);
        }


        /* :: Remove Calls :: */
        // Delete 1st Object
        public void RemoveObjectByPosition(int pos)
        {
            Removed.Add(Added[pos]);
            Added.RemoveAt(pos);
        }
        // Deletes Object with Specific Unique Id
        public bool RemoveObjectByUniqueId(int uid)
        {
            var result = SearchByUniqueId(uid, Added);
            if (result.check)
            {
                RemoveObjectByPosition((int)result.position);
            }
            return result.check;
            
        }
        // Deletes nth Instance of Object with Specific Object Id, Returns Unique Value
        public int RemoveObjectByObjectId(int objectId, int n)
        {
            bool n_value = (n > 0 && n <= GetAddedCount());
            var result = SearchByObjectId(objectId, Added);
            int u_value = -1;
            if (result.check && n_value)
            {
                u_value = result.position[n-1];
                RemoveObjectByPosition((int)result.position[n-1]);
            }
            if (!n_value) { return u_value; }

            return u_value;
        }
        // Deletes All Objects
        public void RemoveAllObjects()
        {
            int aCount = GetAddedCount();
            for (int i = 0; i < aCount; i++)
            {
                Removed.Add(Added[i]);
            }
            Added.Clear();
        }
        // Deletes All Objects With Specific Object Id
        public void RemoveAllObjectsByObjectId(int objectId)
        {
            var result = SearchByObjectId(objectId, Added);
            if (result.check)
            {
                for (int i = 0; i < result.position.Length; i++)
                {
                    RemoveObjectByPosition((int)result.position[i]);
                }
            }
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
        public (bool added, bool removed, int objectId) RetrieveInfoByUniqueId(int uid)
        {
            var result = SearchByUniqueId(uid, Added);
            if (result.check)
            {
                Item item = Added[result.position];
                return (true, false, item.o_id);
            }
            else
            {
                result = SearchByUniqueId(uid, Removed);
                if (result.check)
                {
                    Item item = Removed[result.position];
                    return (false, true, item.o_id);
                }
                else
                {
                    return (false, false, -1);
                }
            }

        }
        public (int objectId, int uid) RetrieveAddedInfoByPosition(int pos)
        {
            return (Added[pos].o_id, Added[pos].id);
        }
        public (int objectId, int uid) RetrieveRemovedInfoByPosition(int pos)
        {
            return (Removed[pos].o_id, Removed[pos].id);
        }
    }
}